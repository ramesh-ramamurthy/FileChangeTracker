using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileChangeTrackerAPI.Models
{
    public class FileVersion
    {
        public int Id { get; set; }
        public int FileEntryId { get; set; }
        public FileEntry FileEntry { get; set; }
        public string Version { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
