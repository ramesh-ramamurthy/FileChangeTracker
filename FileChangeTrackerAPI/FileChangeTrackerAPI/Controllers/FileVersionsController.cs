using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using FileChangeTrackerAPI.Models;

namespace FileChangeTrackerAPI.Controllers
{
    public class FileVersionsController : ApiController
    {
        private readonly AppDbContext _context = new AppDbContext();

        [HttpGet]
        [Route("api/fileversions")]
        public IHttpActionResult GetFileVersions()
        {
            var versions = _context.FileVersions
                .Select(fv => new
                {
                    fv.Id,
                    fv.FileEntryId,
                    FileName = fv.FileEntry.Name,
                    fv.Version,
                    fv.Content,
                    fv.CreatedAt
                })
                .ToList();
            return Ok(versions);
        }

        [HttpGet]
        [Route("api/fileversions/{id}")]
        public IHttpActionResult GetFileVersion(int id)
        {
            var version = _context.FileVersions
                .Where(fv => fv.Id == id)
                .Select(fv => new
                {
                    fv.Id,
                    fv.FileEntryId,
                    FileName = fv.FileEntry.Name,
                    fv.Version,
                    fv.Content,
                    fv.CreatedAt
                })
                .FirstOrDefault();
            if (version == null) return NotFound();
            return Ok(version);
        }
    }
}
