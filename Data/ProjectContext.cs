using Microsoft.EntityFrameworkCore;
using Esquire.Models;

namespace Esquire.Data
{
    public class ProjectContext:DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options) { }
        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<Jurisdiction> Jurisdictions { get; set; }
        public DbSet<ProjectJurisdiction> ProjectJurisdictions { get; set; }
        public DbSet<Scheme> Schemes { get; set; }
        public DbSet<SchemeQuestion> SchemeQuestions { get; set; }
        public DbSet<SchemeAnswer> SchemeAnswers { get; set; }
        public DbSet<CodedQuestionBase> CodedQuestionBases { get; set; }
        public DbSet<CodedQuestion> CodedQuestions { get; set; }
        public DbSet<CodedCategoryQuestion> CodedCategoryQuestions { get; set; }
        public DbSet<ValidatedQuestionBase> ValidatedQuestionBases { get; set; }
        public DbSet<ValidatedQuestion> ValidatedQuestions { get; set; }
        public DbSet<ValidatedCategoryQuestion> ValidatedCategoryQuestions { get; set; }
        public DbSet<CodedAnswer> CodedAnswers { get; set; }
        public DbSet<Protocol> Protocols { get; set; }
        public DbSet<FlagBase> FlagBases { get; set; }
        public DbSet<CodedQuestionFlag> CodedQuestionFlags { get; set; }
        public DbSet<SchemeQuestionFlag> SchemeQuestionFlags { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.Name)
                .IsUnique();
            modelBuilder.Entity<Project>()
                .Property(p => p.Status)
                .HasDefaultValue(1);
            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<ProjectUser>()
                .HasKey(u => u.RowId);
              
            modelBuilder.Entity<Jurisdiction>()
                .HasIndex(j => j.GnisFipsConcatenation)
                .IsUnique();
            modelBuilder.Entity<Jurisdiction>()
                .HasIndex(j => j.Name)
                .HasName("IX_JurisdictionName");
               
            modelBuilder.Entity<CodedQuestion>()
                .HasIndex("CodedById", "ProjectJurisdictionId", "SchemeQuestionId")
                .IsUnique();
            
            modelBuilder.Entity<CodedCategoryQuestion>()
                .HasIndex("CategoryId", "CodedById", "ProjectJurisdictionId", "SchemeQuestionId")
                .IsUnique();
            
            modelBuilder.Entity<ValidatedQuestion>()
                .HasIndex("ProjectJurisdictionId", "SchemeQuestionId")
                .IsUnique();
            
            modelBuilder.Entity<ValidatedCategoryQuestion>()
                .HasIndex("CategoryId", "ProjectJurisdictionId", "SchemeQuestionId")
                .IsUnique();

            modelBuilder.Entity<CodedAnswer>()
                .HasIndex("CodedQuestionBaseId", "SchemeAnswerId")
                .IsUnique();
            modelBuilder.Entity<CodedAnswer>()
                .Property(ca => ca.Annotations)
                .HasDefaultValue("[]");
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
