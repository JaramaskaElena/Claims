namespace Claims.Domain.Entities
{
    public class ClaimAudit
    {
        public int Id { get; set; }
        public Guid ClaimId { get; set; }
        public DateTime Created { get; set; }
        public string HttpRequestType { get; set; }
    }
}