using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

using RestauranteSIS.Models;

namespace RestauranteSIS.Controllers
{
    public class MesasController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Index", "Sesion");
            }
            return View();
        }

        [HttpGet]
        public string CantidadMesas()
        {   
            if (Session["Mesas"] == null)
            {
                string Datos = Session["usuario"].ToString();
                List<Sesion> DatosSesion = JsonConvert.DeserializeObject<List<Sesion>>(Datos);
                Mesa MesaVM = new Mesa();
                DatosSesion.ForEach((d) => {
                    Session["Mesas"] = MesaVM.ListaMesas(d.SucursalID);
                });
            }
            else
            {
                string Datos = Session["usuario"].ToString();
                List<Sesion> DatosSesion = JsonConvert.DeserializeObject<List<Sesion>>(Datos);
                Mesa MesaVM = new Mesa();

                string DataMesas = JsonConvert.SerializeObject(Session["Mesas"]);

                List<Mesa> Mesas = JsonConvert.DeserializeObject<List<Mesa>>(DataMesas);

                int SucursalID = 0;

                Mesas.ForEach((m) => {
                    SucursalID = m.SucursalID;
                });

                DatosSesion.ForEach((d) => {
                    if (SucursalID != 0 && SucursalID != d.SucursalID)
                    {
                        Session["Mesas"] = MesaVM.ListaMesas(d.SucursalID);
                    }                    
                });
            }
            
            return JsonConvert.SerializeObject(Session["Mesas"]);
        }

        [HttpPost]
        public string AsignarMesa(string Mesas)
        {
            if (Mesas != null)
            {
                Session["Mesas"] = JsonConvert.DeserializeObject(Mesas);
                return JsonConvert.SerializeObject(Session["Mesas"]);
            }
            else
            {
                return "Error al enviar los datos";
            }
        }
    }
}