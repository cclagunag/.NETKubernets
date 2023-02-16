using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetKubernetes.Dtos.UsuarioDtos;
using NetKubernetes.Middleware;
using NetKubernetes.Models;
using NetKubernetes.Token;

namespace NetKubernetes.Data.Usuarios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IJwtGenerador _jwtGenerador;
        private readonly AppDbContext _context;
        private readonly IUsuarioSesion _usuarioSesion;
        public UsuarioRepository(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IJwtGenerador jwtGenerador,
            AppDbContext context,
            IUsuarioSesion usuarioSesion
            )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._jwtGenerador = jwtGenerador;
            this._context = context;
            this._usuarioSesion = usuarioSesion;
        }

        private UsuarioResponseDto TransformerUsertoUserDto(Usuario usuario)
        {
            return new UsuarioResponseDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Telefono = usuario.Telefono,
                Email = usuario.Email,
                UserName = usuario.UserName,
                Token = _jwtGenerador.CrearToken(usuario)
            };
        }
        public async Task<UsuarioResponseDto> GetUsuario()
        {
            /* 
                mediante el objeto
                UserManager<Usuario> _userManager.FindByNameAsync encontrar sessión de usuario mediante userNme;
                IUsuarioSesion _usuarioSesion
             */
            var usuario = await _userManager.FindByNameAsync(_usuarioSesion.ObtenerUsuarioSesion());
            /* si no encontró usuario entonces se crea un MiddlewareException registrando el
                - Código HTTP
                - el mensaje
             */
            if (usuario is null)
            {
                throw new MiddlewareException(
                    HttpStatusCode.Unauthorized,
                    new { mensaje = "El usuario del token no existe en la base de datos" }
                    );
            }
            /* 
                va a transformar el objeto usuario con un objeto UsuarioResponseDto
             */
            return TransformerUsertoUserDto(usuario!);

        }


        /* 
            La funcion se encarga de  validar si el usuario existe o no
         */
        public async Task<UsuarioResponseDto> Login(UsuarioLoginRequestDto usuarioLoginRequestDto)
        {
            /* 
                mediante el objeto
                UserManager<Usuario> _userManager.FindByEmailAsync encontrar usuario mediante email
             */
            var usuario = await _userManager.FindByEmailAsync(usuarioLoginRequestDto.Email!);
            if (usuario is null)
            {
                throw new MiddlewareException(
                    HttpStatusCode.Unauthorized,
                    new { mensaje = "El email del usuario no existe en la base de datos" }
                    );
            }

            /* 
                Si esta correcto el password entonces genera la sesión de usuario
                Task<SignInResult> SignInManager<Usuario>.CheckPasswordSignInAsync(Usuario user, string password, bool lockoutOnFailure)
                - usuario: reciben el usuario
                - password: reciben el password
                - true: la cuenta se bloquea si el usuario se equivoca 2 veces, falso: anula esta función
             */
            var resultado = await _signInManager.CheckPasswordSignInAsync(usuario!, usuarioLoginRequestDto.Password!, false);
            if (resultado.Succeeded)
            {
                return TransformerUsertoUserDto(usuario!);
            }

            throw new MiddlewareException(
                    HttpStatusCode.Unauthorized,
                    new { mensaje = "Las credenciales son incorrectas" }
                    );

        }

        /* 
            La funcion se encarga de registrar el usuario
         */
        public async Task<UsuarioResponseDto> RegistroUsuario(UsuarioRegistroRequestDto usuarioRegistroRequestDto)
        {

            /* 
                encontrar si el email existe
             */
            var existeEmail = await _context.Users.Where(x => x.Email == usuarioRegistroRequestDto.Email).AnyAsync();

            if (existeEmail)
            {
                throw new MiddlewareException(
                    HttpStatusCode.BadRequest,
                    new { mensaje = "El Email del usuario ya existe" }
                    );
            }

            /* 
                encontrar si el email existe
             */
            var existeUsername = await _context.Users.Where(x => x.UserName == usuarioRegistroRequestDto.UserName).AnyAsync();

            if (existeUsername)
            {
                throw new MiddlewareException(
                    HttpStatusCode.BadRequest,
                    new { mensaje = "El usuario ya existe" }
                    );
            }
            var usuario = new Usuario
            {
                Nombre = usuarioRegistroRequestDto.Nombre,
                Apellido = usuarioRegistroRequestDto.Apellido,
                Telefono = usuarioRegistroRequestDto.Telefono,
                Email = usuarioRegistroRequestDto.Email,
                UserName = usuarioRegistroRequestDto.UserName
            };
            /* 
                mediante el objeto
                UserManager<Usuario> _userManager.CreateAsync va a crear el usuario con la contraseña cifrada
             */
            var resultado = await _userManager.CreateAsync(usuario!, usuarioRegistroRequestDto.Password!);
            if (resultado.Succeeded)
            {
                return TransformerUsertoUserDto(usuario);
            }

            throw new Exception("No se pudo registrar el usuario");


        }
    }
}