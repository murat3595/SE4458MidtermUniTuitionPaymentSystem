using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using TuitionApi.Models.Dto;

namespace TuitionApi.Controllers.V1
{

    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/v{version:apiVersion}/Tuition")]
    [ControllerName("Tuition Version 1.0")]
    [ApiController]
    public class TuitionController : ControllerBase
    {
        private DummyDataContext _dummyDataContext;
        private IConfiguration configuration;
        public TuitionController(DummyDataContext dummyDataContext, IConfiguration configuration)
        {
            this._dummyDataContext = dummyDataContext;
            this.configuration = configuration;
        }

        /// <summary>
        /// Get users tuition and balance with his/her profile information 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getTuitionOfStudent")]
        public IActionResult GetTuition([FromQuery] int id)
        {
            var result = _dummyDataContext.Users
                .Where(s => s.UserId == id)
                .Select(k => new TuitionDto
                    {
                        Balance = k.Balance,
                        Tuition = k.TuitionTotal,
                        User = new UserDto
                        {
                            Name = k.Name,
                            Surname = k.Surname,
                        }
                    })
                .FirstOrDefault();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Send payment amount with userId. You will get a paymentStatus success result if it was succesful. 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("payTuitionOfStudent")]
        public IActionResult PayTuition([FromBody] PayTuitionRequestDto request)
        {
            var user = _dummyDataContext.Users.FirstOrDefault(s => s.UserId == request.UserID);

            if (user == null)
                return NotFound(new
                {
                    PaymentStatus = "Error"
                });

            //user.Balance += request.PaymentAmount; this will be made by PaymentBackgroundService

            var payment = new Models.Data.PaymentModel(
                Guid.NewGuid().ToString(),
                request.UserID,
                request.PaymentAmount,
                false,
                true);

            _dummyDataContext.Payments.Add(
                payment
            );

            var connectionFactory = new RabbitMQ.Client.ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:hostname"],
                Port = Convert.ToInt32(configuration["RabbitMQ:port"]),
                UserName = String.IsNullOrEmpty(configuration["RabbitMQ:username"]) ? ConnectionFactory.DefaultUser : configuration["RabbitMQ:username"],
                Password = String.IsNullOrEmpty(configuration["RabbitMQ:password"]) ? ConnectionFactory.DefaultPass : configuration["RabbitMQ:password"]
            };

            var connection = connectionFactory.CreateConnection();
            var _channelModel = connection.CreateModel();

            var properties = _channelModel.CreateBasicProperties();
            _channelModel.BasicPublish("tuition.exchange", "tuition.queue.payment", properties, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new PaymentQueueDto
            {
                PaymentID = payment.PaymentId,
                Processed = payment.Processed,
                Success = payment.Success,
            })));

            connection.Dispose();

            _dummyDataContext.SaveChanges();

            return Ok (payment);
        }
        //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImFkbWluIiwiZXhwIjoxNzE0NTIyMzI5LCJpc3MiOiJNdXJhdCBCb3prdXJ0IiwiYXVkIjoiRGnEn2VybGVyaSJ9.3Xamevs7QP88uoMEDuCuklmOsYSb7JE7JcnG9rjtjjI
        /// <summary>
        /// To add tuition to a user send additional tuition with userId. This can be used in 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("addTuitionForStudent")]
        public IActionResult AddTuitionToUser([FromBody] AddTuitionToUserRequest request)
        {
            var user = _dummyDataContext.Users.FirstOrDefault(s => s.UserId == request.UserID);

            if (user == null)
                return NotFound(new
                {
                    TransactionStatus = "error",
                });

            user.TuitionTotal += request.AdditionalTuition;


            return Ok(new
            {
                TransactionStatus = "success",
                Tuition = new TuitionDto
                {
                    Balance = user.Balance,
                    Tuition = user.TuitionTotal,
                    User = new UserDto
                    {
                        Name = user.Name,
                        Surname = user.Surname,
                    }
                }
            });
        }

        /// <summary>
        /// This will return only the students who haven't paid their tuition yet. isThereMore field is telling you if there is more pages.
        /// </summary>
        /// <param name="elementCount"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet("GetUnpaidStudents")]
        [Authorize]
        public IActionResult GetUnpaidUsers([FromQuery] int elementCount, [FromQuery] int pageIndex)
        {
            var result = _dummyDataContext.Users
                .Where(s => s.Balance < s.TuitionTotal)
                .Skip(elementCount * pageIndex)
                .Take(elementCount + 1)
                .Select(s => new TuitionDto
                {
                    Balance = s.Balance,
                    Tuition = s.TuitionTotal,
                    User = new UserDto { Name = s.Name, Surname = s.Surname,}
                }).ToList();

            var isThereMore = result.Count > elementCount;

            if (isThereMore)
            {
                result = result.Take(elementCount).ToList();
            }

            return Ok(new
            {
                IsThereMore = isThereMore,
                Students = result
            });
        }

    }
}
