using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CRMnew.Models;

namespace CRMnew.Controllers
{
    public class ContactPersonsController : Controller
    {
        private prjCRMEntities db = new prjCRMEntities();

        // GET: ContactPersons
        public ViewResult Index(string searchString)
        {
            var contactPersons = db.ContactPersons.Include(c => c.AspNetUsers).Include(c => c.Companies);
            if (!String.IsNullOrEmpty(searchString))
            {
                contactPersons = contactPersons.Where(s => s.Surname.Contains(searchString));
            }
            return View(contactPersons.ToList());
        }

        // GET: ContactPersons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactPersons contactPersons = db.ContactPersons.Find(id);
            if (contactPersons == null)
            {
                return HttpNotFound();
            }
            return View(contactPersons);
        }

        // GET: ContactPersons/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name");
            return View();
        }

        // POST: ContactPersons/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Surname,PhoneNumber,Mail,Position,CompanyId,UserId")] ContactPersons contactPersons)
        {
            if (ModelState.IsValid)
            {
                db.ContactPersons.Add(contactPersons);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", contactPersons.UserId);
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", contactPersons.CompanyId);
            return View(contactPersons);
        }

        // GET: ContactPersons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactPersons contactPersons = db.ContactPersons.Find(id);
            if (contactPersons == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", contactPersons.UserId);
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", contactPersons.CompanyId);
            return View(contactPersons);
        }

        // POST: ContactPersons/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Surname,PhoneNumber,Mail,Position,CompanyId,UserId")] ContactPersons contactPersons)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contactPersons).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", contactPersons.UserId);
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", contactPersons.CompanyId);
            return View(contactPersons);
        }

        // GET: ContactPersons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactPersons contactPersons = db.ContactPersons.Find(id);
            if (contactPersons == null)
            {
                return HttpNotFound();
            }
            return View(contactPersons);
        }

        // POST: ContactPersons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContactPersons contactPersons = db.ContactPersons.Find(id);
            db.ContactPersons.Remove(contactPersons);
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
