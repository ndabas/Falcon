using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FalconWeb.Models
{
    public class TrackingReportModel : Tracking
    {
        public string EventDescription
        {
            get
            {
                switch (this.EventType)
                {
                    case 0x01:
                        return "SOS button pressed";
                    case 0x49:
                        return "Button A pressed";
                    case 0x09:
                        return "Auto shutdown";
                    case 0x10:
                        return "Low battery";
                    case 0x11:
                        return "Overspeed alarm";
                    case 0x13:
                        return "Recover from overspeed";
                    case 0x30:
                        return "Parking alarm";
                    case 0x42:
                        return "Outside geo-fence";
                    case 0x43:
                        return "inside geo-fence";
                    case 0x50:
                        return "IO-1 close";
                    case 0x51:
                        return "IO-1 open";
                    case 0x52:
                        return "IO-2 close";
                    case 0x53:
                        return "IO-2 open";
                    case 0x54:
                        return "IO-3 close";
                    case 0x55:
                        return "IO-3 open";
                    case 0x56:
                        return "IO-4 close";
                    case 0x57:
                        return "IO-4 open";
                    case 0x60:
                        return "Begin charge";
                    case 0x61:
                        return "End charge";
                    case 0x88:
                        return "Heartbeat";
                    case 0x91:
                        return "Going to sleep mode";
                    case 0x92:
                        return "Wake up from sleep mode";
                    case 0xAA:
                        return "Data interval";
                    default:
                        return String.Format("Event: 0x{0:X2}", this.EventType);
                }
            }
        }

        public TrackingReportModel()
        {
        }

        public TrackingReportModel(Tracking t)
            : base(t)
        {
        }
    }
}