using Claims.Domain.Entities;
using Microsoft.EntityFrameworkCore;
//using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Infrastructure.Data
{
    public class ClaimsContext : DbContext
    {
        public ClaimsContext(DbContextOptions<ClaimsContext> options) : base(options) { }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Cover> Covers { get; set; }
    }
}