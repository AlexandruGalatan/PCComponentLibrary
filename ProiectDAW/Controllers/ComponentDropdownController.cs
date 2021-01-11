using ProiectDAW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProiectDAW.Controllers
{
    public class ComponentDropdownController : Controller
    {
        private ComponentStuff db = new ComponentStuff();

        [ChildActionOnly]
        public ActionResult DropDown()
        {
            var categories = db.Categories.ToList();
            return PartialView("_CategoryDropDown", categories);
        }
    }
}