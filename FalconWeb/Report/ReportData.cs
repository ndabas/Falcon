using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FalconWeb.Models;
using System.Web.Security;

namespace FalconWeb.Report
{
    public class ReportData
    {
        private FalconContext db = new FalconContext();

        public ICollection<Device> GetDevices()
        {
            if (Roles.IsUserInRole("Admin"))
                return db.Devices.ToList();

            Guid userID = (Guid)Membership.GetUser().ProviderUserKey;
            return db.Users.Include("Devices").Single(u => u.ID == userID).Devices;
        }

        private TimeSpan TimeZoneOffset
        {
            get
            {
                return new TimeSpan(5, 30, 0);
            }
        }

        public IQueryable<Tracking> MovementReport(string IMEI, DateTime? from, DateTime? to)
        {
            var imeis = GetDevices().Select(d => d.IMEI);
            var query = db.TrackingData.Where(t => t.SatelliteFixStatus == "A"
                && t.Speed > 0
                && imeis.Contains(t.IMEI));

            if (!String.IsNullOrEmpty(IMEI))
                query = query.Where(t => t.IMEI == IMEI);

            if (from != null)
            {
                DateTime fromDate = from.Value.Add(TimeZoneOffset.Negate());
                query = query.Where(t => t.SatelliteTimeStamp > from);
            }

            if (to != null)
            {
                DateTime toDate = to.Value.Add(TimeZoneOffset.Negate());
                query = query.Where(t => t.SatelliteTimeStamp < to);
            }

            return query.OrderByDescending(t => t.SatelliteTimeStamp);
        }

        public IQueryable<Tracking> StoppageReport(string IMEI, DateTime? from, DateTime? to)
        {
            var imeis = GetDevices().Select(d => d.IMEI);
            var query = db.TrackingData.Where(t => t.SatelliteFixStatus == "A"
                && t.Speed == 0
                && imeis.Contains(t.IMEI));

            if (!String.IsNullOrEmpty(IMEI))
                query = query.Where(t => t.IMEI == IMEI);

            if (from != null)
            {
                DateTime fromDate = from.Value.Add(TimeZoneOffset.Negate());
                query = query.Where(t => t.SatelliteTimeStamp > from);
            }

            if (to != null)
            {
                DateTime toDate = to.Value.Add(TimeZoneOffset.Negate());
                query = query.Where(t => t.SatelliteTimeStamp < to);
            }

            return query.OrderByDescending(t => t.SatelliteTimeStamp);
        }
    }
}