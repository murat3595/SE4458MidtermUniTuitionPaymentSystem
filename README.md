# To Run:

To run this you need a RabbitMQ instance. 
In my case I used *WSL Ubuntu** to create and RabbitMQ instance like this:


```
sudo apt update && sudo apt upgrade -y¨

sudo apt install curl gnupg -y
curl -fsSL https://packages.rabbitmq.com/gpg | sudo apt-key add -

sudo add-apt-repository 'deb https://dl.bintray.com/rabbitmq/debian focal main'

sudo apt update 

sudo apt install rabbitmq-server -y

sudo systemctl enable rabbitmq-server
sudo systemctl start rabbitmq-server

sudo systemctl status rabbitmq-server

sudo rabbitmq-plugins enable rabbitmq_management

```

after you do these you can access to management console of RabbitMQ instance from  [http://localhost:15672](http://localhost:15672) here.

default login and password are "guest".

after you do this you can directly run this project. Or maybe you have another RabbitMQ instance then in this case please change the necessary places in the *appsettings.json* file.

In *appsettings.json* file there is section called RabbitMQ. 
Change hostname and port fields according to your needs but by default after doing the steps above what already there would be enough.
If your rabbitmq instance has username and password then set those there to.


# My Design And Assumptions

Push notification would be signalr or some other 3rd party service like Firebase. But in this project 

*BackgroundServices/PaymentBackgroundService.cs*

background service will just print Messages instead pushing them anywhere after consuming from RabbitMQ channels, and make the payment happen by reducing students debt.

Consumers will call Pay on TuitionController and from there we will push message to RabbitMQ
then from background service, we will consume these messages and act accordingly.
