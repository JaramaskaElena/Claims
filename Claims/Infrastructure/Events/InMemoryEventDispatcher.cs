using Claims.Application.Interfaces;

namespace Claims.Infrastructure.Events
{
    public class InMemoryEventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public InMemoryEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync<TEvent>(TEvent @event)
        {
            var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();
            var tasks = handlers.Select(h => h.HandleAsync(@event));
            await Task.WhenAll(tasks);
        }
    }
}