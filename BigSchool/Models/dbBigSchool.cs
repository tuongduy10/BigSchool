namespace BigSchool.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class dbBigSchool : DbContext
    {
        public dbBigSchool()
            : base("name=dbBigSchool")
        {
        }

        public virtual DbSet<Attendance> Attendances { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Following> Followings { get; set; }
            
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(e => e.Courses)
                .WithRequired(e => e.Category)
                .WillCascadeOnDelete(false);
        }
    }
}
