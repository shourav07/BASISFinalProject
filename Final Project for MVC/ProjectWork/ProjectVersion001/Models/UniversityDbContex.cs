using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;


namespace ProjectVersion001.Models
{
    public class UniversityDbContex:DbContext

    {
        private DropCreateDatabaseIfModelChanges<UniversityDbContex> CreateDatabaseIfModelChanges; 

       public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Semister> Semisters { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<AssignCourse> AssignCourses { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomAllocation> RoomAllocations { get; set; }
        public DbSet<Day> Days { get; set; }

        public DbSet<EnrollCourse> EnrollCourses { get; set; }
        public DbSet<ResultEntry> ResultEntries { get; set; }
        public DbSet<Grade> Grades { set; get; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
             
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }

      
}
}