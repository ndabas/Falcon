using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FalconWeb.Models
{
    public class User
    {
        public Guid ID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
        
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        public string Address { get; set; }

        [DisplayName("Mobile Number")]
        [StringLength(16)]
        public string MobileNumber { get; set; }

        [DisplayName("Emergency Mobile Number")]
        [StringLength(16)]
        public string EmergencyNumber { get; set; }

        [DisplayName("Emergency Message")]
        public string EmergencyMessage { get; set; }

        public virtual ICollection<Device> Devices { get; set; }

        public virtual ICollection<GeoArea> GeoAreas { get; set; }

        public User()
        {
        }

        public User(User user)
        {
            this.Address = user.Address;
            this.CompanyName = user.CompanyName;
            this.Devices = user.Devices;
            this.EmergencyMessage = user.EmergencyMessage;
            this.EmergencyNumber = user.EmergencyNumber;
            this.ID = user.ID;
            this.MobileNumber = user.MobileNumber;
            this.Name = user.Name;
        }
    }
}