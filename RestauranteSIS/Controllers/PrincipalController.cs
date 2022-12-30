using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestauranteSIS.Models;

namespace RestauranteSIS.Controllers
{
    public class PrincipalController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Index", "Sesion");
            }
            Sucursal ObjSUcursal = new Sucursal();            

            return View(ObjSUcursal.SucursalesIngreso());
        }

    }
}