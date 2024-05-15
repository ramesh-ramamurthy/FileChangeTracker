using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using FileChangeTrackerAPI.Models;

namespace FileChangeTrackerAPI.Controllers
{
    public class ChangesController : ApiController
    {
        private readonly AppDbContext _context = new AppDbContext();

        [HttpGet]
        [Route("api/changes")]
        public IHttpActionResult GetChanges()
        {
            var changes = _context.FileChanges
                .Select(fc => new
                {
                    fc.Id,
                    FileName = fc.FileVersion.FileEntry.Name,
                    fc.ChangeDiff,
                    fc.CreatedAt
                })
                .ToList();
            return Ok(changes);
        }

        [HttpGet]
        [Route("api/changes/{id}")]
        public IHttpActionResult GetChange(int id)
        {
            var change = _context.FileChanges
                .Where(fc => fc.Id == id)
                .Select(fc => new
                {
                    fc.Id,
                    FileName = fc.FileVersion.FileEntry.Name,
                    fc.ChangeDiff,
                    fc.CreatedAt
                })
                .FirstOrDefault();
            if (change == null) return NotFound();
            return Ok(change);
        }
    }
}
