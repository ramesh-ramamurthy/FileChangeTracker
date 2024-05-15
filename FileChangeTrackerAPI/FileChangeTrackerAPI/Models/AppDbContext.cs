using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;

namespace FileChangeTrackerAPI.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<FileEntry> Files { get; set; }
        public DbSet<FileVersion> FileVersions { get; set; }
        public DbSet<FileChange> FileChanges { get; set; }

        public AppDbContext() : base("name=DefaultConnection")
        {
        }
    }
}
