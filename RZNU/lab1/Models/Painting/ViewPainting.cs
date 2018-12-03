using lab1.Data;
using lab1.Models.Artist;

namespace lab1.Models.Painting
{
	public class ViewPainting
	{
		public ViewPainting(PaintingDto dto)
		{
			Id = dto.Id;
			Name = dto.Name;
			YearMade = dto.YearMade;
			AuthorName = dto.Author.Name;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public int YearMade { get; set; }
		public string AuthorName { get; set; }
	}
}