using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions.Consumers
{
    public interface IConsumerOtherConfiguration<T>
    {
        Consumer<T> Build();

        Task ConnectAsync();
        Task DisconnectAsync();
    }
}
