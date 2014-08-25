using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectVersion001.Models
{
    public class Teacher
    {
        public Teacher()
        {
            this.TeachersCourses = new Collection<Course>();
        }
        public int TeacherId { get; set; }
        [Required(ErrorMessage = "Field must be given")]
        [Display(Name = "Teacher Name")]
        public String Name { get; set; }
        public String Address { get; set; }
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Must be a valid Email Address")]
        [Remote("CheckEmailUniqe","Teacher",ErrorMessage = "This Email Address Already Registred!!!")]
        public String Email { get; set; }
         [Required(ErrorMessage = "Field must be given")]
        public String Contact { get; set; }
         [Required(ErrorMessage = "Field must be given")]
        public int DesignationId { get; set; }
         [Required(ErrorMessage = "Field must be given")]
         [Display(Name = "Course Credit")]
         [Range(typeof(double), "0.00", "2147483647.00", ErrorMessage = "Credit Should be a positive number")]
        public Double CreditTaken { get; set; }

       // public float TotalCradit { get; set; }

        public int DepartmentId { get; set; }
        public virtual Department TeacherDepartment { get; set; }

        public virtual Designation TeachersDesignations  { get; set; }

        public virtual ICollection<Course> TeachersCourses { get; set; }




    }
}