using Microsoft.AspNetCore.Identity;

namespace NetKubernetes.Models
{
    public class Usuario: IdentityUser
    {
        //ID
        //EMAIL
        //USERNAME
        public String? Nombre { get; set; }
        public String? Apellido { get; set; }
        public String? Telefono { get; set; }
    }
}