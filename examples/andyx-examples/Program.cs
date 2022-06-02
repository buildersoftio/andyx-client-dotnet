using System;
using System.Diagnostics;
using System.Threading;

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
            Console.WriteLine($"Production started");
            Stopwatch stopwatch = Stopwatch.StartNew();
            SimpleProducer producer = new SimpleProducer();
            for (int i = 0; i < 20000000; i++)
            {
                producer.ProduceSimpleMessages(i, "Andy", "Hello for Andy X the newest distributed streaming platform!");
                Thread.Sleep(2);
            }

            stopwatch.Stop();
            Console.WriteLine($"Finished for : {stopwatch.Elapsed.TotalSeconds} sec");

            Console.ReadLine();
        }
    }
}
