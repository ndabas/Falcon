using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FalconWeb.Models;
using System.Web.Security;

namespace FalconWeb.Controllers
{ 
    public class GeoFenceController : Controller
    {
        private FalconContext db = new FalconContext();

        //
        // GET: /GeoFence/

        [Authorize]
        public ViewResult Index()
        {
            if (Roles.IsUserInRole("Admin"))
                return View(db.GeoAreas.ToList());

            Guid userID = (Guid)Membership.GetUser().ProviderUserKey;
            return View(db.Users.Include("GeoAreas")
                .First(u => u.ID == userID).GeoAreas.ToList());
        }

        [Authorize]
        public ViewResult Assign(int id)
        {
            if (Roles.IsUserInRole("Admin"))
            {
                var devices = db.Devices.Select(d =>
                    new SelectListItem()
                    {
                        Text = d.VehicleNumber,
                        Value = d.IMEI
                    }).ToList();
                return View(devices);
            }

            Guid userID = (Guid)Membership.GetUser().ProviderUserKey;
            var userDevices = db.Users.Include("Devices").Single(u => u.ID == userID).Devices.Select(d =>
                    new SelectListItem()
                    {
                        Text = d.VehicleNumber,
                        Value = d.IMEI
                    }).ToList();
            return View(userDevices);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Assign(int id, List<string> IMEIs)
        {
            foreach (string imei in IMEIs)
            {
                GeoArea area = db.GeoAreas.Find(id);
                Device d = db.Devices.SingleOrDefault(dev => dev.IMEI == imei);
                if (d != null)
                {
                    d.GeoArea = area;
                }
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //
        // GET: /GeoFence/Create

        [Authorize]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /GeoFence/Create

        [Authorize]
        [HttpPost]
        public ActionResult Create(GeoArea geoarea)
        {
            if (ModelState.IsValid)
            {
                Guid userID = (Guid)Membership.GetUser().ProviderUserKey;
                geoarea.User = db.Users.Find(userID);
                db.GeoAreas.Add(geoarea);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(geoarea);
        }
        
        //
        // GET: /GeoFence/Edit/5

        [Authorize]
        public ActionResult Edit(int id)
        {
            GeoArea geoarea = db.GeoAreas.Find(id);
            return View("Create", geoarea);
        }

        //
        // POST: /GeoFence/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(GeoArea geoarea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(geoarea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Create", geoarea);
        }

        //
        // GET: /GeoFence/Delete/5
 
        [Authorize]
        public ActionResult Delete(int id)
        {
            GeoArea geoarea = db.GeoAreas.Find(id);
            return View(geoarea);
        }

        //
        // POST: /GeoFence/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {            
            GeoArea geoarea = db.GeoAreas.Find(id);
            db.GeoAreas.Remove(geoarea);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}