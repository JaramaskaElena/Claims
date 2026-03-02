using Claims.Domain.Entities;

namespace Claims.Domain.Events
{
    public class CoverCreatedEvent
    {
        public Cover Cover { get; }
        public CoverCreatedEvent(Cover cover)
        {
            Cover = cover;
        }
    }
}