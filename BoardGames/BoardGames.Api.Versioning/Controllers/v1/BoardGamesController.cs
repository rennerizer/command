﻿using BoardGames.Api.Versioning.DTO.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace BoardGames.Api.Controllers.v1
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class BoardGamesController : ControllerBase
    {

        private readonly ILogger<BoardGamesController> _logger;

        public BoardGamesController(ILogger<BoardGamesController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "BoardGames")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60, NoStore = true)]
        public RestDTO<BoardGame[]> Get()
        {
            return new RestDTO<BoardGame[]>()
            {
                Data = new BoardGame[] {
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
                },
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
