using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileChangeTrackerAPI.Models
{
    public class FileEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Folder { get; set; }
        public string CommitHash { get; set; }
        public string Metadata { get; set; }
    }
}
