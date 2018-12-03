using System.Collections.Generic;
using System.Linq;
using lab1.Data;
using lab1.Models.Painting;

namespace lab1.Models.Artist
{
	public class ViewArtist
	{
		public ViewArtist()
		{
			Works = new List<ViewPainting>();
		}

		public ViewArtist(ArtistDto dto)
		{
			Id = dto.Id;
			Name = dto.Name;
			YearOfBirth = dto.YearOfBirth;
			Works = dto.Works.Select(p => new ViewPainting(p)).ToList();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public int YearOfBirth { get; set; }
		public List<ViewPainting> Works { get; set; }
	}
}