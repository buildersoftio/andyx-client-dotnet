using Andy.X.Client;
using andyx_examples.Models;

namespace andyx_examples
{
    public class SimpleProducer
    {
        private readonly Producer<SimpleMessage> producer;
        public SimpleProducer()
        {
            XClient client = new XClient("https://localhost:9001");

            producer = new Producer<SimpleMessage>(client)
                .Name("simple-producer")
                .Component("simple")
                .Topic("simple-message")
                .RetryProducing(true)
                .BuildAsync().Result as Producer<SimpleMessage>;

            producer.OpenAsync().Wait();
        }

        public void ProduceSimpleMessages(int id, string name, string message)
        {
            var simpleMessage = new SimpleMessage() { Id = id, Name = name, Message = message };
            producer.Produce(simpleMessage);
        }
    }
}
