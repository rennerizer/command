using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;

using BoardGames.Api.DTO;
using BoardGames.Api.Models;
using BoardGames.Api.Attributes;

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
        public async Task<RestDTO<BoardGame[]>> Get([FromQuery] RequestDTO<BoardGameDTO> request)
        {
            var query = _context.BoardGames.AsQueryable();

            if (!string.IsNullOrEmpty(request.FilterQuery))
                query = query.Where(b => b.Name.Contains(request.FilterQuery));

            var recordCount = await query.CountAsync();

            query = query
                .OrderBy($"{request.SortColumn} {request.SortOrder}")
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize);

            return new RestDTO<BoardGame[]>()
            {
                Data = await query.ToArrayAsync(),

                PageIndex = request.PageIndex,
                
                PageSize = request.PageSize,

                RecordCount = recordCount,

                Links = new List<LinkDTO>{
                    new LinkDTO(
                        Url.Action(null, "BoardGames", new { request.PageIndex, request.PageSize}, Request.Scheme)!,
                        "self",
                        "GET"
                    )
                }
            };
        }

        [HttpPost(Name = "UpdateBoardGame")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<BoardGame?>> Post(BoardGameDTO model)
        {
            var boardgame = await _context.BoardGames
                .Where(bg => bg.Id == model.Id)
                .FirstOrDefaultAsync();

            if (boardgame != null)
            {
                if (!string.IsNullOrEmpty(model.Name))
                    boardgame.Name = model.Name;

                if (model.Year.HasValue && model.Year.Value > 0)
                    boardgame.Year = model.Year.Value;

                boardgame.LastModifiedDate = DateTime.Now;

                _context.BoardGames.Update(boardgame);

                await _context.SaveChangesAsync();
            }

            return new RestDTO<BoardGame?>()
            {
                Data = boardgame,
                Links = new List<LinkDTO>
                {
                    new LinkDTO(
                        Url.Action(
                            null,
                            "BoardGames",
                            model,
                            Request.Scheme)!,
                        "self",
                        "POST")
                }
            };
        }

        [HttpDelete(Name = "DeleteBoardGame")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<BoardGame?>> Delete(int id)
        {
            var boardgame = await _context.BoardGames
                .Where(bg => bg.Id == id)
                .FirstOrDefaultAsync();

            if (boardgame != null)
            {
                _context.BoardGames.Remove(boardgame);

                await _context.SaveChangesAsync();
            }

            return new RestDTO<BoardGame?>()
            {
                Data = boardgame,
                Links = new List<LinkDTO>
                {
                    new LinkDTO(
                        Url.Action(
                            null,
                            "BoardGames",
                            id,
                            Request.Scheme)!,
                        "self",
                        "DELETE")
                }
            };
        }
    }
}
