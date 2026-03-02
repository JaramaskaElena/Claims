using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Events;
using Claims.Infrastructure.Queue;

namespace Claims.Infrastructure.Handlers
{
    public class ClaimCreatedAuditHandler : IEventHandler<ClaimCreatedEvent>
    {
        private readonly IBackgroundQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        public ClaimCreatedAuditHandler(
            IBackgroundQueue queue,
            IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }

        public Task HandleAsync(ClaimCreatedEvent @event)
        {
            _queue.Enqueue(async token =>
            {
                using var scope = _scopeFactory.CreateScope();
                var auditRepo = scope.ServiceProvider.GetRequiredService<IAuditRepository>();

                await auditRepo.AddClaimAuditAsync(new ClaimAudit
                {
                    ClaimId = @event.Claim.Id,
                    HttpRequestType = "CREATE",
                    Created = DateTime.UtcNow
                });
            });

            return Task.CompletedTask;
        }
    }
}