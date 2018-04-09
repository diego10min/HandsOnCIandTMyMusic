using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Music.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test
{
    public class MusicTest
    {
        public MusicTest()
        {
            InitContext();
        }

        private MusicasContext _musicasContext;

        public void InitContext()
        {
            var builder = new DbContextOptionsBuilder<MusicasContext>();

            var context = new MusicasContext(builder.Options);
            _musicasContext = context;
        }

        [Fact(DisplayName = "GetMusica_Caracter_Menor_Que_3")]
        public void GetMusica_Caracter_Menor_Que_3()
        {
            string filtro = "an";
            var controller = new MusicaController(_musicasContext);
            var resultMusic = controller.GetMusica(filtro);
            Assert.IsType<BadRequestResult>(resultMusic.Result);
        }

        [Fact(DisplayName = "GetMusica")]
        public void GetMusica()
        {
            string filtro = "ana";
            var controller = new MusicaController(_musicasContext);
            var resultMusic = controller.GetMusica(filtro);
            Assert.IsType<OkObjectResult>(resultMusic.Result);
        }
    }
}
