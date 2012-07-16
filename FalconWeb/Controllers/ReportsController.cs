using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using FalconWeb.Models;
using System.Web.Security;

namespace FalconWeb.Controllers
{
    public class ReportsController : Controller
    {
        private FalconContext db = new FalconContext();

        //
        // GET: /Reports/

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        private ICollection<Device> GetDevices()
        {
            if(Roles.IsUserInRole("Admin"))
                return db.Devices.ToList();

            Guid userID = (Guid)Membership.GetUser().ProviderUserKey;
            return db.Users.Include("Devices").Single(u => u.ID == userID).Devices;
        }

        [Authorize]
        public ActionResult Movement()
        {
            ViewBag.Title = "Movement Report";
            ViewBag.DataAction = "MovementData";

            ViewBag.DeviceList = GetDevices().Select(d => new SelectListItem()
            {
                Text = d.VehicleNumber,
                Value = d.IMEI
            }).ToList();
            return View();
        }

        [GridAction]
        public ActionResult MovementData(string IMEI, DateTime? from, DateTime? to)
        {
            var imeis = GetDevices().Select(d => d.IMEI);
            var query = db.TrackingData.Where(t => t.SatelliteFixStatus == "A"
                && t.Speed > 0
                && imeis.Contains(t.IMEI));

            if(!String.IsNullOrEmpty(IMEI))
                query = query.Where(t => t.IMEI == IMEI);

            if (from != null)
            {
                DateTime fromDate = from.Value.ToUniversalTime();
                query = query.Where(t => t.SatelliteTimeStamp > from);
            }

            if (to != null)
            {
                DateTime toDate = to.Value.ToUniversalTime();
                query = query.Where(t => t.SatelliteTimeStamp < to);
            }

            return View(new GridModel<Tracking>
            {
                Data = query.OrderByDescending(t => t.SatelliteTimeStamp)
            });
        }

        [Authorize]
        public ActionResult Stoppage()
        {
            ViewBag.Title = "Stoppage Report";
            ViewBag.DataAction = "StoppageData";

            ViewBag.DeviceList = GetDevices().Select(d => new SelectListItem()
            {
                Text = d.VehicleNumber,
                Value = d.IMEI
            }).ToList();
            return View("Movement");
        }

        [GridAction]
        public ActionResult StoppageData(string IMEI, DateTime? from, DateTime? to)
        {
            var imeis = GetDevices().Select(d => d.IMEI);
            var query = db.TrackingData.Where(t => t.SatelliteFixStatus == "A"
                && t.Speed == 0
                && imeis.Contains(t.IMEI));

            if (!String.IsNullOrEmpty(IMEI))
                query = query.Where(t => t.IMEI == IMEI);

            if (from != null)
            {
                DateTime fromDate = from.Value.ToUniversalTime();
                query = query.Where(t => t.SatelliteTimeStamp > from);
            }

            if (to != null)
            {
                DateTime toDate = to.Value.ToUniversalTime();
                query = query.Where(t => t.SatelliteTimeStamp < to);
            }

            return View(new GridModel<Tracking>
            {
                Data = query.OrderByDescending(t => t.SatelliteTimeStamp)
            });
        }

        [Authorize]
        public ActionResult Speed()
        {
            ViewBag.Title = "Speed Report";
            ViewBag.DataAction = "SpeedData";

            ViewBag.DeviceList = GetDevices().Select(d => new SelectListItem()
            {
                Text = d.VehicleNumber,
                Value = d.IMEI
            }).ToList();
            return View("Movement");
        }

        [GridAction]
        public ActionResult SpeedData(string IMEI, DateTime? from, DateTime? to)
        {
            var imeis = GetDevices().Select(d => d.IMEI);
            var query = db.TrackingData.Where(t => t.SatelliteFixStatus == "A"
                && t.Speed > 0
                && imeis.Contains(t.IMEI));

            if (!String.IsNullOrEmpty(IMEI))
                query = query.Where(t => t.IMEI == IMEI);

            if (from != null)
            {
                DateTime fromDate = from.Value.ToUniversalTime();
                query = query.Where(t => t.SatelliteTimeStamp > from);
            }

            if (to != null)
            {
                DateTime toDate = to.Value.ToUniversalTime();
                query = query.Where(t => t.SatelliteTimeStamp < to);
            }

            return View(new GridModel<Tracking>
            {
                Data = query.OrderByDescending(t => t.Speed)
            });
        }

        [Authorize]
        public ActionResult Alarm()
        {
            ViewBag.Title = "Alarm Report";
            ViewBag.DataAction = "AlarmData";

            ViewBag.DeviceList = GetDevices().Select(d => new SelectListItem()
            {
                Text = d.VehicleNumber,
                Value = d.IMEI
            }).ToList();
            return View();
        }

        [GridAction]
        public ActionResult AlarmData(string IMEI, DateTime? from, DateTime? to)
        {
            var imeis = GetDevices().Select(d => d.IMEI);
            var query = db.TrackingData.Where(t => t.EventType != 0xAA
                && imeis.Contains(t.IMEI));

            if (!String.IsNullOrEmpty(IMEI))
                query = query.Where(t => t.IMEI == IMEI);

            if (from != null)
            {
                DateTime fromDate = from.Value.ToUniversalTime();
                query = query.Where(t => t.ServerTimeStamp > from);
            }

            if (to != null)
            {
                DateTime toDate = to.Value.ToUniversalTime();
                query = query.Where(t => t.ServerTimeStamp < to);
            }

            return View(new GridModel<TrackingReportModel>
            {
                Data = query.OrderByDescending(t => t.ServerTimeStamp).Select(t => new TrackingReportModel()
                {
                    ServerTimeStamp = t.ServerTimeStamp,
                    Address = t.Address,
                    EventType = t.EventType
                })
            });
        }

        [Authorize]
        public ActionResult Sos()
        {
            ViewBag.Title = "SOS Report";
            ViewBag.DataAction = "SosData";

            ViewBag.DeviceList = GetDevices().Select(d => new SelectListItem()
            {
                Text = d.VehicleNumber,
                Value = d.IMEI
            }).ToList();
            return View("Alarm");
        }

        [GridAction]
        public ActionResult SosData(string IMEI, DateTime? from, DateTime? to)
        {
            var imeis = GetDevices().Select(d => d.IMEI);
            var query = db.TrackingData.Where(t => t.EventType == 0x01
                && imeis.Contains(t.IMEI));

            if (!String.IsNullOrEmpty(IMEI))
                query = query.Where(t => t.IMEI == IMEI);

            if (from != null)
            {
                DateTime fromDate = from.Value.ToUniversalTime();
                query = query.Where(t => t.ServerTimeStamp > from);
            }

            if (to != null)
            {
                DateTime toDate = to.Value.ToUniversalTime();
                query = query.Where(t => t.ServerTimeStamp < to);
            }

            return View(new GridModel<TrackingReportModel>
            {
                Data = query.OrderByDescending(t => t.ServerTimeStamp).Select(t => new TrackingReportModel()
                {
                    ServerTimeStamp = t.ServerTimeStamp,
                    Address = t.Address,
                    EventType = t.EventType
                })
            });
        }

    }
}
