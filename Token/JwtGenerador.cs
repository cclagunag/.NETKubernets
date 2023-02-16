using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using NetKubernetes.Models;

namespace NetKubernetes.Token
{
    public class JwtGenerador : IJwtGenerador
    {
        public string CrearToken(Usuario usuario)
        {
            /* 
                los claims son los datos que se van a cifrar e incluir en los tokens en este caso es
                - usuarioId
                - email
             */
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.NameId, usuario.UserName!),
                new Claim("userId", usuario.Id),
                new Claim("email", usuario.Email!)
            };

            /* 
                se genera un key en bytes para cifrar el jwt
             */
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("M1Pa1abraS3cr3ta3sM1a"));
            /* 
                se registra el tipo de cifrado en este caso es HmacSha512Signature con la key
             */
            var credenciales = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            // generar la descripcion del token
            var tokenDescription = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims), // Sucjects se registra los claims
                Expires = DateTime.Now.AddDays(30), // tiempo de expiración
                SigningCredentials = credenciales // registra el algoritmo de cifrado
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription); // crear token mediante tokenDescription
            return tokenHandler.WriteToken(token);  // retornar el token tipo string
        }
    }
}