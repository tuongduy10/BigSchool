using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        private dbBigSchool db = new dbBigSchool();

        // GET: Courses
        public ActionResult Index()
        {
            var courses = db.Courses.Include(c => c.Category);
            return View(courses.ToList());
        }
        

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LecturerId,Place,DateTime,CategoryId")] Course course)
        {
            if (!ModelState.IsValid)
            {
                course.ListCategory = db.Categories.ToList();
                return View("Create", course);
            }
            if (ModelState.IsValid)
            {
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                course.LecturerId = user.Id;

                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LecturerId,Place,DateTime,CategoryId")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        public ActionResult Attending()
        {
            dbBigSchool db = new dbBigSchool();
            ApplicationUser crnUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendaces = db.Attendances.Where(p => p.Attendee == crnUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in listAttendaces)
            {
                Course obj = temp.Course;
                obj.Name = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                                    .FindById(obj.LecturerId).Name;
                courses.Add(obj);
            }

            return View(courses);
        }


        public ActionResult Mine()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            dbBigSchool context = new dbBigSchool();
            var courses = context.Courses.Where(c => c.LecturerId == currentUser.Id && c.DateTime > DateTime.Now).ToList();
            foreach (Course i in courses)
            {
                i.Name = currentUser.Name;
            }
            return View(courses);
        }


        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            dbBigSchool context = new dbBigSchool();
            //danh sách giảng viên được theo dõi bởi người dùng (đăng nhập) hiện tại
            var listFollwee = context.Followings.Where(p => p.FollowerId == currentUser.Id).ToList();

            //danh sách các khóa học mà người dùng đã đăng ký
            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();

            var courses = new List<Course>();
            foreach (var course in listAttendances)
            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeId == course.Course.LecturerId)
                    {
                        Course objCourse = course.Course;
                        objCourse.Name = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                        .FindById(objCourse.LecturerId).Name;
                        courses.Add(objCourse);
                    }
                }
            }
            return View(courses);
        }
    }
}
