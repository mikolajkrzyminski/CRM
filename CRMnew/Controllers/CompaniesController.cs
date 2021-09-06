using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CRMnew.Models;
using Microsoft.AspNet.Identity;
using PagedList;

namespace CRMnew.Controllers
{
    public class CompaniesController : CustomController
    {
        private prjCRMEntities db = new prjCRMEntities();

        // GET: Companies
        [Authorize(Roles = "Mod,User")]
        public ActionResult Index(string sortOrder, bool? showDeleted, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) || sortOrder == "NameSortDsc" ? "NameSortAsc" : "NameSortDsc";
            ViewBag.NIPSortParam = String.IsNullOrEmpty(sortOrder) || sortOrder == "NIPSortDsc" ? "NIPSortAsc" : "NIPSortDsc";
            ViewBag.AdressSortParam = String.IsNullOrEmpty(sortOrder) || sortOrder == "AddressSortDsc" ? "AddressSortAsc" : "AddressSortDsc";
            ViewBag.CitySortParam = String.IsNullOrEmpty(sortOrder) || sortOrder == "CitySortDsc" ? "CitySortAsc" : "CitySortDsc";
            ViewBag.BrancheSortParam = String.IsNullOrEmpty(sortOrder) || sortOrder == "BrancheSortDsc" ? "BrancheSortAsc" : "BrancheSortDsc";
            ViewBag.UserSortParam = String.IsNullOrEmpty(sortOrder) || sortOrder == "UserSortDsc" ? "UserSortAsc" : "UserSortDsc";
            var companies = from c in db.Companies.Include(c => c.Branches).Include(c => c.AspNetUsers) select c;
            switch (sortOrder)
            {
                case "NameSortAsc": companies = companies.OrderBy(c => c.Name); break;
                case "NameSortDsc": companies = companies.OrderByDescending(c => c.Name); break;

                case "NIPSortAsc": companies = companies.OrderBy(c => c.NIP); break;
                case "NIPSortDsc": companies = companies.OrderByDescending(c => c.NIP); break;

                case "AddressSortAsc": companies = companies.OrderBy(c => c.Address); break;
                case "AddressSortDsc": companies = companies.OrderByDescending(c => c.Address); break;

                case "CitySortAsc": companies = companies.OrderBy(c => c.City); break;
                case "CitySortDsc": companies = companies.OrderByDescending(c => c.City); break;

                case "BrancheSortAsc": companies = companies.OrderBy(c => c.Branches.BranchName); break;
                case "BrancheSortDsc": companies = companies.OrderByDescending(c => c.Branches.BranchName); break;

                case "UserSortAsc": companies = companies.OrderBy(c => c.AspNetUsers.Email); break;
                case "UserSortDsc": companies = companies.OrderByDescending(c => c.AspNetUsers.Email); break;

                default: companies = companies.OrderBy(c => c.Name); break;
            }
// var companies = db.Companies.Include(c => c.Branches).Include(c => c.AspNetUsers);

            int pageSize = 2;
            int pageNumber = (page ?? 1);

            if (showDeleted == null || showDeleted == false)
            {
                companies = companies.Where(c => c.IsDeleted == false);
            }

            ViewBag.showDeleted = showDeleted;

            return View(companies.ToPagedList(pageNumber, pageSize));
        }

        // GET: Companies/Details/5
        [Authorize(Roles = "Mod,User")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Companies companies = db.Companies.Find(id);
            if (companies == null)
            {
                return HttpNotFound();
            }
            return View(companies);
        }

        // GET: Companies/Create
        [Authorize(Roles = "Mod,User")]
        public ActionResult Create()
        {
            ViewBag.BranchId = new SelectList(db.Branches, "Id", "BranchName");
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: Companies/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Mod")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,NIP,BranchId,Address,City,UserId,IsDeleted")] Companies companies)
        {
            if (ModelState.IsValid)
            {
                companies.UserId = User.Identity.GetUserId();
                db.Companies.Add(companies);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BranchId = new SelectList(db.Branches, "Id", "BranchName", companies.BranchId);
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", companies.UserId);
            return View(companies);
        }

        // GET: Companies/Edit/5
        [Authorize(Roles = "Mod")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Companies companies = db.Companies.Find(id);
            if (companies == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchId = new SelectList(db.Branches, "Id", "BranchName", companies.BranchId);
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", companies.UserId);
            return View(companies);
        }

        // POST: Companies/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Mod")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,NIP,BranchId,Address,City,UserId,IsDeleted")] Companies companies)
        {
            if (ModelState.IsValid)
            {
                companies.UserId = User.Identity.GetUserId();
                db.Entry(companies).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchId = new SelectList(db.Branches, "Id", "BranchName", companies.BranchId);
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", companies.UserId);
            return View(companies);
        }

        // GET: Companies/Delete/5
        [Authorize(Roles = "Mod")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Companies companies = db.Companies.Find(id);
            if (companies == null)
            {
                return HttpNotFound();
            }
            return View(companies);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Mod")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Companies companies = db.Companies.Find(id);
            //db.Companies.Remove(companies);
            companies.IsDeleted = true;
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
