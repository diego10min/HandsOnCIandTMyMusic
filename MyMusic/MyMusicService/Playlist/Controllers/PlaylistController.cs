using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entity;
using EFSecondLevelCache.Core;

namespace Playlist.Controllers
{
    [Produces("application/json")]
    public class PlaylistController : Controller
    {
        private readonly PlaylistsContext _context;

        private readonly MusicasContext _musicasContext;

        private readonly UsuariosContext _usersContext;

        private readonly PlaylistMusicasContext _playlistMusicasContext;

        public PlaylistController(PlaylistsContext context, UsuariosContext userContext, PlaylistMusicasContext playlistMusicasContext, MusicasContext musicasContext)
        {
            _context = context;
            _usersContext = userContext;
            _playlistMusicasContext = playlistMusicasContext;
            _musicasContext = musicasContext;
        }

        /// <summary>
        /// Get Playlist
        /// </summary>
        /// <param name="filtro"></param>
        /// <response code="204"> Usuário não encontrado</response>
        [HttpGet]
        [Route("api/playlist/{filtro}")]
        public IActionResult GetPlaylist([FromRoute] string filtro)
        {
            List<PlaylistMusicas> playlistMusicas = null;
            Playlists playlist = null;

            if (String.IsNullOrEmpty(filtro))
            {
                return NotFound();
            }

            var user = _usersContext.Usuarios.SingleOrDefaultAsync(u => u.Nome == filtro);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                playlist = new Playlists();
                playlist.Id = user.Result.PlaylistId;
                playlist.Usuario = user.Result;

                playlistMusicas = _playlistMusicasContext.PlaylistMusicas
                    .Include(p => p.Musica)
                    .Include(p => p.Musica.Artista)
                    .Where(p => p.PlaylistId == user.Result.PlaylistId)
                    .Cacheable()
                    .ToList();
            }

            if (playlistMusicas != null)
            {
                playlist.PlaylistMusicas = playlistMusicas;
            }

            return Ok(playlist);
        }

        /// <summary>
        /// Add new music in playlist
        /// </summary>
        /// <param name="playlistId"></param>
        /// <param name="musica"></param>
        /// <response code="400">Identificadores não encontrados</response>
        [HttpPut]
        [Route("api/playlist/addmusica/{playlistId}")]
        public async Task<IActionResult> PutPlaylist([FromRoute] string playlistId, [FromBody]Musicas musica)
        {
            if (String.IsNullOrEmpty(playlistId) || musica == null)
            {
                return BadRequest();
            }

            var user = await _usersContext.Usuarios.SingleOrDefaultAsync(u => u.PlaylistId == playlistId);

            if (user == null)
            {
                return BadRequest();
            }
            else
            {
                var musicas = _musicasContext.Musicas.SingleOrDefaultAsync(m => m.Id == musica.Id);

                if (musicas == null || musicas.Result == null)
                {
                    return BadRequest();
                }
                else
                {
                    var playlistMusicaExist = PlaylistMusicaExistsByPlaylistIdMusicaId(user.PlaylistId, musica.Id);

                    if (playlistMusicaExist == null || playlistMusicaExist.Result == null)
                    {
                        PlaylistMusicas playListMusicas = new PlaylistMusicas
                        {
                            MusicaId = musica.Id,
                            PlaylistId = user.PlaylistId
                        };

                        _playlistMusicasContext.Entry(playListMusicas).State = EntityState.Added;

                        try
                        {
                            _playlistMusicasContext.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    }
                }
            }

            return Ok();
        }

        /// <summary>
        /// Delete PlaylistMusica
        /// </summary>
        /// <param name="playlistId"></param>
        /// <param name="musica"></param>
        /// <response code="200">Musica excluida da playlist</response>
        /// <response code="400">Identificadores não encontrados</response>
        [HttpDelete]
        [Route("api/playlist/removemusica/{playlistId}")]
        public async Task<IActionResult> DeletePlaylist([FromRoute] string playlistId, [FromBody]Musicas musica)
        {
            if (String.IsNullOrEmpty(playlistId) || musica == null)
            {
                return BadRequest();
            }

            var user = await _usersContext.Usuarios.SingleOrDefaultAsync(u => u.PlaylistId == playlistId);

            if (user == null)
            {
                return BadRequest();
            }
            else
            {
                var musicas = _musicasContext.Musicas.SingleOrDefaultAsync(m => m.Id == musica.Id);

                if (musicas == null || musicas.Result == null)
                {
                    return BadRequest();
                }
                else
                {
                    var playlistMusicaToRemove = PlaylistMusicaExistsByPlaylistIdMusicaId(user.PlaylistId, musica.Id);

                    if (playlistMusicaToRemove.Result != null)
                    {
                        _playlistMusicasContext.PlaylistMusicas.Remove(playlistMusicaToRemove.Result);

                        _playlistMusicasContext.SaveChanges();
                    }
                }
            }

            return Ok();
        }

        private bool PlaylistExists(string id)
        {
            return _context.Playlist.Any(e => e.Id == id);
        }

        public Task<PlaylistMusicas> PlaylistMusicaExistsByPlaylistIdMusicaId(string playlistId, string musicaId)
        {
            return _playlistMusicasContext.PlaylistMusicas.SingleOrDefaultAsync(e => e.PlaylistId == playlistId && e.MusicaId == musicaId);
        }
    }
}