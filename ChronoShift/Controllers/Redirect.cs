using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChronoShift.Controllers;

namespace ChronoShift.Controllers
{
    public class Redirect : Controller
    {
        public IActionResult RedirectToIndex()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
