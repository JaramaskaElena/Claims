using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Events;
using Claims.Infrastructure.Queue;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Infrastructure.Handlers
{
    public class CoverCreatedAuditHandler : IEventHandler<CoverCreatedEvent>
    {
        private readonly IBackgroundQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        public CoverCreatedAuditHandler(
            IBackgroundQueue queue,
            IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }

        public Task HandleAsync(CoverCreatedEvent @event)
        {
            _queue.Enqueue(async token =>
            {
                using var scope = _scopeFactory.CreateScope();
                var auditRepo = scope.ServiceProvider.GetRequiredService<IAuditRepository>();

                await auditRepo.AddCoverAuditAsync(new CoverAudit
                {
                    CoverId = @event.Cover.Id,
                    HttpRequestType = "CREATE",
                    Created = DateTime.UtcNow
                });
            });

            return Task.CompletedTask;
        }
    }
}