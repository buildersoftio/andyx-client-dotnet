using Andy.X.Client;
using Andy.X.Client.Nodes;
using andyx_examples.Models;

namespace andyx_examples
{
    public class SimpleProducer
    {
        private readonly Producer<SimpleMessage> producer;
        public SimpleProducer()
        {
            XClient client = XClient.CreateConnection()
                .ForService("192.168.0.24", 6540, NodeConnectionType.NON_SSL)
                .AndTenant("default")
                .AndProduct("default")
                .Build();

            producer = Producer<SimpleMessage>.CreateNewProducer(client)
                .ForComponent("simple")
                .AndTopic("simple-message")
                .WithName("simple-producer")
                .Build();

            producer.OpenAsync().Wait();
        }

        public void ProduceSimpleMessages(int id, string name, string message)
        {
            var simpleMessage = new SimpleMessage() { Id = id, Name = name, Message = message };
            producer.Produce(simpleMessage);
        }
    }
}
