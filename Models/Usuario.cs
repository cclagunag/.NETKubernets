using Microsoft.AspNetCore.Identity;

namespace NetKubernetes.Models
{
    // la clase usuario hereda las propiedades de IdentityUser que contiene otros campos
    //ID
    //EMAIL
    //USERNAME
    public class Usuario : IdentityUser
    {
        //ID
        //EMAIL
        //USERNAME
        public String? Nombre { get; set; }
        public String? Apellido { get; set; }
        public String? Telefono { get; set; }
    }
}