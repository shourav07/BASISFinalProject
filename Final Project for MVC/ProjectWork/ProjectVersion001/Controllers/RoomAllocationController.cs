using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectVersion001.Models;

namespace DepartmentMVC.Controllers
{
    public class RoomAllocationController : Controller
    {
        private UniversityDbContex db = new UniversityDbContex();

        //
        // GET: /RoomAllocation/

        public ActionResult Index()
        {
            var roomallocations = db.RoomAllocations.Include(r => r.Department).Include(r => r.Course).Include(r => r.Room).Include(r => r.Day);
            return View(roomallocations.ToList());
        }

        //
        // GET: /RoomAllocation/Details/5

        public ActionResult Details(int id = 0)
        {
            RoomAllocation roomallocation = db.RoomAllocations.Find(id);
            if (roomallocation == null)
            {
                return HttpNotFound();
            }
            return View(roomallocation);
        }

        //
        // GET: /RoomAllocation/Create

        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code");
            ViewBag.CourseId = new SelectList("", "CourseId", "Code");
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "Name");
            ViewBag.DayId = new SelectList(db.Days, "DayId", "Name");
            return View();
        }

        //
        // POST: /RoomAllocation/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoomAllocation roomallocation)
        {

            Room room = db.Rooms.Find(roomallocation.RoomId);
            Course course = db.Courses.Find(roomallocation.CourseId);
            Day day = db.Days.Find(roomallocation.DayId);

           
            if (ModelState.IsValid)
            {
               
                int givenfrom, givenEnd;

                try
                {
                    givenfrom = ConvertTimetoInt(roomallocation.StartTime);
                }
                catch (Exception anException)
                {
                    if (TempData["ErrorMessage3"] == null)
                    {
                        TempData["ErrorMessage1"] = anException.Message;
                    }
                    TempData["ErrorMessage4"] = TempData["ErrorMessage3"];
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", course.DepartmentId);
                    ViewBag.CourseId = new SelectList(db.Courses.Where(c => c.DepartmentId == course.DepartmentId), "CourseId", "Code", roomallocation.CourseId);
                    ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "Name", roomallocation.RoomId);
                    ViewBag.DayId = new SelectList(db.Days, "DayId", "Name", roomallocation.DayId);
                    return View(roomallocation);
                    
                }

                try
                {
                    givenEnd = ConvertTimetoInt(roomallocation.EndTime);
                }
                catch (Exception anException)
                {
                    if (TempData["ErrorMessage3"] == null)
                    {
                        TempData["ErrorMessage2"] = anException.Message;
                    }
                    TempData["ErrorMessage5"] = TempData["ErrorMessage3"];
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", course.DepartmentId);
                    ViewBag.CourseId = new SelectList(db.Courses.Where(c => c.DepartmentId == course.DepartmentId), "CourseId", "Code", roomallocation.CourseId);
                    ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "Name", roomallocation.RoomId);
                    ViewBag.DayId = new SelectList(db.Days, "DayId", "Name", roomallocation.DayId);
                    return View(roomallocation);
                }

                if (givenEnd < givenfrom)
                {
                    TempData["Message"] = "Class Should Start Before End (24 hours)";
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", course.DepartmentId);
                    ViewBag.CourseId = new SelectList(db.Courses.Where(c => c.DepartmentId == course.DepartmentId), "CourseId", "Code", roomallocation.CourseId);
                    ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "Name", roomallocation.RoomId);
                    ViewBag.DayId = new SelectList(db.Days, "DayId", "Name", roomallocation.DayId);
                    return View(roomallocation);
                }
                if (givenEnd == givenfrom)
                {
                    TempData["Message"] = "Class Should Start Before End (24 hours)";
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", course.DepartmentId);
                    ViewBag.CourseId = new SelectList(db.Courses.Where(c => c.DepartmentId == course.DepartmentId), "CourseId", "Code", roomallocation.CourseId);
                    ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "Name", roomallocation.RoomId);
                    ViewBag.DayId = new SelectList(db.Days, "DayId", "Name", roomallocation.DayId);
                    return View(roomallocation);
                }

                if ((givenfrom < 0) || (givenEnd >= (24*60)))
                {
                    TempData["Message"] = " 24 hours--format HH:MM";
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", course.DepartmentId);
                    ViewBag.CourseId = new SelectList(db.Courses.Where(c => c.DepartmentId == course.DepartmentId), "CourseId", "Code", roomallocation.CourseId);
                    ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "Name", roomallocation.RoomId);
                    ViewBag.DayId = new SelectList(db.Days, "DayId", "Name", roomallocation.DayId);
                    return View(roomallocation);
                }

                    List<RoomAllocation> overLapList=new List<RoomAllocation>();

                    var DayRoomAllocationList =
                    db.RoomAllocations.Include(c => c.Course).Include(d =>d.Day).Include(r => r.Room).Where(r => r.RoomId == roomallocation.RoomId && r.DayId == roomallocation.DayId)
                        .ToList();

                    foreach (var aDayroomAllocation in DayRoomAllocationList)
                    {
                        //int OverlapFrom = 0;
                        //int OverlapEnd = 0;
                        //RoomAllocation overlap =new RoomAllocation();

                        int DbFrom = ConvertTimetoInt(aDayroomAllocation.StartTime);                        
                        int DbEnd =  ConvertTimetoInt(aDayroomAllocation.EndTime);
                      
                        if (
                                ((DbFrom <= givenfrom) && (givenfrom < DbEnd))
                                || ((DbFrom < givenEnd) && (givenEnd <= DbEnd))
                                || ((givenfrom <= DbFrom) && (DbEnd <= givenEnd))
                            )
                        {
                            overLapList.Add(aDayroomAllocation);
                        }
                        
                    }
                    if (overLapList.Count == 0)
                    {
                      
                        db.RoomAllocations.Add(roomallocation);
                        db.SaveChanges();
                        TempData["Message"] = "Room : " + room.Name + "Allocated "
                        + " for course : " + course.Code + " From " + roomallocation.StartTime
                        + " to " + roomallocation.EndTime + " on " + day.Name;
                    }
                    else
                    {
                        string message = "Room : " + room.Name + " Overlapped With : ";
                        foreach (var anOverlappedRoom in overLapList)
                        {
                            int dbFrom = ConvertTimetoInt(anOverlappedRoom.StartTime);
                            int dbEnd = ConvertTimetoInt(anOverlappedRoom.EndTime);

                            string overlapStart, overlapEnd;

                            if ((dbFrom <= givenfrom) && (givenfrom < dbEnd))
                                overlapStart = roomallocation.StartTime;
                            else
                                overlapStart = anOverlappedRoom.StartTime;

                            if ((dbFrom < givenEnd) && (givenEnd <= dbEnd))
                                overlapEnd = roomallocation.EndTime;
                            else
                                overlapEnd = anOverlappedRoom.EndTime;
                            message += " Course : " + anOverlappedRoom.Course.Code + " Start Time : "
                                + anOverlappedRoom.StartTime + " End Time : "
                                + anOverlappedRoom.EndTime + " Overlapped from : ";
                            message += overlapStart + " To " + overlapEnd;
                        }

                        TempData["Message"] = message + " on " + day.Name ;

                        ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code");
                        ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", course.DepartmentId);
                        ViewBag.CourseId = new SelectList(db.Courses.Where(c => c.DepartmentId == course.DepartmentId), "CourseId", "Code", roomallocation.CourseId);
                        ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "Name", roomallocation.RoomId);
                        ViewBag.DayId = new SelectList(db.Days, "DayId", "Name", roomallocation.DayId);
                        return View(roomallocation);
                    }
                //}
                
                return RedirectToAction("Create");
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", roomallocation.DepartmentId);
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "Code", roomallocation.CourseId);
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "Name", roomallocation.RoomId);
            ViewBag.DayId = new SelectList(db.Days, "DayId", "Name", roomallocation.DayId);
            return View(roomallocation);
        }

        private int ConvertTimetoInt(string timeStr)
        {
            try
            {
                if (TimeFormateCheck(timeStr))
                {
                    if (timeStr.Length == 4)
                    {
                        timeStr = "0" + timeStr;
                    }
                    string hour = timeStr[0].ToString() + timeStr[1];
                    string minute = timeStr[3].ToString() + timeStr[4];

                    if (Convert.ToInt32(minute) > 59 || Convert.ToInt32(minute) < 0 )
                    {
                        TempData["ErrorMessage3"]="Minites Must Be Equal Or Less Then 59";
                        throw new Exception();
                    }

                    int time = (Convert.ToInt32(hour)*60);
                    time += Convert.ToInt32(minute);
                    return time;
                }
                else
                {
                    throw new Exception("24 hours--format HH:MM");
                }
            }

            catch (FormatException aFormatException)
            {
                throw new FormatException(
                    "24 hours--format HH:MM", aFormatException);
            }

            catch (IndexOutOfRangeException aRangException)
            {
                throw new IndexOutOfRangeException(
                    "24 hours--format HH:MM", aRangException);
            }

            catch (Exception anException)
            {
                throw new Exception("24 hours--format HH:MM", anException);
            }
        }

        private bool TimeFormateCheck(string timeStr)
        {
            char[] list = timeStr.ToCharArray();
            foreach (var aChar in list)
            {
                if (aChar==':')
                {
                    return true;
                }
            }
            return false;
        }

        public ActionResult RoomAllocationView()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code");
            List<Course> CourseList = db.Courses.ToList();

            foreach (var aCourse in CourseList)
            {
                aCourse.RoomAllocationList
                    = db.RoomAllocations.Include(a => a.Room).Include(a => a.Day)
                    .Where(a => a.CourseId == aCourse.CourseId).ToList();
            }

            return View(CourseList);
        }
        //
        // GET: /RoomAllocation/Edit/5

        public ActionResult Edit(int id = 0)
        {
            RoomAllocation roomallocation = db.RoomAllocations.Find(id);
            if (roomallocation == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", roomallocation.DepartmentId);
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "Code", roomallocation.CourseId);
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "Name", roomallocation.RoomId);
            ViewBag.DayId = new SelectList(db.Days, "DayId", "Name", roomallocation.DayId);
            return View(roomallocation);
        }

        //
        // POST: /RoomAllocation/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RoomAllocation roomallocation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roomallocation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Code", roomallocation.DepartmentId);
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "Code", roomallocation.CourseId);
            ViewBag.RoomId = new SelectList(db.Rooms, "RoomId", "Name", roomallocation.RoomId);
            ViewBag.DayId = new SelectList(db.Days, "DayId", "Name", roomallocation.DayId);
            return View(roomallocation);
        }

        //
        // GET: /RoomAllocation/Delete/5

        public ActionResult Delete(int id = 0)
        {
            RoomAllocation roomallocation = db.RoomAllocations.Find(id);
            if (roomallocation == null)
            {
                return HttpNotFound();
            }
            return View(roomallocation);
        }

        //
        // POST: /RoomAllocation/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoomAllocation roomallocation = db.RoomAllocations.Find(id);
            db.RoomAllocations.Remove(roomallocation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        public ActionResult LoadCourse(int? departmentId)
        {
            var courseList = db.Courses.Where(u => u.DepartmentId == departmentId).ToList();
            ViewBag.CourseId = new SelectList(courseList, "CourseId", "Name");
            return PartialView("~/Views/shared/_FilteredCourse.cshtml");

        }

       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public PartialViewResult AllocatedRoomLoad(int? departmentId)
        {
            List<Course> courseList = null;
            if (departmentId != null)
            {
                courseList = db.Courses.Where(c => c.DepartmentId == departmentId).ToList();
            }
            //else
            //{
            //    CourseList = db.Courses.ToList();
            //}

            foreach (var aCourse in courseList)
            {
                aCourse.RoomAllocationList
                    = db.RoomAllocations.Include(a => a.Room).Include(a => a.Day)
                    .Where(a => a.CourseId == aCourse.CourseId).ToList();
            }
            if (courseList.Count == 0)
            {
                ViewBag.NoCourse = "Department Empty";
            }
            return PartialView("~/Views/shared/_RoomAllocationView.cshtml", courseList);
        }
    }
}