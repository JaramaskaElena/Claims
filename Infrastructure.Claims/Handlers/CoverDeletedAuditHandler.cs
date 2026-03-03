using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Events;
using Claims.Infrastructure.Queue;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Infrastructure.Handlers
{
    public class CoverDeletedAuditHandler : IEventHandler<CoverDeletedEvent>
    {
        private readonly IBackgroundQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        public CoverDeletedAuditHandler(
            IBackgroundQueue queue,
            IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }

        public Task HandleAsync(CoverDeletedEvent @event)
        {
            _queue.Enqueue(async token =>
            {
                using var scope = _scopeFactory.CreateScope();
                var auditRepo = scope.ServiceProvider.GetRequiredService<IAuditRepository>();

                await auditRepo.AddCoverAuditAsync(new CoverAudit
                {
                    CoverId = @event.CoverId,
                    HttpRequestType = "DELETE",
                    Created = DateTime.UtcNow
                });
            });

            return Task.CompletedTask;
        }
    }
}