using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectVersion001.Models;


namespace ProjectVersion001.Controllers
{
    public class ResultEntryController : Controller
    {
        private UniversityDbContex db = new UniversityDbContex();

        //
        // GET: /ResultEntry/

        public ActionResult Index()
        {
            var resultentries = db.ResultEntries.Include(r => r.Student).Include(r => r.Course).Include(r => r.Grade);
            return View(resultentries.ToList());
        }

        //
        // GET: /ResultEntry/Details/5

        public ActionResult Details(int id = 0)
        {
            ResultEntry resultentry = db.ResultEntries.Find(id);
            if (resultentry == null)
            {
                return HttpNotFound();
            }
            return View(resultentry);
        }

        //
        // GET: /ResultEntry/Create

        public ActionResult Create()
        {
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "RegistrationId");
            ViewBag.CourseId = new SelectList("", "CourseId", "Code");
            ViewBag.GradeId = new SelectList(db.Grades, "GradeId", "Name");
            return View();
        }

        //
        // POST: /ResultEntry/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResultEntry resultentry)
        {
            if (ModelState.IsValid)
            {
                var result =
                    db.ResultEntries.Count(
                        r => r.StudentId == resultentry.StudentId && r.CourseId == resultentry.CourseId) == 0;
                if (result)
                {
                    Grade aGrade = db.Grades.Where(g => g.GradeId == resultentry.GradeId).FirstOrDefault();
                    EnrollCourse resultEnrollCourse =
                        db.EnrollCourses.FirstOrDefault(r=>r.CourseId==resultentry.CourseId && r.StudentId==resultentry.StudentId);

                    if (resultEnrollCourse != null) resultEnrollCourse.GradeName = aGrade.Name;

                    db.ResultEntries.Add(resultentry);
                    db.SaveChanges();
                    TempData["success"] = "Result Successfully Entered";
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["Already"] = "Result Of This Course has Already Been Assigned";
                    return RedirectToAction("Create");
                }
            }

            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "RegistrationId", resultentry.StudentId);
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "Code", resultentry.CourseId);
            ViewBag.GradeId = new SelectList(db.Grades, "GradeId", "Name", resultentry.GradeId);
            return View(resultentry);
        }

        public PartialViewResult StudentInfoLoad(int? studentId)
        {
            if (studentId != null)
            {
                Student aStudent = db.Students.FirstOrDefault(s => s.StudentId == studentId);
                ViewBag.Name = aStudent.Name;
                ViewBag.Email = aStudent.Email;
                ViewBag.Dept = aStudent.StudentDepartment.Name;
                return PartialView("~/Views/Shared/_StudentInformation.cshtml");
            }
            else
            {
                return PartialView("~/Views/Shared/_StudentInformation.cshtml");
            }

        }



        //
        // GET: /ResultEntry/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ResultEntry resultentry = db.ResultEntries.Find(id);
            if (resultentry == null)
            {
                return HttpNotFound();
            }
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "RegistrationId", resultentry.StudentId);
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "Code", resultentry.CourseId);
            ViewBag.GradeId = new SelectList(db.Grades, "GradeId", "Name", resultentry.GradeId);
            return View(resultentry);
        }

        //
        // POST: /ResultEntry/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ResultEntry resultentry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(resultentry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "RegistrationId", resultentry.StudentId);
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "Code", resultentry.CourseId);
            ViewBag.GradeId = new SelectList(db.Grades, "GradeId", "Name", resultentry.GradeId);
            return View(resultentry);
        }

        //
        // GET: /ResultEntry/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ResultEntry resultentry = db.ResultEntries.Find(id);
            if (resultentry == null)
            {
                return HttpNotFound();
            }
            return View(resultentry);
        }

        //
        // POST: /ResultEntry/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ResultEntry resultentry = db.ResultEntries.Find(id);
            db.ResultEntries.Remove(resultentry);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public PartialViewResult CourseLoad(int? studentId)
        {
            List<EnrollCourse> enrollmentList = new List<EnrollCourse>();
            List<Course> courseList = new List<Course>();
            if (studentId != null)
            {
                enrollmentList = db.EnrollCourses.Where(e => e.StudentId == studentId).ToList();
                foreach (EnrollCourse anEnrollment in enrollmentList)
                {
                    Course aCourse = db.Courses.FirstOrDefault(c => c.CourseId == anEnrollment.CourseId);
                    courseList.Add(aCourse);
                }
                ViewBag.CourseId = new SelectList(courseList, "CourseId", "Code");
            }
            return PartialView("~/Views/shared/_FilteredCourse.cshtml");
        }

        public ActionResult ViewResult()
        {
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "RegistrationId");
            return View();
        }

        public PartialViewResult ResultInfoLoad(int? studentId)
        {
            
            List<EnrollCourse> enrollCourseList= new List<EnrollCourse>();

            if (studentId != null)
            {
                enrollCourseList = db.EnrollCourses.Where(r => r.StudentId == studentId).ToList();

                if (enrollCourseList.Count == 0)
                {
                    Student aStudent = db.Students.Find(studentId);
                    ViewBag.NotEnrolled = aStudent.RegistrationId + "  : has not taken any course";
                }
            }
            
            return PartialView("~/Views/shared/_resultInformation.cshtml", enrollCourseList);
        }
    }
}