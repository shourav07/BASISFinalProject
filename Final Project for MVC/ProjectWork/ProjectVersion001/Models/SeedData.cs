using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProjectVersion001.Models
{
    public class SeedData : DropCreateDatabaseIfModelChanges<UniversityDbContex>
    {
        protected override void Seed(UniversityDbContex context)
        {

            //Semisters
            context.Semisters.Add(new Semister() {Semistername = "Fall 2013"});
            context.Semisters.Add(new Semister() { Semistername = "summer 2013" });
            context.Semisters.Add(new Semister() { Semistername = "Spring 2013" });
            context.Semisters.Add(new Semister() { Semistername = "Fall 2014" });
            context.Semisters.Add(new Semister() { Semistername = "Summer 2014" });
            context.Semisters.Add(new Semister() { Semistername = "spring 2014" });
            context.Semisters.Add(new Semister() { Semistername = "Fall 2015" });
            context.Semisters.Add(new Semister() { Semistername = "Summer 2015" });



            //Designation
            context.Designations.Add(new Designation(){DesignationName = "Professor"});
            context.Designations.Add(new Designation() { DesignationName = "asst. Professor" });
            context.Designations.Add(new Designation() { DesignationName = "Lecturer" });
            context.Designations.Add(new Designation() { DesignationName = "Asst Lecturer" });

            //Department
            context.Departments.Add(new Department(){Code = "CSE",Name = "Computer Science"});


            //room
            context.Rooms.Add(new Room(){Name = "A 501"});
            context.Rooms.Add(new Room(){Name = "A 502"});
            context.Rooms.Add(new Room(){Name = "A 505"});
            context.Rooms.Add(new Room(){Name = "B 601"});
            context.Rooms.Add(new Room(){Name = "B 609"});
            context.Rooms.Add(new Room(){Name = "MCL A"});


            //Day

            context.Days.Add(new Day() { Name = "SaturDay" });
            context.Days.Add(new Day() { Name = "SunDay" });
            context.Days.Add(new Day() { Name = "MonDay" });
            context.Days.Add(new Day() { Name = "TuesDay" });
            context.Days.Add(new Day() { Name = "WednesDay" });
            context.Days.Add(new Day() { Name = "ThursDay" });
            context.Days.Add(new Day() { Name = "Friday" });

            //grade
            context.Grades.Add(new Grade() { Name = "A+" });
            context.Grades.Add(new Grade() { Name = "A" });
            context.Grades.Add(new Grade() { Name = "B+" });
            context.Grades.Add(new Grade() { Name = "B" });
            context.Grades.Add(new Grade() { Name = "C+" });
            context.Grades.Add(new Grade() { Name = "C" });
            context.Grades.Add(new Grade() { Name = "D" });
            context.Grades.Add(new Grade() { Name = "F" });

            
            


            context.SaveChanges();



        }
    }
}