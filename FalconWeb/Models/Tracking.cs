using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace FalconWeb.Models
{
    public class Tracking
    {
        public int ID { get; set; }
        public DateTime ServerTimeStamp { get; set; }
        
        [Required]
        [StringLength(16)]
        public string IMEI { get; set; }
        public int EventType { get; set; }
        public DateTime? SatelliteTimeStamp { get; set; }
        [StringLength(1)]
        public string SatelliteFixStatus { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public float Speed { get; set; }
        public float Bearing { get; set; }
        public float PDOP { get; set; }
        public float HDOP { get; set; }
        public float VDOP { get; set; }
        public string IOStatus { get; set; }
        public DateTime DeviceTimeStamp { get; set; }
        public bool ChargeStatus { get; set; }
        public float BatteryVoltage { get; set; }
        public float InputVoltage { get; set; }
        public float ADC0 { get; set; }
        public float ADC1 { get; set; }
        public int LocationCode { get; set; }
        public int CellID { get; set; }
        public double Mileage { get; set; }
        public int Serial { get; set; }
        public string Address { get; set; }
        [StringLength(4)]
        public string GeofenceStatus { get; set; }

        public Tracking()
        {
        }

        public Tracking(Tracking t)
        {
            this.ADC0 = t.ADC0;
            this.ADC1 = t.ADC1;
            this.Address = t.Address;
            this.BatteryVoltage = t.BatteryVoltage;
            this.Bearing = t.Bearing;
            this.CellID = t.CellID;
            this.ChargeStatus = t.ChargeStatus;
            this.DeviceTimeStamp = t.DeviceTimeStamp;
            this.EventType = t.EventType;
            this.GeofenceStatus = t.GeofenceStatus;
            this.HDOP = t.HDOP;
            this.ID = t.ID;
            this.IMEI = t.IMEI;
            this.InputVoltage = t.InputVoltage;
            this.IOStatus = t.IOStatus;
            this.Latitude = t.Latitude;
            this.LocationCode = t.LocationCode;
            this.Longitude = t.Longitude;
            this.Mileage = t.Mileage;
            this.PDOP = t.PDOP;
            this.SatelliteFixStatus = t.SatelliteFixStatus;
            this.SatelliteTimeStamp = t.SatelliteTimeStamp;
            this.Serial = t.Serial;
            this.ServerTimeStamp = t.ServerTimeStamp;
            this.Speed = t.Speed;
            this.VDOP = t.VDOP;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ServerTimeStamp: {0}\r\n", this.ServerTimeStamp);
            sb.AppendFormat("IMEI: {0}\r\n", this.IMEI);
            sb.AppendFormat("EventType: {0}\r\n", this.EventType);
            sb.AppendFormat("SatelliteFixStatus: {0}\r\n", this.SatelliteFixStatus);
            if (this.SatelliteFixStatus == "A")
            {
                sb.AppendFormat("SatelliteTimeStamp: {0}\r\n", this.SatelliteTimeStamp);
                sb.AppendFormat("LatLong: {0},{1}\r\n", this.Latitude, this.Longitude);
                sb.AppendFormat("Speed: {0}\r\n", this.Speed);
            }
            sb.AppendFormat("PDOP, HDOP, VDOP: {0}, {1}, {2}\r\n", this.PDOP, this.HDOP, this.VDOP);
            sb.AppendFormat("Serial: {0}\r\n", this.Serial);

            return sb.ToString();
        }
    }
}