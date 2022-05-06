using System.Collections.Generic;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions.Producers
{
    public interface IProducerOtherConfiguration<T>
    {
        IProducerOtherConfiguration<T> AddDefaultHeader(string key, object value);
        IProducerOtherConfiguration<T> AddDefaultHeader(IDictionary<string, object> headers);
        IProducerOtherConfiguration<T> RetryProducingIfFails();
        IProducerOtherConfiguration<T> HowManyTimesToTryProducing(int nTimesRetry);


        Producer<T> Build();
        Task OpenAsync();
        Task CloseAsync();
    }
}
