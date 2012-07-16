using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using FalconWeb.Models;
using System.Net;

namespace FalconWeb.Controllers
{
    public class TrackingDataController : Controller
    {
        private FalconContext db = new FalconContext();

        //
        // GET: /TrackingData/

        public ActionResult Index()
        {
            return new HttpUnauthorizedResult();
        }

        public string Report(string data)
        {
            string[] parts = data.Split('|');
            if (parts.Length != 14)
                return "Incomplete";

            Tracking trackingData = new Tracking();

            trackingData.ServerTimeStamp = DateTime.UtcNow;

            trackingData.IMEI = parts[0].Substring(4);
            trackingData.EventType = Convert.ToInt32(parts[1].Substring(0, 2), 16);

            string gprmc = parts[1].Substring(2);
            if (gprmc.StartsWith("$GPRMC"))
            {
                GPRMCData gprmcData = ProcessGPRMC(gprmc);

                trackingData.SatelliteTimeStamp = gprmcData.TimeStamp;
                trackingData.SatelliteFixStatus = new string(gprmcData.DataValid, 1);
                trackingData.Latitude = gprmcData.Latitude;
                trackingData.Longitude = gprmcData.Longitude;
                trackingData.Speed = gprmcData.GroundSpeed * 1.852f; // knots -> kmph
                trackingData.Bearing = gprmcData.Bearing;
            }
            else
            {
                trackingData.SatelliteFixStatus = "V";
            }

            trackingData.PDOP = Convert.ToSingle(parts[2]);
            trackingData.HDOP = Convert.ToSingle(parts[3]);
            trackingData.VDOP = Convert.ToSingle(parts[4]);

            trackingData.IOStatus = parts[5];

            trackingData.DeviceTimeStamp = new DateTime(
                Convert.ToInt32(parts[6].Substring(0, 4)), // year
                Convert.ToInt32(parts[6].Substring(4, 2)), // month
                Convert.ToInt32(parts[6].Substring(6, 2)), // day
                Convert.ToInt32(parts[6].Substring(8, 2)), // hour
                Convert.ToInt32(parts[6].Substring(10, 2)), // minute
                Convert.ToInt32(parts[6].Substring(12, 2)), // second
                DateTimeKind.Utc);

            trackingData.ChargeStatus = parts[7][0] == '1';
            trackingData.BatteryVoltage = Convert.ToSingle(parts[7].Substring(1, 3)) / 100.0f;
            trackingData.InputVoltage = Convert.ToSingle(parts[7].Substring(4)) / 100.0f;

            trackingData.ADC0 = Convert.ToSingle(parts[8].Substring(0, 4)) / 100.0f;
            trackingData.ADC1 = Convert.ToSingle(parts[8].Substring(4, 4)) / 100.0f;

            trackingData.LocationCode = Convert.ToInt32(parts[9].Substring(0, 4), 16);
            trackingData.CellID = Convert.ToInt32(parts[9].Substring(4, 4), 16);

            trackingData.Mileage = Convert.ToDouble(parts[11]);
            trackingData.Serial = Convert.ToInt32(parts[12]);

            if (trackingData.SatelliteFixStatus == "A")
                trackingData.Address = ReverseGeocode(trackingData.Latitude, trackingData.Longitude);

            Device device = db.Devices.Include("GeoArea").SingleOrDefault(d => d.IMEI == trackingData.IMEI);
            if (trackingData.SatelliteFixStatus == "A" && device != null && device.GeoArea != null)
            {
                string[] lls = device.GeoArea.LatLongs.Split('|');
                int count = lls.Length;
                double[] polyX = new double[count];
                double[] polyY = new double[count];

                for (int k = 0; k < count; k++)
                {
                    string[] ll = lls[k].Split(',');
                    polyX[k] = Convert.ToDouble(ll[0]);
                    polyY[k] = Convert.ToDouble(ll[1]);
                }

                bool oddNodes = false;
                int i, j = count - 1;
                double x = trackingData.Latitude;
                double y = trackingData.Longitude;

                for (i = 0; i < count; i++)
                {
                    if (polyY[i] < y && polyY[j] >= y || polyY[j] < y && polyY[i] >= y)
                    {
                        if (polyX[i] + (y - polyY[i]) / (polyY[j] - polyY[i]) * (polyX[j] - polyX[i]) < x)
                        {
                            oddNodes = !oddNodes;
                        }
                    }
                    j = i;
                }

                trackingData.GeofenceStatus = oddNodes ? "in" : "out";
            }

            db.TrackingData.Add(trackingData);
            db.SaveChanges();

            SendNotifications(trackingData);

            return "OK";
        }

        private WebClient webClient = new WebClient();

        private void SendMessage(string to, string message)
        {
            webClient.DownloadString("http://api.clickatell.com/http/sendmsg?user=ndabas&password=rahulgupta64&api_id=3313563&to="
                + to + "&from=&text="
                + HttpUtility.UrlEncode(message));
        }

        private void SendNotifications(Tracking trackingData)
        {
            Device device = db.Devices.Include("Users").SingleOrDefault(d => d.IMEI == trackingData.IMEI);
            if (device != null)
            {
                if (trackingData.GeofenceStatus != null)
                {
                    Tracking last = db.TrackingData.Where(
                        t => t.IMEI == trackingData.IMEI &&
                            t.SatelliteTimeStamp < trackingData.SatelliteTimeStamp)
                            .OrderByDescending(t => t.SatelliteTimeStamp)
                            .FirstOrDefault();
                    if (last != null && last.GeofenceStatus != trackingData.GeofenceStatus)
                    {
                        foreach (User user in device.Users)
                        {
                            if (!String.IsNullOrEmpty(user.MobileNumber))
                            {
                                SendMessage(user.MobileNumber, String.Format("{0} is {1} geofence.", device.VehicleNumber, trackingData.GeofenceStatus));
                            }
                        }
                    }
                }

                if (trackingData.EventType == 0xAA)
                    return;

                string message = String.Empty;
                switch (trackingData.EventType)
                {
                    case 0x01:
                        message = "SOS button pressed";
                        break;
                    case 0x09:
                        message = "Auto shutdown";
                        break;
                    case 0x10:
                        message = "Low battery";
                        break;
                    case 0x60:
                        message = "Begin charge";
                        break;
                    case 0x61:
                        message = "End charge";
                        break;
                    case 0x91:
                        message = "Going to sleep mode";
                        break;
                    case 0x92:
                        message = "Wake up from sleep mode";
                        break;
                    default:
                        // message = String.Format("Event: {0}", trackingData.EventType);
                        break;
                }

                message = String.Format("Vehicle: {0}\r\n{1}", device.VehicleNumber, message);

                foreach (User user in device.Users)
                {
                    if (!String.IsNullOrEmpty(user.MobileNumber))
                    {
                        SendMessage(user.MobileNumber, message);
                    }

                    if (trackingData.EventType == 0x01 &&
                        !String.IsNullOrEmpty(user.EmergencyNumber))
                    {
                        SendMessage(user.EmergencyNumber,
                            String.Format("SOS Message from vehicle {0}:\r\n{1}",
                            device.VehicleNumber,
                            user.EmergencyMessage ?? String.Empty));
                    }
                }
            }
        }

        private string ReverseGeocode(double latitude, double longitude)
        {
            try
            {
                string key = "AujGnxnB4an52KZpuBv-R8NJj-qO29k4UQhi2_-P4d2l_8exWoKoN1-ZlITXhY1T";
                BingMapsGeocodeService.ReverseGeocodeRequest request = new BingMapsGeocodeService.ReverseGeocodeRequest();

                request.Credentials = new BingMapsGeocodeService.Credentials();
                request.Credentials.ApplicationId = key;

                BingMapsGeocodeService.Location point = new BingMapsGeocodeService.Location();
                point.Latitude = latitude;
                point.Longitude = longitude;

                request.Location = point;

                BingMapsGeocodeService.GeocodeServiceClient service = new BingMapsGeocodeService.GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
                BingMapsGeocodeService.GeocodeResponse response = service.ReverseGeocode(request);
                return response.Results[0].DisplayName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static double DDMMSSToDecimalDegrees(string data)
        {
            var ddmmss = (Convert.ToDouble(data) / 100);
            var degrees = (int)ddmmss;
            var minutesseconds = ((ddmmss - degrees) * 100) / 60.0;
            return degrees + minutesseconds;
        }

        public GPRMCData ProcessGPRMC(string data)
        {
            string[] fields = data.Split(',');
            GPRMCData GPRMC = new GPRMCData();

            GPRMC.TimeStamp = new DateTime(
                Convert.ToInt32(fields[9].Substring(4, 2)) + 2000, // year
                Convert.ToInt32(fields[9].Substring(2, 2)), // month
                Convert.ToInt32(fields[9].Substring(0, 2)), // day
                Convert.ToInt32(fields[1].Substring(0, 2)), // hour
                Convert.ToInt32(fields[1].Substring(2, 2)), // minute
                Convert.ToInt32(fields[1].Substring(4, 2)), // second
                DateTimeKind.Utc);
            
            
            GPRMC.DataValid = Convert.ToChar(fields[2]);
            
            //Latitude
            GPRMC.Latitude = DDMMSSToDecimalDegrees(fields[3]);
            if (fields[4] == "S")
                GPRMC.Latitude = GPRMC.Latitude * -1;
            
            //Longitude
            GPRMC.Longitude = DDMMSSToDecimalDegrees(fields[5]);
            if (fields[6] == "W")
                GPRMC.Longitude = GPRMC.Longitude * -1;

            GPRMC.GroundSpeed = Convert.ToSingle(fields[7]);

            if (!String.IsNullOrEmpty(fields[8]))
                GPRMC.Bearing = Convert.ToSingle(fields[8]);

            return GPRMC;
        }

    }
}
