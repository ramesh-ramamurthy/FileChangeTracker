using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json.Linq;
using FileChangeTrackerAPI.Models;
using System.Timers;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

public class FileWatcherService
{
    private readonly Timer _timer;
    private readonly AppDbContext _context;

    public FileWatcherService()
    {
        _context = new AppDbContext();
        _timer = new Timer(86400000); // 24 hours in milliseconds
        _timer.Elapsed += OnTimedEvent;
    }

    public void Start()
    {
        _timer.Start();
        CheckFolders();
    }

    private void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        CheckFolders();
    }

    private void CheckFolders()
    {
        string[] folders = { "C:\\Users\\2rame\\source\\repos\\contexture\\FileChangeTrackerAPI\\test\\dev", "cert", "prod" };

        foreach (var folder in folders)
        {
            if (HasFolderChanged(folder))
            {
                ProcessFolder(folder);
            }
        }
    }

    private bool HasFolderChanged(string folder)
    {
        if ( ! Directory.Exists(folder) )
        {
            return false;
        }
        string commitFilePath = Path.Combine(folder, "lastcommit.txt");
        
        string lastCommit = File.ReadAllText(commitFilePath).Trim();

        var existingFile = _context.Files.FirstOrDefault(f => f.Folder == folder);
        if (existingFile != null && existingFile.CommitHash == lastCommit)
        {
            return false;
        }

        if (existingFile != null)
        {
            existingFile.CommitHash = lastCommit;
        }
        else
        {
            _context.Files.Add(new FileEntry
            {
                Folder = folder,
                CommitHash = lastCommit
            });
        }

        _context.SaveChanges();
        return true;
    }

    private void ProcessFolder(string folder)
    {
        string[] files = Directory.GetFiles(folder);

        foreach (var file in files)
        {
            if (Path.GetFileName(file) != "lastcommit.txt")
            {
                string content = File.ReadAllText(file);
                string metadata = ExtractMetadata(folder);
                var fileEntry = _context.Files.FirstOrDefault(f => f.Path == file);

                if (fileEntry == null)
                {
                    fileEntry = new FileEntry
                    {
                        Name = Path.GetFileName(file),
                        Path = file,
                        Folder = folder,
                        Metadata = metadata
                    };
                    _context.Files.Add(fileEntry);
                }

                var fileVersion = new FileVersion
                {
                    FileEntryId = fileEntry.Id,
                    Content = content,
                    CreatedAt = DateTime.Now
                };

                _context.FileVersions.Add(fileVersion);
                _context.SaveChanges();

                if (_context.FileVersions.Count(fv => fv.FileEntryId == fileEntry.Id) > 1)
                {
                    TrackChanges(fileEntry, fileVersion);
                }
            }
        }
    }

    private string ExtractMetadata(string folder)
    {
        string metadataPath = Path.Combine(folder, "config.json");
        string metadataContent = File.ReadAllText(metadataPath);
        JObject metadata = JObject.Parse(metadataContent);
        return metadata.ToString();
    }

    private void TrackChanges(FileEntry fileEntry, FileVersion newVersion)
    {
        var oldVersion = _context.FileVersions
            .Where(fv => fv.FileEntryId == fileEntry.Id)
            .OrderByDescending(fv => fv.CreatedAt)
            .Skip(1)
            .FirstOrDefault();

        if (oldVersion != null)
        {
            var diff = CompareFiles(oldVersion.Content, newVersion.Content);

            _context.FileChanges.Add(new FileChange
            {
                FileVersionId = newVersion.Id,
                ChangeDiff = diff,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();
        }
    }

    private string CompareFiles(string oldContent, string newContent)
    {
        var diffBuilder = new InlineDiffBuilder(new Differ());
        var diff = diffBuilder.BuildDiffModel(oldContent, newContent);
        string result = "";

        foreach (var line in diff.Lines)
        {
            switch (line.Type)
            {
                case ChangeType.Inserted:
                    result += $"+ {line.Text}\n";
                    break;
                case ChangeType.Deleted:
                    result += $"- {line.Text}\n";
                    break;
                case ChangeType.Modified:
                    result += $"~ {line.Text}\n";
                    break;
                default:
                    result += $"  {line.Text}\n";
                    break;
            }
        }

        return result;
    }
}

