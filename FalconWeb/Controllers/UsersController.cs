using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FalconWeb.Models;

namespace FalconWeb.Controllers
{
    public class UsersController : Controller
    {
        private FalconContext db = new FalconContext();

        //
        // GET: /Users/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var membershipUsers = Membership.GetAllUsers();
            var users = new List<UserViewModel>(membershipUsers.Count);

            foreach (MembershipUser mu in membershipUsers)
            {
                Guid userID = (Guid)mu.ProviderUserKey;
                var user = db.Users.Find(userID);
                if (user == null)
                {
                    user = new User();
                    user.ID = userID;
                }
                var userModel = new UserViewModel(user);
                userModel.UserName = mu.UserName;
                userModel.Email = mu.Email;
                users.Add(userModel);
            }

            return View(users);
        }

        //
        // GET: /Users/Edit/5

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(Guid id)
        {
            var membershipUser = Membership.GetUser(id);
            var user = db.Users.Find(id);
            if (user == null)
            {
                user = new User();
                user.ID = id;
            }
            var userModel = new UserViewModel(user);
            userModel.UserName = membershipUser.UserName;
            userModel.Email = membershipUser.Email;
            userModel.IsAdmin = Roles.IsUserInRole(membershipUser.UserName, "Admin");

            return View(userModel);
        }

        //
        // POST: /Users/Edit/5

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                Models.User userModel = new Models.User(user as Models.User);
                if (db.Users.Count(u => u.ID == userModel.ID) > 0)
                {
                    db.Entry(userModel).State = EntityState.Modified;
                }
                else
                {
                    db.Users.Add((Models.User)userModel);
                }
                db.SaveChanges();

                var mUser = Membership.GetUser(user.UserName);
                mUser.Email = user.Email;
                Membership.UpdateUser(mUser);

                if (user.IsAdmin && !Roles.IsUserInRole(user.UserName, "Admin"))
                    Roles.AddUserToRole(user.UserName, "Admin");
                if (!user.IsAdmin && Roles.IsUserInRole(user.UserName, "Admin"))
                    Roles.RemoveUserFromRole(user.UserName, "Admin");

                return RedirectToAction("Index");
            }
            return View(user);
        }

        //
        // GET: /Users/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Users/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
