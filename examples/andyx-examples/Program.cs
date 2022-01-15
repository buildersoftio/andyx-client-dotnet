using System;

namespace andyx_examples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Andy X Client");
            Console.WriteLine(".NET Example");
            Console.WriteLine("Simple Consumer and Producer");

            // initialize Consumer
            SimpleConsumer consumer = new SimpleConsumer();

            // initialize Producer
            SimpleProducer producer = new SimpleProducer();
            producer.ProduceSimpleMessages(1, "Andy", "Hello for Andy X the newest distributed streaming platform!");

            Console.ReadLine();
        }
    }
}
