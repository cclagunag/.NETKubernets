using Microsoft.AspNetCore.Identity;
using NetKubernetes.Models;

namespace NetKubernetes.Data;

public class LoadDatabase
{
    public static async Task InsertarData(AppDbContext context, UserManager<Usuario> usuarioManager)
    {
        // si no hay usuarios registrados entonces crear usuarios
        if (!usuarioManager.Users.Any())
        {
            var usuario = new Usuario
            {
                Nombre = "Christian",
                Apellido = "Laguna",
                Email = "cclagunag@udistrital.edu.co",
                UserName = "cclagunag",
                Telefono = "76552146"
            };

            await usuarioManager.CreateAsync(usuario, "Laguna#00");
        }
        // si no hay inmuebles registrados entonces crear inmuebles
        if (!context.Inmuebles!.Any())
        {

            context.Inmuebles!.AddRange(
                new Inmueble {
                    Nombre = "Casa en el antro",
                    Direccion = "Avenida sol playa antro",
                    Precio = 4500M,
                    FechaCreacion = DateTime.Now
                },
                new Inmueble {
                    Nombre = "Casa en el antro Invierno",
                    Direccion = "Avenida sol playa antro Invierno",
                    Precio = 3500M,
                    FechaCreacion = DateTime.Now
                },
                new Inmueble {
                    Nombre = "Casa en el antro Verano",
                    Direccion = "Avenida sol playa antro Verano",
                    Precio = 5500M,
                    FechaCreacion = DateTime.Now
                }
            );
        }
        // guardar cambios a la base de datos
        context.SaveChanges();

    }
}