using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entity;
using EFSecondLevelCache.Core;

namespace Music.Controllers
{
    [Produces("application/json")]
    public class MusicaController : Controller
    {
        private readonly MusicasContext _context;

        public MusicaController(MusicasContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get Music by Name of Music or Artist
        /// </summary>
        /// <param name="filtro"></param>
        /// <response code="204"> Não há dados</response>
        /// <response code="400"> Caracteres de busca menor que 3</response>
        [HttpGet]
        [Route("api/musica/{filtro}")]
        public async Task<IActionResult> GetMusica([FromRoute] string filtro)
        {
            if (String.IsNullOrEmpty(filtro))
            {
                return BadRequest();
            }

            if (filtro.Length < 3)
            {
                return BadRequest();
            }

            var musicas = _context.Musicas
                .Include(m => m.Artista)
                .Where(m => m.Nome.ToLower().Contains(filtro.ToLower()) || m.Artista.Nome.ToLower().Contains(filtro.ToLower()))
                .Cacheable()
                .ToList()
                .OrderBy(m => m.Artista.Nome)
                .ThenBy(m => m.Nome);

            if (musicas == null)
            {
                return NoContent();
            }

            return Ok(musicas);
        }
    }
}