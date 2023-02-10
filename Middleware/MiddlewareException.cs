using System.Net;

namespace NetKubernetes.Middleware
{
    public class MiddlewareException: Exception
    {
        public HttpStatusCode Codigo { get; set; }
        public object? Errores { get; set; }
        public MiddlewareException(HttpStatusCode Codigo, object? Errores = null)
        {
            this.Codigo = Codigo;
            this.Errores = Errores;
        }
    }
}