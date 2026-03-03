namespace Claims.Domain.Events
{
    public class ClaimDeletedEvent
    {
        public Guid ClaimId { get; }
        public ClaimDeletedEvent(Guid claimId)
        {
            ClaimId = claimId;
        }
    }
}