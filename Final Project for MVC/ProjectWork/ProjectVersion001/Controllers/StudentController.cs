using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ProjectVersion001.Models;


namespace ProjectVersion001.Controllers
{
    public class StudentController : Controller
    {
        private UniversityDbContex db = new UniversityDbContex();

        //
        // GET: /Student/

        public ActionResult Index()
        {
            var students = db.Students.Include(s => s.StudentDepartment);
            return View(students.ToList());
        }

        //
        // GET: /Student/Details/5

        public ActionResult Details(int id = 0)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // GET: /Student/Create

        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code");
            return View();
        }

        //
        // POST: /Student/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                Regex phoneFormate = new Regex(@"^([0-9\(\)\/\+ \-]*)$");
                if (phoneFormate.IsMatch(student.Contact))
                {
                    student.RegistrationId = CreateStudentRegistrationNo(student);
                    ViewBag.registrationId = student.Name + " Registration ID: " + student.RegistrationId;
                    db.Students.Add(student);
                    db.SaveChanges();
                    RedirectToAction("Create");
                }
                else
                {
                    TempData["Message"] = "Contact format is not valid";
                    RedirectToAction("Create");
                }
                    
               
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", student.DepartmentId);
            return View(student);
        }

       

        private string CreateStudentRegistrationNo(Student student)
        {
            int id = db.Students.Count(s => (s.DepartmentId == student.DepartmentId)
                    && (s.Date.Year == student.Date.Year)) + 1;
            Department aDepartment = db.Departments.Where(d => d.DepartmentId == student.DepartmentId).FirstOrDefault();
            string registrationId = aDepartment.Code + "-" + student.Date.Year+ "-";

            string addZero ="";
            int len = 3 - id.ToString().Length;
            for (int i = 0; i < len; i++)
            {
                addZero = "0" + addZero;
            }

            return registrationId + addZero + id;
        }

       

        //
        // GET: /Student/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", student.DepartmentId);
            return View(student);
        }

        //
        // POST: /Student/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", student.DepartmentId);
            return View(student);
        }

        //
        // GET: /Student/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // POST: /Student/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public JsonResult CheckEmail(string email)
        {
            var result = db.Students.Count(u => u.Email == email) == 0;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckEmailUnique(string email)
        {
            var result = db.Students.Count(u => u.Email == email) == 0;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}