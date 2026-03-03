using Claims.Domain.Entities;

namespace Claims.Domain.Events
{
    public class ClaimCreatedEvent
    {
        public Claim Claim { get; }
        public ClaimCreatedEvent(Claim claim)
        {
            Claim = claim;
        }
    }
}