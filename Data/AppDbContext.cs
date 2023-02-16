using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetKubernetes.Models;

namespace NetKubernetes.Data
{
    // la clase AppDbContext que configura la base de datos y hereda una configuración de
    // IdentityDbContext con Usuario para generar configuración para autenticación 
    public class AppDbContext: IdentityDbContext<Usuario>
    {
        // crear configuracion de DbContextOptions para registrar tablas
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Generate tables databases
        }

        // registrar tabla Inmueble
        public DbSet<Inmueble>? Inmuebles { get; set; }


    }
}