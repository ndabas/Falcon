using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FalconWeb.Models
{
    public class GPRMCData
    {
        public DateTime TimeStamp { get; set; }
        public char DataValid { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public float GroundSpeed { get; set; }
        public float Bearing { get; set; }
    }
}