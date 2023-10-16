using BoardGames.Api.Models;
using BoardGames.Api.Models.Csv;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BoardGames.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly BoardGamesContext _context;

        private readonly IWebHostEnvironment _environment;

        private readonly ILogger<SeedController> _logger;

        public SeedController(BoardGamesContext context, IWebHostEnvironment env, ILogger<SeedController> logger)
        {
            _context = context;
            _environment = env;
            _logger = logger;
        }

        [HttpPut(Name = "Seed")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Put()
        {
            var config = new CsvConfiguration(CultureInfo.GetCultureInfo("pt-BR"))
            {
                HasHeaderRecord = true,
                Delimiter = ";"
            };

            using var reader = new StreamReader(System.IO.Path.Combine(_environment.ContentRootPath, "Data/bgg_dataset.csv"));

            using var csv = new CsvReader(reader, config);

            var existingBoardGames = await _context.BoardGames
                .ToDictionaryAsync(b => b.Id);

            var existingDomains = await _context.Domains
                .ToDictionaryAsync(d => d.Name);

            var existingMechanics = await _context.Mechanics
                .ToDictionaryAsync(m => m.Name);

            var now = DateTime.Now;

            var records = csv.GetRecords<BggRecord>();
            var skippedRows = 0;

            foreach (var record in records)
            {
                // Check for duplicates or missing information
                if (!record.ID.HasValue
                    || string.IsNullOrEmpty(record.Name)
                    || existingBoardGames.ContainsKey(record.ID.Value))
                {
                    skippedRows++;
                    continue;
                }

                var boardgame = new BoardGame()
                {
                    Id = record.ID.Value,
                    Name = record.Name,
                    BGGRank = record.BGGRank ?? 0,
                    ComplexityAverage = record.ComplexityAverage ?? 0,
                    MaxPlayers = record.MaxPlayers ?? 0,
                    MinAge = record.MinAge ?? 0,
                    MinPlayers = record.MinPlayers ?? 0,
                    OwnedUsers = record.OwnedUsers ?? 0,
                    PlayTime = record.PlayTime ?? 0,
                    RatingAverage = record.RatingAverage ?? 0,
                    UsersRated = record.UsersRated ?? 0,
                    Year = record.YearPublished ?? 0,
                    CreatedDate = now,
                    LastModifiedDate = now
                };

                _context.BoardGames.Add(boardgame);

                if (!string.IsNullOrEmpty(record.Domains))
                {
                    foreach (var domainName in record.Domains
                        .Split(',', StringSplitOptions.TrimEntries)
                        .Distinct(StringComparer.InvariantCultureIgnoreCase))
                    {
                        var domain = existingDomains.GetValueOrDefault(domainName);

                        if (domain == null)
                        {
                            domain = new Domain()
                            {
                                Name = domainName,
                                CreatedDate = now,
                                LastModifiedDate = now
                            };

                            _context.Domains.Add(domain);

                            existingDomains.Add(domainName, domain);
                        }

                        _context.BoardGameDomains.Add(new BoardGameDomain()
                        {
                            BoardGame = boardgame,
                            Domain = domain,
                            CreatedDate = now
                        });
                    }
                }

                if (!string.IsNullOrEmpty(record.Mechanics))
                {
                    foreach (var mechanicName in record.Mechanics
                        .Split(',', StringSplitOptions.TrimEntries)
                        .Distinct(StringComparer.InvariantCultureIgnoreCase))
                    {
                        var mechanic = existingMechanics.GetValueOrDefault(mechanicName);

                        if (mechanic == null)
                        {
                            mechanic = new Mechanic()
                            {
                                Name = mechanicName,
                                CreatedDate = now,
                                LastModifiedDate = now
                            };

                            _context.Mechanics.Add(mechanic);

                            existingMechanics.Add(mechanicName, mechanic);
                        }

                        _context.BoardGameMechanics.Add(new BoardGameMechanic()
                        {
                            BoardGame = boardgame,
                            Mechanic = mechanic,
                            CreatedDate = now
                        });
                    }
                }
            }

            using var transaction = _context.Database.BeginTransaction();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT BoardGame ON");
            await _context.SaveChangesAsync();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT BoardGame OFF");
            transaction.Commit();

            return new JsonResult(new
            {
                BoardGames = _context.BoardGames.Count(),
                Domains = _context.Domains.Count(),
                Mechanics = _context.Mechanics.Count(),
                SkippedRows = skippedRows
            });
        }
    }
}
