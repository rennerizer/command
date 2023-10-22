using System.ComponentModel.DataAnnotations;

namespace BoardGames.Api.DTO
{
    public class BoardGameDTO
    {
        [Required]
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? Year { get; set; }
    }
}
