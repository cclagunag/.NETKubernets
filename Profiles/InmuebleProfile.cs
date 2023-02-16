using AutoMapper;
using NetKubernetes.Dtos.InmueblesDtos;
using NetKubernetes.Models;

namespace NetKubernetes.Profiles
{
    public class InmuebleProfile: Profile
    {
        public InmuebleProfile()
        {
            // crear mapper para transformar informacion del objeto Inmueble a InmuebleResponseDto
            CreateMap<Inmueble, InmuebleResponseDto>();
            // crear mapper para transformar informacion del objeto InmuebleRequestDto a Inmueble
            CreateMap<InmuebleRequestDto, Inmueble>();
        }
    }
}