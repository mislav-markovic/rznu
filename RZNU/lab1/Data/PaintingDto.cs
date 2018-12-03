using lab1.Models.Painting;

namespace lab1.Data
{
	public class PaintingDto
	{
		public PaintingDto()
		{
			Author = new ArtistDto();
		}

		public PaintingDto(CreatePainting model) : this()
		{
			Name = model.Name;
			YearMade = model.YearMade;
			AuthorId = model.AuthorId;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public int YearMade { get; set; }
		public int AuthorId { get; set; }
		public ArtistDto Author { get; set; }
	}
}