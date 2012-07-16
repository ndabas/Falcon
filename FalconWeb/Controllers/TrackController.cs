using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FalconWeb.Models;
using System.Web.Security;
using System.Net;

namespace FalconWeb.Controllers
{
    public class TrackController : Controller
    {
        private FalconContext db = new FalconContext();

        //
        // GET: /Track/

        [Authorize]
        public ActionResult Index()
        {
            if (Roles.IsUserInRole("Admin"))
                return View(db.Devices.ToList());

            Guid userID = (Guid)Membership.GetUser().ProviderUserKey;
            return View(db.Users.Include("Devices").First(u => u.ID == userID).Devices.ToList());
        }

        //
        // GET: /Device/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            Guid userID = (Guid)Membership.GetUser().ProviderUserKey;
            if (Roles.IsUserInRole("Admin") ||
                db.Users
                .Include("Devices")
                .First(u => u.ID == userID)
                .Devices.Count(d => d.ID == id) > 0)
            {
                Device device = db.Devices.Find(id);
                ViewBag.LastPosition = db.TrackingData
                    .Where(t => t.IMEI == device.IMEI)
                    .Where(t => t.SatelliteFixStatus == "A")
                    .OrderByDescending(t => t.SatelliteTimeStamp)
                    .Take(1).SingleOrDefault();
                return View(device);
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        public ActionResult TrackingData(int id)
        {
            Guid userID = (Guid)Membership.GetUser().ProviderUserKey;
            if (Roles.IsUserInRole("Admin") ||
                db.Users
                .Include("Devices")
                .First(u => u.ID == userID)
                .Devices.Count(d => d.ID == id) > 0)
            {
                Device device = db.Devices.Find(id);
                var trackings = db.TrackingData
                    .Where(t => t.IMEI == device.IMEI)
                    .Where(t => t.SatelliteFixStatus == "A")
                    .OrderByDescending(t => t.SatelliteTimeStamp)
                    .Take(100);
                return Json(trackings.ToList());
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        public ActionResult MultiTrackingData(List<string> IMEIs, int? minutes, DateTime? from, DateTime? to)
        {
            Guid userID = (Guid)Membership.GetUser().ProviderUserKey;
            if (Roles.IsUserInRole("Admin") ||
                db.Users
                .Include("Devices")
                .First(u => u.ID == userID)
                .Devices.All(d => IMEIs.Contains(d.IMEI)))
            {
                IQueryable<Tracking> trackings;
                if (minutes != null)
                {
                    var fromDate = DateTime.UtcNow.AddMinutes(-(double)minutes);
                    trackings = db.TrackingData
                        .Where(t => IMEIs.Contains(t.IMEI) &&
                            t.SatelliteFixStatus == "A" &&
                            t.SatelliteTimeStamp > fromDate)
                        .OrderByDescending(t => t.SatelliteTimeStamp);
                    return Json(trackings.ToList());
                }
                else if(from != null && to != null)
                {
                    var fromDate = from.Value.ToUniversalTime();
                    var toDate = to.Value.ToUniversalTime();
                    trackings = db.TrackingData
                        .Where(t => IMEIs.Contains(t.IMEI) &&
                            t.SatelliteFixStatus == "A" &&
                            t.SatelliteTimeStamp > fromDate &&
                            t.SatelliteTimeStamp < toDate)
                        .OrderByDescending(t => t.SatelliteTimeStamp);
                    return Json(trackings.ToList());
                }
                else
                {
                    return new HttpNotFoundResult();
                }
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
        }

        private WebClient webClient = new WebClient();

        public string SendCommand(string IMEI, string cmd)
        {
            Guid userID = (Guid)Membership.GetUser().ProviderUserKey;
            if (Roles.IsUserInRole("Admin") ||
                db.Users
                .Include("Devices")
                .First(u => u.ID == userID)
                .Devices.Count(d => d.IMEI == IMEI) > 0)
            {
                string message = null;
                switch (cmd)
                {
                    case "enable":
                        message = "*000000,025,B,0#";
                        break;
                    case "disable":
                        message = "*000000,025,B,1#";
                        break;
                    default:
                        return "InvalidCommand";
                }

                Device device = db.Devices.SingleOrDefault(d => d.IMEI == IMEI);
                webClient.DownloadString("http://api.clickatell.com/http/sendmsg?user=ndabas&password=rahulgupta64&api_id=3313563&to="
                + device.MobileNumber + "&from=919891412580&text="
                + HttpUtility.UrlEncode(message));

                return "OK";
            }
            else
            {
                return "Unauthorized";
            }
        }
    }
}
