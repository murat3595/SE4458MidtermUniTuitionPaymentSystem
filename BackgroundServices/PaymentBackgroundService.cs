
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.ComponentModel;
using System.Text;
using System.Threading.Channels;
using TuitionApi.Models.Dto;

namespace TuitionApi.Jobs
{
    public class PaymentBackgroundService : BackgroundService
    {
        private IConnection _rbConnection;
        private IModel _channelModel;
        private DummyDataContext dbContext;

        public PaymentBackgroundService(IConfiguration configuration, DummyDataContext dbContext)
        {
            this.dbContext = dbContext;

            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:hostname"],
                Port = Convert.ToInt32(configuration["RabbitMQ:port"]),
                UserName = String.IsNullOrEmpty(configuration["RabbitMQ:username"]) ? ConnectionFactory.DefaultUser : configuration["RabbitMQ:username"],
                Password = String.IsNullOrEmpty(configuration["RabbitMQ:password"]) ? ConnectionFactory.DefaultPass : configuration["RabbitMQ:password"]
            };

            _rbConnection = connectionFactory.CreateConnection();

            _channelModel = _rbConnection.CreateModel();

            _channelModel.ExchangeDeclare("tuition.exchange", ExchangeType.Topic);
            _channelModel.QueueDeclare("tuition.queue.payment", false, false, false, null);
            _channelModel.QueueBind("tuition.queue.payment", "tuition.exchange", "tuition.queue.*", null);
            _channelModel.BasicQos(0, 1, false);

        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channelModel);

            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());

                var paymentMessage = JsonConvert.DeserializeObject<PaymentQueueDto>(message);

                var payment = dbContext.Payments.First(s => s.PaymentId == paymentMessage.PaymentID);

                if (payment.Success == true)
                {
                    payment.Processed = true;
                    Console.WriteLine("Processed"); //if we had used Signalr or maybe something like Firebase we would push message to user here.

                    var student = dbContext.Users.First(s => s.UserId == payment.UserId);

                    student.Balance += (float)payment.Amount;

                    dbContext.SaveChanges();
                }

                Console.WriteLine(message);

                _channelModel.BasicAck(e.DeliveryTag, false); //acknowledging this the point that we really consuming the message.
            };

            _channelModel.BasicConsume("tuition.queue.payment", false, consumer);

            return Task.CompletedTask;
        }


        public override void Dispose()
        {
            _rbConnection?.Dispose();
            base.Dispose();
        }
    }
}
