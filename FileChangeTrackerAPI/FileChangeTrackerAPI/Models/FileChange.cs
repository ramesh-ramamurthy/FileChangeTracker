using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileChangeTrackerAPI.Models
{
    public class FileChange
    {
        public int Id { get; set; }
        public int FileVersionId { get; set; }
        public FileVersion FileVersion { get; set; }
        public string ChangeDiff { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
