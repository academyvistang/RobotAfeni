using ConsoleAlwaysRunningCheckingServiceBus.Impl;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAlwaysRunningCheckingServiceBus
{
    class Program
    {
        private const string serviceBusConnectionString = "Endpoint=sb://bookingreservation.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=hiJzgEQhWmvxR3N9Y2LXPn5u/zwgkRq165hz7DEDMgA=";

        private const string topicName = "glee";
        //private static ITopicClient topicClient;
        const string subscriptionName = "gleeSubscription";
        static ISubscriptionClient subscriptionClient;


        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            try
            {
                subscriptionClient = new SubscriptionClient(serviceBusConnectionString, topicName, subscriptionName);

                //var builder = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);
                ////subscriptionClient = new SubscriptionClient(serviceBusConnectionString, topicName, subscriptionClient);
                //subscriptionClient = new SubscriptionClient(builder, subscriptionName);
                //subscriptionClient = new SubscriptionClient()

                Console.WriteLine("======================================================");
                Console.WriteLine("Press ENTER key to exit after receiving all the messages.");
                Console.WriteLine("======================================================");

                // Register subscription message handler and receive messages in a loop
                RegisterOnMessageHandlerAndReceiveMessages();



                Console.ReadKey();

                await subscriptionClient.CloseAsync();
            }
            catch (Exception ex)
            {
                int p = 90;
            }
            
        }

        static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };

            // Register the function that processes messages.
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message.
            //Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            var model = JsonConvert.DeserializeObject<MessagePayload>(Encoding.UTF8.GetString(message.Body));

            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{model.ToString()}");

            int i = 90;

            if(i == 90)
            {
                //Put fully booked in cache
                try
                {
                    var client = new SendGridClient("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");
                    var msg = new SendGridMessage();

                    msg.SetFrom(new EmailAddress("afeniohiro@hotmail.com", "Cabbash App"));

                    var recipients = new List<EmailAddress>
                {
                    new EmailAddress(model.data.providerEmailAddress),
                    new EmailAddress("academyvistang@gmail.com"),
                    new EmailAddress("bruno.ohiro@nhs.net")
                };
                    msg.AddTos(recipients);

                    msg.SetSubject("Mail from Azure and SendGrid");

                    msg.AddContent(MimeType.Text, "Reservation could not be fulfilled 12");
                    msg.AddContent(MimeType.Html, "<p>Only 12 Deluxe rooms available for NGN 28000.00</p>");
                    var response = await client.SendEmailAsync(msg);
                }
                catch(Exception ex)
                {
                    int p = 90;
                }
                
            }



            // Complete the message so that it is not received again.
            // This can be done only if the subscriptionClient is created in ReceiveMode.PeekLock mode (which is the default).
            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the subscriptionClient has already been closed.
            // If subscriptionClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
            // to avoid unnecessary exceptions.
        }
    }
}
