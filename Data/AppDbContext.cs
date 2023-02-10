using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetKubernetes.Models;

namespace NetKubernetes.Data
{
    public class AppDbContext: IdentityDbContext<Usuario>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Generate tables databases
        }

        public DbSet<Inmueble>? Inmuebles { get; set; }


    }
}