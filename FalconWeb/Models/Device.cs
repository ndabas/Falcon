using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FalconWeb.Models
{
    public class Device
    {
        public int ID { get; set; }

        [Required]
        [StringLength(16)]
        public string IMEI { get; set; }

        [DisplayName("Mobile Number")]
        [StringLength(16)]
        public string MobileNumber { get; set; }

        [DisplayName("Vehicle Number")]
        [StringLength(16)]
        public string VehicleNumber { get; set; }

        [StringLength(30)]
        public string Nickname { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual GeoArea GeoArea { get; set; }
    }
}