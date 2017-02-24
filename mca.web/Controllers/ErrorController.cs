using mca.web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAapp.Controllers
{
    [NotRestricted]
    public class ErrorController : Controller
    {
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("404");
        }

        public ActionResult InternalServerError()
        {
            Response.StatusCode = 500;
            return View("500");
        }
    }
}