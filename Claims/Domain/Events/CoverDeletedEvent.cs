namespace Claims.Domain.Events
{
    public class CoverDeletedEvent
    {
        public Guid CoverId { get; }
        public CoverDeletedEvent(Guid coverId)
        {
            CoverId = coverId;
        }
    }
}