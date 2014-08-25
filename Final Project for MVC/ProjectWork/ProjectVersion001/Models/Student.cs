
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectVersion001.Models
{
    public class Student
    {

        public int StudentId { get; set; }
        [Display(Name = "Student Name")]
        [Required(ErrorMessage = "Student Must Have A Name ")]
        public String Name { get; set; }


        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email Required!")]
        [DataType(DataType.EmailAddress)]
        [Remote("CheckEmailUnique","Student",ErrorMessage = "This Email is already Exist ")]
        public String Email { get; set; }
        [Display(Name = "Contact Number")]
        public String Contact { get; set; }
        [Required(ErrorMessage = "Date Required")]
        [Display(Name = "Date")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Address Must Be Given ")]
        public String Address { get; set; }
        public virtual String RegistrationId { get; set; }

        public int DepartmentId { get; set; }
        public virtual Department StudentDepartment { get; set; }




    }
}