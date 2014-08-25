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
    public class AssignCourseController : Controller
    {
        private UniversityDbContex db = new UniversityDbContex();

        //
        // GET: /AssignCourse/

        public ActionResult Index()
        {
            var assigncourses = db.AssignCourses.Include(a => a.Department).Include(a => a.Teacher).Include(a => a.Course);
            return View(assigncourses.ToList());
        }

        //
        // GET: /AssignCourse/Details/5

        public ActionResult Details(int id = 0)
        {
            AssignCourse assigncourse = db.AssignCourses.Find(id);
            if (assigncourse == null)
            {
                return HttpNotFound();
            }
            return View(assigncourse);
        }

        //
        // GET: /AssignCourse/Create

        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code");
            ViewBag.TeacherId = new SelectList("", "TeacherId", "Name");
            ViewBag.CourseId = new SelectList("", "CourseId", "Code");
            return View();
        }

        //
        // POST: /AssignCourse/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AssignCourse assigncourse)
        {
            if (ModelState.IsValid)
            {
                var result =
                    db.AssignCourses.Count(
                        r => r.CourseId == assigncourse.CourseId ) == 0;

                if (result)
                {
                    Teacher aTeacher = db.Teachers.FirstOrDefault(t => t.TeacherId == assigncourse.TeacherId);
                    Course aCourse = db.Courses.FirstOrDefault(c => c.CourseId == assigncourse.CourseId);
                    List<AssignCourse> assignTeachers =
                        db.AssignCourses.Where(t => t.TeacherId == assigncourse.TeacherId).ToList();
                    AssignCourse assign = null;
                    if (assignTeachers.Count != 0)
                    {
                        
                        assign = assignTeachers.Last();
                        assigncourse.RemaingCredit = assign.RemaingCredit;
                    }
                    else
                    {
                        assigncourse.RemaingCredit = aTeacher.CreditTaken;
                    }
                    
                    //if (assign == null)
                    //    assigncourse.RemaingCredit = aTeacher.CreditTaken;
                    
                    if (assigncourse.RemaingCredit < aCourse.Credit)
                    {
                        Session["Teacher"] = aTeacher;
                        Session["Course"] = aCourse;
                        Session["AssignedCourse"] = assigncourse;
                        Session["AssigneddCourseCheck"] = assign;
                        return RedirectToAction("AskToAssign");
                    }

                    assigncourse.CreditTaken = aTeacher.CreditTaken;

                    if (assign == null)
                    {
                        assigncourse.RemaingCredit = aTeacher.CreditTaken - aCourse.Credit;
                    }
                    else
                    {
                        assigncourse.RemaingCredit = assign.RemaingCredit - aCourse.Credit;
                    }

                    aCourse.AssignTo = aTeacher.Name;

                    db.AssignCourses.Add(assigncourse);
                    db.SaveChanges();
                    TempData["success"] = "Course Assigned Successfully";
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["Already"] = "Course Has Already Been Assigned";
                    return RedirectToAction("Create");
                }
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", assigncourse.DepartmentId);
            ViewBag.TeacherId = new SelectList(db.Teachers, "TeacherId", "Name", assigncourse.TeacherId);
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "Code", assigncourse.CourseId);
            return View(assigncourse);
        }

        public ActionResult AskToAssign()
        {
            Teacher aTeacher = (Teacher)Session["Teacher"];
            Course aCourse = (Course)Session["Course"];
            AssignCourse assign = (AssignCourse) Session["AssigneddCourseCheck"];
            double remainingCredite =0.0;
            if (assign == null)
                remainingCredite = aTeacher.CreditTaken;
            else
            {
                remainingCredite = assign.RemaingCredit;
            }
            if (remainingCredite < 0)
            {
                ViewBag.Message = aTeacher.Name
                + " Credit Limit Is Over. And The Course Credit  : " + aCourse.Code
                + " Is " + aCourse.Credit
                + "  ! Still You Want To Assign This Course To This Teacher ?";
            }
            else
            {
                ViewBag.Message = aTeacher.Name
                + " has only " + remainingCredite
                + " Credits Remaining . But, The Credit  : " + aCourse.Code
                + " Is " + aCourse.Credit
                + "  ! Still You Want To Assign This Course To This Teacher ?";
            }
            
            return View();
        }

        public ActionResult AssignConfirmed()
        {
            Teacher aTeacher = (Teacher)Session["Teacher"];
            
            AssignCourse assigncourse = (AssignCourse)Session["AssignedCourse"];
            AssignCourse assign = (AssignCourse)Session["AssigneddCourseCheck"];
            Course aCourse = db.Courses.FirstOrDefault(c => c.CourseId == assigncourse.CourseId);
           

            assigncourse.CreditTaken = aTeacher.CreditTaken;
            if (assign == null)
            {
                assigncourse.RemaingCredit = aTeacher.CreditTaken - aCourse.Credit;
            }
            else
            {
                assigncourse.RemaingCredit = assign.RemaingCredit - aCourse.Credit;
            }

            aCourse.AssignTo = aTeacher.Name;

            db.AssignCourses.Add(assigncourse);
            db.SaveChanges();
            TempData["success"] = "Course Is Assigned";
            return View();
        }

        //
        // GET: /AssignCourse/Edit/5

        public ActionResult Edit(int id = 0)
        {
            AssignCourse assigncourse = db.AssignCourses.Find(id);
            if (assigncourse == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", assigncourse.DepartmentId);
            ViewBag.TeacherId = new SelectList(db.Teachers, "TeacherId", "Name", assigncourse.TeacherId);
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "Code", assigncourse.CourseId);
            return View(assigncourse);
        }

        //
        // POST: /AssignCourse/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AssignCourse assigncourse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assigncourse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", assigncourse.DepartmentId);
            ViewBag.TeacherId = new SelectList(db.Teachers, "TeacherId", "Name", assigncourse.TeacherId);
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "Code", assigncourse.CourseId);
            return View(assigncourse);
        }

        //
        // GET: /AssignCourse/Delete/5

        public ActionResult Delete(int id = 0)
        {
            AssignCourse assigncourse = db.AssignCourses.Find(id);
            if (assigncourse == null)
            {
                return HttpNotFound();
            }
            return View(assigncourse);
        }

        //
        // POST: /AssignCourse/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AssignCourse assigncourse = db.AssignCourses.Find(id);
            db.AssignCourses.Remove(assigncourse);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        public ActionResult LoadTeacher(int? departmentId)
        {
            var teacherList = db.Teachers.Where(u => u.DepartmentId == departmentId).ToList();
            ViewBag.TeacherId = new SelectList(teacherList, "TeacherId", "Name");
            return PartialView("~/Views/Shared/_FillteredTeacher.cshtml");
        }

        public ActionResult LoadCourse(int? departmentId)
        {
            var courseList = db.Courses.Where(u => u.DepartmentId == departmentId).ToList();
            ViewBag.CourseId = new SelectList(courseList, "CourseId", "Name");
            return PartialView("~/Views/shared/_FilteredCourse.cshtml");

        }

        public PartialViewResult TeacherInfoLoad(int? teacherId)
        {
            if (teacherId != null)
            {
                Teacher aTeacher = db.Teachers.FirstOrDefault(s => s.TeacherId == teacherId);
                ViewBag.Credit = aTeacher.CreditTaken;
                List<AssignCourse> assignTeachers =
                        db.AssignCourses.Where(t => t.TeacherId == teacherId).ToList();
                AssignCourse assign = null;
                if (assignTeachers.Count != 0)
                {
                    assign = assignTeachers.Last();
                }
                if (assign == null)
                {
                    ViewBag.RemainingCredit = aTeacher.CreditTaken;
                }
                else
                {
                    ViewBag.RemainingCredit = assign.RemaingCredit;
                }
                
                return PartialView("~/Views/Shared/_TeachersCreditInfo.cshtml");
            }
            else
            {
                return PartialView("~/Views/Shared/_TeachersCreditInfo.cshtml");
            }

        }


        public PartialViewResult CourseInfoLoad(int? courseId)
        {
            if (courseId != null)
            {
                Course aCourse = db.Courses.FirstOrDefault(s => s.CourseId == courseId);
                ViewBag.Code = aCourse.Code;
                ViewBag.Credit = aCourse.Credit;
                return PartialView("~/Views/Shared/_CourseInfo.cshtml");
            }
            else
            {
                return PartialView("~/Views/Shared/_CourseInfo.cshtml");
            }

        }

        public ActionResult ViewCourseStatus()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code");
            return View();
        }

        public PartialViewResult CourseStatusLoad(int? departmentId)
        {
            List<Course> courseList = new List<Course>();
            if (departmentId != null)
            {
                courseList = db.Courses.Where(r => r.DepartmentId == departmentId).ToList();
                if (courseList.Count == 0)
                {
                    ViewBag.NotAssigned = "Department Empty";
                }
            }
            
            
            return PartialView("~/Views/shared/_coursestatus.cshtml", courseList);
        }

        
    }

}