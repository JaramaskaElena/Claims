using Claims.Domain.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Claims.Domain.Entities
{
    public class Claim
    {
        [BindNever]
        public Guid Id { get; set; }
        public Guid CoverId { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public ClaimType Type { get; set; }
        public decimal DamageCost { get; set; }
    }
}