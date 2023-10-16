using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using BoardGames.Api.DTO;
using BoardGames.Api.Models;

namespace BoardGames.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {
        private readonly ILogger<BoardGamesController> _logger;

        private readonly BoardGamesContext _context;

        public BoardGamesController(BoardGamesContext context, ILogger<BoardGamesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet(Name = "BoardGames")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<RestDTO<BoardGame[]>> Get()
        {
            var query = _context.BoardGames;

            return new RestDTO<BoardGame[]>()
            {
                Data = await query.ToArrayAsync(),

                /*Data = new BoardGame[] {
                    new BoardGame
                    {
                        Id = 1,
                        Name = "Axis & Allies",
                        Year = 1981,
                        MinPlayers = 2,
                        MaxPlayers = 5
                    },
                    new BoardGame
                    {
                        Id = 2,
                        Name = "Citadels",
                        Year = 2000,
                        MinPlayers = 2,
                        MaxPlayers = 8
                    },
                    new BoardGame
                    {
                        Id = 3,
                        Name = "Terraforming Mars",
                        Year = 2016,
                        MinPlayers = 1,
                        MaxPlayers= 5
                    }
                },*/

                Links = new List<LinkDTO>{
                    new LinkDTO(
                        Url.Action(null, "BoardGames", null, Request.Scheme)!,
                        "self",
                        "GET"
                    )
                }
            };
        }
    }
}
