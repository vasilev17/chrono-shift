using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ChronoShift.Models
{
    public class Login
    {

        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberPassword { get; set; }

    }
}
