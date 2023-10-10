namespace BoardGames.Api.Versioning.DTO.v2
{
    public class RestDTO<T>
    {
        public List<DTO.v1.LinkDTO> Links { get; set; } = new List<DTO.v1.LinkDTO>();

        public T Items { get; set; } = default!;
    }
}
