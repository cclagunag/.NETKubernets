using NetKubernetes.Dtos.UsuarioDtos;

namespace NetKubernetes.Data.Usuarios
{
    // IUsuarioRepository es una interfaz para crear injección de dependencias mediante una clase que crea
    // la funcionalidad de los métodos
    /* 
        - GetUsuario()
        - Login()
        - RegistroUsuario()
     */
    public interface IUsuarioRepository
    {
        // task crea un método asincrono
        Task<UsuarioResponseDto> GetUsuario();
        Task<UsuarioResponseDto> Login(UsuarioLoginRequestDto usuarioLoginRequestDto);
        Task<UsuarioResponseDto> RegistroUsuario(UsuarioRegistroRequestDto usuarioRegistroRequestDto);
    }
}