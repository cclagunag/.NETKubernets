using System.Security.Claims;
namespace NetKubernetes.Token
{
    public class UsuarioSesion : IUsuarioSesion
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UsuarioSesion(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }
        public string ObtenerUsuarioSesion()
        {
            // obtener el username que está en sesión de usuario en la aplicación 
            // mediante el IHttpContextAccessor _httpContextAccessor;
            var userName = _httpContextAccessor.HttpContext!.User?.Claims?
            .FirstOrDefault(x=>x.Type == ClaimTypes.NameIdentifier)?.Value;
            return userName!;
        }
    }
}