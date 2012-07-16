using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FalconWeb.Models
{
    public class GeoArea
    {
        public virtual User User { get; set; }
        public int ID { get; set; }
        
        [Required]
        public string LatLongs { get; set; }
        
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Device> Devices { get; set; }
    }
}