using Entity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Playlist;
using Playlist.Controllers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Test
{
    public class PlaylistTest
    {
        public PlaylistTest()
        {
            InitContext();
        }
        
        private PlaylistsContext _playlistsContext;
        private MusicasContext _musicasContext;
        private UsuariosContext _usersContext;
        private PlaylistMusicasContext _playlistMusicasContext;

        public void InitContext()
        {
            var builder = new DbContextOptionsBuilder<PlaylistsContext>();
            var context = new PlaylistsContext(builder.Options);
            _playlistsContext = context;

            var builderMusica = new DbContextOptionsBuilder<MusicasContext>();
            var contextMusica = new MusicasContext(builderMusica.Options);
            _musicasContext = contextMusica;

            var builderUsuario = new DbContextOptionsBuilder<UsuariosContext>();
            var contextUsuario = new UsuariosContext(builderUsuario.Options);
            _usersContext = contextUsuario;

            var builderPlaylistMusicas = new DbContextOptionsBuilder<PlaylistMusicasContext>();
            var contextPlaylistMusicas = new PlaylistMusicasContext(builderPlaylistMusicas.Options);
            _playlistMusicasContext = contextPlaylistMusicas;
        }

        [Fact(DisplayName = "GetPlaylist")]
        public void GetPlaylist()
        {
            string filtro = "ana";
            var controller = new PlaylistController(_playlistsContext, _usersContext, _playlistMusicasContext, _musicasContext);
            var resulPlaylist = controller.GetPlaylist(filtro);
            Assert.IsType<OkObjectResult>(resulPlaylist);
        }

        [Fact(DisplayName = "GetPlaylist_Login_Invalido")]
        public void GetPlaylist_Login_Invalido()
        {
            string filtro = "bla";
            var controller = new PlaylistController(_playlistsContext, _usersContext, _playlistMusicasContext, _musicasContext);
            var resulPlaylist = controller.GetPlaylist(filtro);
            Assert.IsType<NotFoundResult>(resulPlaylist);
        }

        [Fact(DisplayName = "GetPlaylist_Login_Valido")]
        public void GetPlaylist_Login_Valido()
        {
            string filtro = "ana";
            var controller = new PlaylistController(_playlistsContext, _usersContext, _playlistMusicasContext, _musicasContext);
            var resultPlaylist = controller.GetPlaylist(filtro);
            Assert.IsType<OkObjectResult>(resultPlaylist);
        }

        [Fact(DisplayName = "PutPlaylist_Identificador_PlaylistId_Invalido")]
        public void PutPlaylist_Identificador_PlaylistId_Invalido()
        {
            string playlistId = "teste";
            var controller = new PlaylistController(_playlistsContext, _usersContext, _playlistMusicasContext, _musicasContext);
            var resultPlaylist = controller.PutPlaylist(playlistId, null);
            Assert.IsType<BadRequestResult>(resultPlaylist.Result);
        }

        [Fact(DisplayName = "PutPlaylist_Identificador_MusicaId_Invalido")]
        public void PutPlaylist_Identificador_MusicaId_Invalido()
        {
            string playlistId = "e643958a-f388-4c0c-ab90-787336a61ae1";
            Musicas musica = new Musicas
            {
                Id = "teste_errado"
            };
            var controller = new PlaylistController(_playlistsContext, _usersContext, _playlistMusicasContext, _musicasContext);
            var resultPlaylist = controller.PutPlaylist(playlistId, musica);
            Assert.IsType<BadRequestResult>(resultPlaylist.Result);
        }

        [Fact(DisplayName = "PutPlaylist")]
        public void PutPlaylist()
        {
            string playlistId = "e643958a-f388-4c0c-ab90-787336a61ae1";
            string musicaId = "283e6fc6-121c-4f84-bce8-29b94b734456";

            var controller = new PlaylistController(_playlistsContext, _usersContext, _playlistMusicasContext, _musicasContext);

            var playlistMusicaExist = controller.PlaylistMusicaExistsByPlaylistIdMusicaId(playlistId, musicaId);

            if (playlistMusicaExist != null && playlistMusicaExist.Result != null)
            {
                _playlistMusicasContext.PlaylistMusicas.Remove(playlistMusicaExist.Result);

                _playlistMusicasContext.SaveChanges();
            }

            Musicas musica = new Musicas
            {
                Id = musicaId
            };
            
            var resultPlaylist = controller.PutPlaylist(playlistId, musica);
            Assert.IsType<OkResult>(resultPlaylist.Result);
        }

        [Fact(DisplayName = "DeletePlaylist_Identificador_PlaylistId_Invalido")]
        public void DeletePlaylist_Identificador_PlaylistId_Invalido()
        {
            string playlistId = "teste";
            var controller = new PlaylistController(_playlistsContext, _usersContext, _playlistMusicasContext, _musicasContext);
            var resultPlaylist = controller.DeletePlaylist(playlistId, null);
            Assert.IsType<BadRequestResult>(resultPlaylist.Result);
        }

        [Fact(DisplayName = "DeletePlaylist_Identificador_MusicaId_Invalido")]
        public void DeletePlaylist_Identificador_MusicaId_Invalido()
        {
            string playlistId = "e643958a-f388-4c0c-ab90-787336a61ae1";
            Musicas musica = new Musicas
            {
                Id = "teste_errado"
            };
            var controller = new PlaylistController(_playlistsContext, _usersContext, _playlistMusicasContext, _musicasContext);
            var resultPlaylist = controller.DeletePlaylist(playlistId, musica);
            Assert.IsType<BadRequestResult>(resultPlaylist.Result);
        }

        [Fact(DisplayName = "DeletePlaylist")]
        public void DeletePlaylist()
        {
            string playlistId = "e643958a-f388-4c0c-ab90-787336a61ae1";
            string musicaId = "283e6fc6-121c-4f84-bce8-29b94b734456";

            Musicas musica = new Musicas
            {
                Id = musicaId
            };

            var controller = new PlaylistController(_playlistsContext, _usersContext, _playlistMusicasContext, _musicasContext);

            var playlistMusicaExist = controller.PlaylistMusicaExistsByPlaylistIdMusicaId(playlistId, musicaId);

            if (playlistMusicaExist == null || playlistMusicaExist.Result == null)
            {

                PlaylistMusicas playListMusicas = new PlaylistMusicas
                {
                    MusicaId = musicaId,
                    PlaylistId = playlistId
                };

                _playlistMusicasContext.Entry(playListMusicas).State = EntityState.Added;

                try
                {
                    _playlistMusicasContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }
            }

            var resultPlaylist = controller.DeletePlaylist(playlistId, musica);
            Assert.IsType<OkResult>(resultPlaylist.Result);
        }
    }
}
