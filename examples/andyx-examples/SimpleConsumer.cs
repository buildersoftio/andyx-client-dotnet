using Andy.X.Client;
using Andy.X.Client.Configurations;
using Andy.X.Client.Nodes;
using andyx_examples.Models;
using System;

namespace andyx_examples
{
    public class SimpleConsumer
    {
        private readonly Consumer<SimpleMessage> consumer;
        public SimpleConsumer()
        {
            XClient client = XClient.CreateConnection()
                .ForService("localhost", 6541, NodeConnectionType.SSL)
                .AndTenant("default")
                .AndProduct("default")
                .Build();

            consumer = Consumer<SimpleMessage>.CreateNewConsumer(client)
                .ForComponent("simple")
                .AndTopic("simple-message")
                .WithName("simple-consumer")
                .WithInitialPosition(InitialPosition.Earliest)
                .AndSubscriptionType(SubscriptionType.Exclusive)
                .Build();

                consumer.MessageReceived += Consumer_MessageReceived;

            consumer
                .ConnectAsync()
                .Wait();
        }

        private bool Consumer_MessageReceived(object sender, Andy.X.Client.Events.Consumers.MessageReceivedArgs<SimpleMessage> e)
        {
            Console.WriteLine($"Message arrived: payload as raw: '{e.Payload}'; payload as simpleMessage name='{e.GenericPayload.Name}'");

            // Message acknowledged
            return true;
        }
    }
}
