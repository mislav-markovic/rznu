using System.Collections.Generic;
using System.Linq;
using lab1.Data;
using lab1.Models;
using lab1.Models.Artist;
using lab1.Models.Painting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace lab1.Controllers
{
	[Authorize]
	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class ArtistController : ControllerBase
	{
		private readonly GalleryDbContext _context;

		public ArtistController(GalleryDbContext context)
		{
			_context = context;
		}

		/// <summary>
		///     Gets all artists.
		/// </summary>
		/// <returns>List of all artists.</returns>
		/// <response code="200">Returns all artists.</response>
		// GET api/artist
		[SwaggerOperation(Summary = "Gets all artists.", Description = "Returns all artists",
			OperationId = "GetArtists")]
		[SwaggerResponse(200, "Returns all artists.")]
		[HttpGet]
		public ActionResult<IEnumerable<ViewArtist>> Get()
		{
			return _context.Artist.Include(a => a.Works).Select(a => new ViewArtist(a)).ToList();
		}

		/// <summary>
		///     Gets artist with specified id.
		/// </summary>
		/// <param name="id">Id of artists.</param>
		/// <returns>Requested artist.</returns>
		/// <response code="200">Returns artist.</response>
		/// <response code="404">Artist with requested ID does not exist.</response>
		// GET api/artist/5
		[SwaggerOperation(Summary = "Gets artist with specified id.", Description = "Returns artist.",
			OperationId = "GetArtist")]
		[SwaggerResponse(404, "Artist with requested ID does not exist.")]
		[SwaggerResponse(200, "Returns artist")]
		[HttpGet("{id}", Name = "GetArtist")]
		public ActionResult<ViewArtist> Get(int id)
		{
			var result = _context.Artist.Include(a => a.Works)?.FirstOrDefault(a => a.Id == id);
			if (result == null) return NotFound();
			return new ViewArtist(result);
		}

		/// <summary>
		///     Gets list of paintings of specific artist.
		/// </summary>
		/// <param name="id">Id of artists.</param>
		/// <returns>Requested artists works.</returns>
		/// <response code="200">Returns list of paintings.</response>
		/// <response code="404">Artist with requested ID does not exist.</response>
		// GET api/artist/5/paintings
		[SwaggerOperation(Summary = "Gets list of paintings of specific artist.",
			Description = "Requested artists works", OperationId = "GetArtistWorks")]
		[SwaggerResponse(200, "Returns list of paintings.")]
		[SwaggerResponse(404, "Artist with requested ID does not exist.")]
		[HttpGet("{id}/paintings", Name = "GetWorks")]
		public ActionResult<IEnumerable<ViewPainting>> GetPaintings(int id)
		{
			var result = _context.Artist.Include(a => a.Works)?.FirstOrDefault(a => a.Id == id);
			if (result == null) return NotFound();
			return result.Works.Select(p => new ViewPainting(p)).ToList();
		}

		/// <summary>
		///     Creates new artist.
		/// </summary>
		/// <param name="value">Data to be used for new artist.</param>
		/// <returns>Newly created artist.</returns>
		/// <response code="201">Sets location header to newly created resource and returns it as json.</response>
		// POST api/artist
		[SwaggerOperation(Summary = "Creates new artist.",
			Description = "Newly created artist. Parameter represents data to be used for new artist.",
			OperationId = "PostArtist")]
		[SwaggerResponse(201, "Sets location header to newly created resource and returns it as json.")]
		[HttpPost]
		public IActionResult Post([FromBody] CreateArtist value)
		{
			var artist = new ArtistDto(value);
			_context.Artist.Add(artist);
			_context.SaveChanges();

			return CreatedAtRoute("GetArtist", new {id = artist.Id}, new ViewArtist(artist));
		}

		/// <summary>
		///     Updates resource with given Id if it exists otherwise it creates new resource (but does not use given Id).
		/// </summary>
		/// <param name="id">Id of object that will be updated.</param>
		/// <param name="value">Value that will be used for updating existing resource or creating new one.</param>
		/// <returns>Nothing if update is performed, otherwise created object is returned.</returns>
		/// <response code="201">Creates new object with given values and returns it.</response>
		/// <response code="204">Existing object was updated with given values and nothing is returned.</response>
		// PUT api/artist/5
		[SwaggerOperation(
			Summary =
				" Updates resource with given Id if it exists otherwise it creates new resource (but does not use given Id).",
			Description =
				"Parameter id is Id of object that will be updated. Parameter value is value that" +
				" will be used for updating existing resource or creating new one." +
				" Returns nothing if update is performed, otherwise created object is returned.",
			OperationId = "PutArtist")]
		[SwaggerResponse(204, "Existing object was updated with given values and nothing is returned.")]
		[SwaggerResponse(201, "Creates new object with given values and returns it.")]
		[HttpPut("{id}")]
		public IActionResult Put(int id, [FromBody] CreateArtist value)
		{
			var find = _context.Artist.Find(id);
			if (find == null)
			{
				var artist = new ArtistDto(value);
				_context.Artist.Add(artist);
				_context.SaveChanges();
				return CreatedAtRoute("GetArtist", new {id = artist.Id}, new ViewArtist(artist));
			}

			find.Name = value.Name;
			find.YearOfBirth = value.YearOfBirth;
			_context.Artist.Update(find);
			_context.SaveChanges();
			return NoContent();
		}

		/// <summary>
		///     Deletes resource with given id.
		/// </summary>
		/// <param name="id">Id of resource that will be deleted.</param>
		/// <returns>Nothing</returns>
		/// <response code="404">Resource with given Id does not exist, nothing happens.</response>
		/// <response code="204">Resource deleted, nothing returned.</response>
		// DELETE api/artist/5
		[SwaggerOperation(Summary = "Deletes resource with given id.",
			Description =
				"Parameter is id of resource that will be deleted. Returns nothing. " +
				"When artist is deleted all paintings that are authored by that artist are also deleted.",
			OperationId = "DeleteArtist")]
		[SwaggerResponse(404, "Resource with given Id does not exist, nothing happens.")]
		[SwaggerResponse(204, "Resource deleted, nothing returned.")]
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var find = _context.Artist.Include(a => a.Works)?.FirstOrDefault(a => a.Id == id);
			if (find == null) return NotFound();
			_context.Artist.Remove(find);
			_context.SaveChanges();

			return NoContent();
		}
	}
}