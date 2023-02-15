using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetKubernetes.Data.Usuarios;
using NetKubernetes.Dtos.UsuarioDtos;

namespace NetKubernetes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _repository;
        public UsuarioController(IUsuarioRepository repository)
        {
            this._repository = repository;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioResponseDto>> Login(
            [FromBody] UsuarioLoginRequestDto usuarioLoginRequestDto
            )
        {
            return await _repository.Login(usuarioLoginRequestDto);
        }

        [AllowAnonymous]
        [HttpPost("registrar")]
        public async Task<ActionResult<UsuarioResponseDto>> registrar(
            [FromBody] UsuarioRegistroRequestDto usuarioRegistroRequestDto
            )
        {
            return await _repository.RegistroUsuario(usuarioRegistroRequestDto);
        }

        
        [HttpGet]
        public async Task<ActionResult<UsuarioResponseDto>> DevolverUsuario()
        {
            return await _repository.GetUsuario();
        }
    }
}