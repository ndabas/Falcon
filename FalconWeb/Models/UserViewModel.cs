using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace FalconWeb.Models
{
    public class UserViewModel : User
    {
        public string UserName { get; set; }
        public string Email { get; set; }

        [DisplayName("Admistrator Access")]
        public bool IsAdmin { get; set; }

        public UserViewModel()
        {
        }

        public UserViewModel(User user)
            : base(user)
        {
        }
    }
}