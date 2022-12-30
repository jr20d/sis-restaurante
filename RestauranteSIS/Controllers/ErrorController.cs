using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestauranteSIS.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        [Route("/Error/")]
        public ActionResult Index()
        {
            return RedirectToAction("PaginaNoEncontrada");
        }

        [HttpGet]
        public ActionResult PaginaNoEncontrada()
        {
            ViewData["Title"] = "Error";
            return PartialView("Index");
        }
    }
}