using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Models
{
    public class ExamContext:DbContext
    {
        public ExamContext() 
        {

        }
        public ExamContext(DbContextOptions<ExamContext> options) : base(options)
        {

        }

        public DbSet<Exam> Exams { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<User> Users { get; set; }
        //public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Result> Results { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Exam>()
            //    .HasKey(p => p.ExamId);
            //modelBuilder.Entity<Answer>()
            //    .HasKey(p => p.AnswerId);
            //modelBuilder.Entity<Document>()
            //    .HasKey(p => p.DocId);
            //modelBuilder.Entity<Question>()
            //    .HasKey(p => p.QuesionId);
            //modelBuilder.Entity<Subject>()
            //    .HasKey(p => p.SubjectId);
            //modelBuilder.Entity<User>()
            //    .HasKey(p => p.UserId);
            //modelBuilder.Entity<UserType>()
            //    .HasKey(p => p.UserTypeId);
            //modelBuilder.Entity<Result>()
            //    .HasKey(p => p.ResultId);


        }

    }
}
