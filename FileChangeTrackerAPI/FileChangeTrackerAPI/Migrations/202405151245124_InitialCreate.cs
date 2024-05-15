namespace FileChangeTrackerAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileChanges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileVersionId = c.Int(nullable: false),
                        ChangeDiff = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FileVersions", t => t.FileVersionId, cascadeDelete: true)
                .Index(t => t.FileVersionId);
            
            CreateTable(
                "dbo.FileVersions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileEntryId = c.Int(nullable: false),
                        Version = c.String(),
                        Content = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FileEntries", t => t.FileEntryId, cascadeDelete: true)
                .Index(t => t.FileEntryId);
            
            CreateTable(
                "dbo.FileEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Path = c.String(),
                        Folder = c.String(),
                        CommitHash = c.String(),
                        Metadata = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileChanges", "FileVersionId", "dbo.FileVersions");
            DropForeignKey("dbo.FileVersions", "FileEntryId", "dbo.FileEntries");
            DropIndex("dbo.FileVersions", new[] { "FileEntryId" });
            DropIndex("dbo.FileChanges", new[] { "FileVersionId" });
            DropTable("dbo.FileEntries");
            DropTable("dbo.FileVersions");
            DropTable("dbo.FileChanges");
        }
    }
}
