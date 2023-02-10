using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NetKubernetes.Data.Inmuebles;
using NetKubernetes.Dtos.InmueblesDtos;
using NetKubernetes.Middleware;
using NetKubernetes.Models;

namespace NetKubernetes.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InmuebleController : Controller
    {
        private readonly IInmuebleRepository _repository;
        private IMapper _mapper;
        public InmuebleController(IInmuebleRepository repository, IMapper mapper)
        {
            this._mapper = mapper;
            this._repository = repository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InmuebleResponseDto>>> GetInmuebles()
        {
            var inmuebbles = await _repository.GetAllInmuebles();
            return Ok(_mapper.Map<IEnumerable<InmuebleResponseDto>>(inmuebbles));


        }

        [HttpGet("{id}", Name = "GetInmuebleById")]
        public async Task<ActionResult<InmuebleResponseDto>> GetInmuebleById(int id)
        {
            var inmueble = await _repository.GetInmuebleById(id);
            if(inmueble is null){
                throw new MiddlewareException(
                    HttpStatusCode.NotFound,
                    new {mensaje = $"No se encontró inmueble por ese id {id}"}
                );
            }

            return Ok(_mapper.Map<InmuebleResponseDto>(inmueble));
        }

        [HttpPost]
        public async Task<ActionResult<InmuebleResponseDto>> CreateInmueble([FromBody] InmuebleRequestDto inmueble)
        {
            var inmuebleModel = _mapper.Map<Inmueble>(inmueble);
            await _repository.CreateInmueble(inmuebleModel);
            await _repository.SaveChanges();

            var inmuebleResponse = _mapper.Map<InmuebleResponseDto>(inmuebleModel);
            return CreatedAtRoute(nameof(GetInmuebleById), new {inmuebleModel.Id}, inmuebleResponse);
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInmueble(int id)
        {
            await _repository.DeleteInmueble(id);
            await _repository.SaveChanges();
            return Ok();
        }
    }
}