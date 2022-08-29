using AutoMapper;
using BackEndGeneros.DTOs;
using BackEndGeneros.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BackEndGeneros.Controllers
{
    [Route("api/actores")]
    [ApiController]
    public class ActoresController : ControllerBase
    {
        private readonly ILogger<ActoresController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ActoresController(
            ILogger<ActoresController> logger,
            ApplicationDbContext context,
            IMapper mapper)
        {
            this._logger = logger;
            this._context = context;
            this._mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ActorCreacionDTO actorCreacionDTO)
        {
            var actor = _mapper.Map<Actor>(actorCreacionDTO);
            _context.Add(actor);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
