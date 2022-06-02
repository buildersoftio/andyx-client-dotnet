using System.Collections.Generic;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions.Producers
{
    public interface IProducerOtherConfiguration<T>
    {
        IProducerOtherConfiguration<T> AddDefaultHeader(string key, string value);
        IProducerOtherConfiguration<T> AddDefaultHeader(IDictionary<string, string> headers);
        IProducerOtherConfiguration<T> RetryProducingIfFails();
        IProducerOtherConfiguration<T> HowManyTimesToTryProducing(int nTimesRetry);


        Producer<T> Build();
        Task OpenAsync();
        Task CloseAsync();
    }
}
