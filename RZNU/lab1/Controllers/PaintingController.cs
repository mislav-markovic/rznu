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
	public class PaintingController : ControllerBase
	{
		private readonly GalleryDbContext _context;

		public PaintingController(GalleryDbContext context)
		{
			_context = context;
		}

		// GET api/painting
		[SwaggerOperation(Summary = "Gets all paintings.", Description = "Returns list of all paintings.",
			OperationId = "GetPaintings")]
		[SwaggerResponse(200, "Returns all paintings successfully.")]
		[SwaggerResponse(401, "Not logged in.")]
		[HttpGet]
		public ActionResult<IEnumerable<ViewPainting>> Get()
		{
			return _context.Painting.Include(p => p.Author).Select(p => new ViewPainting(p)).ToList();
		}

		// GET api/painting/5
		[SwaggerOperation(Summary = "Gets specific painting",
			Description =
				"Returns specific painting request by giving id, if painting does not exist not found status code is returned.",
			OperationId = "GetPainting")]
		[SwaggerResponse(404, "Painting with requested id does not exist.")]
		[SwaggerResponse(200, "Painting successfully returned.")]
		[SwaggerResponse(401, "Not logged in.")]
		[HttpGet("{id}", Name = "GetPainting")]
		public ActionResult<ViewPainting> Get(int id)
		{
			var result = _context.Painting.Include(p => p.Author)?.FirstOrDefault(p => p.Id == id);
			if (result == null) return NotFound();
			return new ViewPainting(result);
		}

		// GET api/painting/5/author
		[SwaggerOperation(Summary = "Returns artist for specific painting.",
			Description = "Painting with given id must exist", OperationId = "GetPaintingAuthor")]
		[SwaggerResponse(404, "Painting with given id does not exist")]
		[SwaggerResponse(200, "Paintings author returned.")]
		[SwaggerResponse(401, "Not logged in.")]
		[HttpGet("{id}/author", Name = "GetAuthor")]
		public ActionResult<ViewArtist> GetPaintings(int id)
		{
			var result = _context.Painting.Include(p => p.Author).ThenInclude(a => a.Works)
				?.FirstOrDefault(p => p.Id == id);
			if (result == null) return NotFound();
			return new ViewArtist(result.Author);
		}

		// POST api/painting
		[SwaggerOperation(Summary = "Creates new painting.", Description = "Painting must have valid author id",
			OperationId = "PostPainting")]
		[SwaggerResponse(400, "Bad Request is returned when either data is missing or author id does not exist")]
		[SwaggerResponse(201, "Returns newly created painting.")]
		[SwaggerResponse(401, "Not logged in.")]
		[HttpPost]
		public IActionResult Post([FromBody] CreatePainting value)
		{
			var artist = _context.Artist.Find(value.AuthorId);
			if (artist == null) return BadRequest();

			var painting = new PaintingDto(value);
			_context.Painting.Add(painting);
			_context.SaveChanges();

			return CreatedAtRoute("GetPainting", new {id = painting.Id}, new ViewPainting(painting));
		}

		// PUT api/painting/5
		[SwaggerOperation(Summary = "Updates existing painting or creates new one.",
			Description =
				"Both updating and creating require that existing author id is given. When creating new painting id parameter is discarded and new one is generated.",
			OperationId = "PutPainting")]
		[SwaggerResponse(400, "Request with non existing author id is given.")]
		[SwaggerResponse(201, "Returns newly created painting.")]
		[SwaggerResponse(204, "When resource is updated nothing is returned.")]
		[SwaggerResponse(401, "Not logged in.")]
		[HttpPut("{id}")]
		public IActionResult Put(int id, [FromBody] CreatePainting value)
		{
			var find = _context.Painting.Find(id);
			var artist = _context.Artist.Find(value.AuthorId);
			if (find == null)
			{
				if (artist == null) return BadRequest();

				var painting = new PaintingDto(value);
				_context.Painting.Add(painting);
				_context.SaveChanges();

				return CreatedAtRoute("GetPainting", new {id = painting.Id}, new ViewPainting(painting));
			}

			find.Name = value.Name;
			find.YearMade = value.YearMade;
			find.AuthorId = value.AuthorId;
			find.Author = artist;

			_context.Painting.Update(find);
			_context.SaveChanges();

			return NoContent();
		}

		// DELETE api/painting/5
		[SwaggerOperation(Summary = "Deletes painting",
			Description = "Painting must  exist to be deleted, otherwise nothing happens",
			OperationId = "DeletePainting")]
		[SwaggerResponse(204, "Painting is deleted and nothing is returned.")]
		[SwaggerResponse(404, "Painting with requested id does not exist.")]
		[SwaggerResponse(401, "Not logged in.")]
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var find = _context.Painting.Find(id);
			if (find == null) return NotFound();
			_context.Painting.Remove(find);
			_context.SaveChanges();

			return NoContent();
		}
	}
}