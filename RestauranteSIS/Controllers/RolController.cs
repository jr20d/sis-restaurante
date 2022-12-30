using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestauranteSIS.Models;
using RestauranteSIS.Operaciones;
using Newtonsoft.Json;

namespace RestauranteSIS.Controllers
{
    public class RolController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Index", "Sesion");
            }
            string Datos = Session["usuario"].ToString();
            List<Sesion> DatosSesion = JsonConvert.DeserializeObject<List<Sesion>>(Datos);

            int Acceso = 1;
            DatosSesion.ForEach((item) => {
                Acceso = item.Acceso;
            });

            if (Acceso == 1)
            {
                return RedirectToAction("Index", "Principal");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public string Agregar(Rol RolDTO)
        {
            string Mensaje;
            Validacion ObjValidacion = new Validacion();
            Rol ObjROl = new Rol();
            if (RolDTO != null)
            {
                if (ObjValidacion.HayContenido(RolDTO.Nombre) && ObjValidacion.EsNumerico(RolDTO.Salario.ToString()) && ObjValidacion.EsNumerico(RolDTO.Acceso.ToString()))
                {
                    if (RolDTO.Acceso >= 1 && RolDTO.Acceso <= 3)
                    {
                        Mensaje = ObjROl.AgregarRol(RolDTO);
                    }
                    else
                    {
                        Mensaje = "Error en el acceso asignado al rol";
                    }
                }
                else
                {
                    Mensaje = "Verificar que haya ingresado el nombre y/o agregado el monto del salario";
                }
            }
            else
            {
                Mensaje = "Error al enviar los datos";
            }

            return Mensaje;
        }

        [HttpPost]
        public string Editar(Rol RolDTO)
        {
            string Mensaje;
            Validacion ObjValidacion = new Validacion();
            Rol ObjROl = new Rol();
            if (RolDTO != null)
            {
                if (ObjValidacion.EsNumerico(RolDTO.ID.ToString()) && ObjValidacion.EsNumerico(RolDTO.Salario.ToString()))
                {
                    Mensaje = ObjROl.EditarRol(RolDTO);
                }
                else
                {
                    Mensaje = "Verificar que haya agregado el monto del salario";
                }
            }
            else
            {
                Mensaje = "Error al enviar los datos";
            }
            
            return Mensaje;
        }
    }
}