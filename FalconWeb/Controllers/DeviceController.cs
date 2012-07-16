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
    public class DeviceController : Controller
    {
        private FalconContext db = new FalconContext();

        //
        // GET: /Device/
        [Authorize(Roles = "Admin")]
        public ViewResult Index()
        {
            return View(db.Devices.ToList());
        }

        //
        // GET: /Device/Details/5

        [Authorize(Roles = "Admin")]
        public ViewResult Details(int id)
        {
            Device device = db.Devices.Find(id);
            return View(device);
        }

        //
        // GET: /Device/Create

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Device/Create

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Device device)
        {
            if (ModelState.IsValid)
            {
                db.Devices.Add(device);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(device);
        }
        
        //
        // GET: /Device/Edit/5

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Device device = db.Devices.Find(id);
            ViewBag.UserList = GetUserNames(id);
            return View(device);
        }

        //
        // POST: /Device/Edit/5

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(Device device)
        {
            ModelState.Remove("Users");

            if (ModelState.IsValid)
            {
                var dev = db.Devices.Include("Users").First(d => d.ID == device.ID);
                dev.Users.Clear();
                db.SaveChanges();
                db.Entry(dev).State = EntityState.Detached;

                db.Entry(device).State = EntityState.Modified;

                if (!String.IsNullOrEmpty(this.HttpContext.Request.Form["Users"]))
                {
                    string[] ids = this.HttpContext.Request.Form["Users"].Split(',');
                    foreach (var id in ids)
                    {
                        Guid userID = new Guid(id);
                        Models.User user = db.Users.Find(userID) ?? (new Models.User() { ID = userID });
                        device.Users.Add(user);
                    }
                }
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserList = GetUserNames(device.ID);
            return View(device);
        }

        //
        // GET: /Device/Delete/5

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Device device = db.Devices.Find(id);
            return View(device);
        }

        //
        // POST: /Device/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Device device = db.Devices.Find(id);
            db.Devices.Remove(device);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private MultiSelectList GetUserNames(int id)
        {
            Device device = db.Devices.Include("Users").First(d => d.ID == id);
            var users = Membership.GetAllUsers();
            var list = new List<SelectListItem>(users.Count);
            foreach (MembershipUser user in users)
            {
                Guid userID = (Guid)user.ProviderUserKey;
                list.Add(new SelectListItem()
                {
                    Value = userID.ToString(),
                    Text = user.UserName,
                    Selected = device.Users.Count(u => u.ID == userID) > 0
                });
            }
            
            var selectList = new MultiSelectList(list, "Value", "Text", list.Where(i => i.Selected == true));
            return selectList;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}