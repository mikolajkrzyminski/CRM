using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CRMnew.Models;
using PagedList;

namespace CRMnew.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AspNetUsersController : CustomController
    {
        private prjCRMEntities db = new prjCRMEntities();

        // GET: AspNetUsers
        public ActionResult Index(string sortOrder, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.EmailSortParam = String.IsNullOrEmpty(sortOrder) || sortOrder == "EmailSortDsc" ? "EmailSortAsc" : "EmailSortDsc";
            ViewBag.UserFirstNameSortParam = String.IsNullOrEmpty(sortOrder) || sortOrder == "UserFirstNameSortDsc" ? "UserFirstNameSortAsc" : "UserFirstNameSortDsc";
            ViewBag.UserSurnameSortParam = String.IsNullOrEmpty(sortOrder) || sortOrder == "UserSurnameSortDsc" ? "UserSurnameSortAsc" : "UserSurnameSortDsc";
            ViewBag.DateOfBirthSortParam = String.IsNullOrEmpty(sortOrder) || sortOrder == "DateOfBirthSortDsc" ? "DateOfBirthSortAsc" : "DateOfBirthSortDsc";
            var users = from s in db.AspNetUsers select s;
            switch (sortOrder)
            {
                case "EmailSortAsc": users = users.OrderBy(s => s.Email);           break;
                case "EmailSortDsc": users = users.OrderByDescending(s => s.Email); break;

                case "UserFirstNameSortAsc": users = users.OrderBy(s => s.UserFirstName);           break;
                case "UserFirstNameSortDsc": users = users.OrderByDescending(s => s.UserFirstName); break;
               
                case "UserSurnameSortAsc": users = users.OrderBy(s => s.UserSurname);           break;
                case "UserSurnameSortDsc": users = users.OrderByDescending(s => s.UserSurname); break;
               
                case "DateOfBirthSortAsc": users = users.OrderBy(s => s.DateOfBirth);           break;
                case "DateOfBirthSortDsc": users = users.OrderByDescending(s => s.DateOfBirth); break;
               
                default: users = users.OrderBy(s => s.UserSurname); break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }

        // GET: AspNetUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        // GET: AspNetUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AspNetUsers/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,UserFirstName,UserSurname,DateOfBirth")] AspNetUsers aspNetUsers)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspNetUsers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetUsers);
        }

        // GET: AspNetUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            AspNetRoles aspNetRoles = aspNetUsers.AspNetRoles.Count > 0 ? aspNetUsers.AspNetRoles.First() : new AspNetRoles();
            ViewBag.AspNetRoleId = new SelectList(db.AspNetRoles, "Id", "Name", aspNetRoles.Id);
            return View(aspNetUsers);
        }

        // POST: AspNetUsers/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,UserFirstName,UserSurname,DateOfBirth")] AspNetUsers aspNetUsers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUsers).State = EntityState.Modified;
                db.Entry(aspNetUsers).Property(x => x.PasswordHash).IsModified = false;
                db.Entry(aspNetUsers).Property(x => x.SecurityStamp).IsModified = false;
                db.Entry(aspNetUsers).Property(x => x.TwoFactorEnabled).IsModified = false;
                db.Entry(aspNetUsers).Property(x => x.LockoutEndDateUtc).IsModified = false;
                db.Entry(aspNetUsers).Property(x => x.LockoutEnabled).IsModified = false;                
                db.Entry(aspNetUsers).Property(x => x.AccessFailedCount).IsModified = false;
                db.Entry(aspNetUsers).Property(x => x.PhoneNumberConfirmed).IsModified = false;
                var userManager = new ApplicationUserManager(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(new ApplicationDbContext()));                
                IList<string> userRoles = await userManager.GetRolesAsync(aspNetUsers.Id);
                var result = await userManager.RemoveFromRolesAsync(aspNetUsers.Id, userRoles.ToArray<string>());                
                AspNetRoles aspNetRoles = db.AspNetRoles.Find(Request.Params["AspNetRoleId"]);                
                aspNetUsers.AspNetRoles.Add(aspNetRoles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }            
            return View(aspNetUsers);
        }

        // GET: AspNetUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        // POST: AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUsers);
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
    }
}
