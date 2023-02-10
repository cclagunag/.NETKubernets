using NetKubernetes.Models;

namespace NetKubernetes.Token
{
    public interface IJwtGenerador
    {
        public string CrearToken(Usuario usuario);
    }
}