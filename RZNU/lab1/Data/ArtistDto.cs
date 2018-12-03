using System.Collections.Generic;
using System.Linq;
using lab1.Models;
using lab1.Models.Artist;

namespace lab1.Data
{
	public class ArtistDto
	{
		public ArtistDto()
		{
			Works = new List<PaintingDto>();
		}

		public ArtistDto(CreateArtist model) : this()
		{
			Name = model.Name;
			YearOfBirth = model.YearOfBirth;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public int YearOfBirth { get; set; }
		public List<PaintingDto> Works { get; set; }
	}
}