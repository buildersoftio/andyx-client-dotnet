using Andy.X.Client;
using Andy.X.Client.Configurations;
using andyx_examples.Models;
using System;

namespace andyx_examples
{
    public class SimpleConsumer
    {
        private readonly Consumer<SimpleMessage> consumer;
        public SimpleConsumer()
        {
            XClient client = new XClient("https://localhost:6541");

            consumer = new Consumer<SimpleMessage>(client)
                .Name("simple-consumer")
                .Component("simple")
                .Topic("simple-message")
                .InitialPosition(InitialPosition.Earliest)
                .SubscriptionType(SubscriptionType.Exclusive).Build() as Consumer<SimpleMessage>;

            consumer.MessageReceived += Consumer_MessageReceived;

            consumer
                .SubscribeAsync()
                .Wait();
        }

        private bool Consumer_MessageReceived(object sender, Andy.X.Client.Events.Consumers.MessageReceivedArgs<SimpleMessage> e)
        {
            Console.WriteLine($"Message arrived: payload as raw: '{e.RawData}'; payload as simpleMessage name='{e.Data.Name}'");

            // Message acknowledged
            return true;
        }
    }
}
