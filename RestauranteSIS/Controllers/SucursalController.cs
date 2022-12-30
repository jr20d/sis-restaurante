using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RestauranteSIS.Models;
using RestauranteSIS.Operaciones;

namespace RestauranteSIS.Controllers
{
    public class SucursalController : Controller
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
        public string Agregar(Sucursal SucursalDTO, int CantMesas)
        {
            string Mensaje;
            Validacion ObjValidar = new Validacion();
            Sucursal ObjSucursal = new Sucursal();
            Mesa ObjMesa = new Mesa();
            if (SucursalDTO != null && CantMesas > 0)
            {
                if (ObjValidar.HayContenido(SucursalDTO.Nombre) && ObjValidar.HayContenido(SucursalDTO.Telefono) && ObjValidar.HayContenido(SucursalDTO.Direccion) && ObjValidar.EsNumerico(SucursalDTO.Eliminar.ToString()) && SucursalDTO.Telefono.Length == 9)
                {
                    Mensaje = ObjSucursal.AgregarSucursal(SucursalDTO);
                    if (ObjValidar.EsNumerico(Mensaje))
                    {
                        int SucursalID = Convert.ToInt32(Mensaje);
                        for (int i = 1; i <= CantMesas; i++)
                        {                            
                            Mensaje = ObjMesa.AgregarMesa(i, SucursalID);
                        }
                    }
                    else
                    {
                        Mensaje = "Error al agregar mesas";
                    }
                }
                else
                {
                    Mensaje = "Verifica que todos los campos estén completados";
                }
            }
            else
            {
                Mensaje = "Ha ocurrido un error al enviar los datos";
            }

            return Mensaje;
        }

        [HttpPost]
        public string Editar(Sucursal SucursalDTO)
        {
            string Mensaje;
            Validacion ObjValidar = new Validacion();
            Sucursal ObjSucursal = new Sucursal();

            if (SucursalDTO != null)
            {
                if (ObjValidar.EsNumerico(SucursalDTO.ID.ToString()) && ObjValidar.HayContenido(SucursalDTO.Telefono) && SucursalDTO.Telefono.Length == 9)
                {
                    Mensaje = ObjSucursal.EditarSucursal(SucursalDTO);
                }
                else
                {
                    Mensaje = "Completar el campo del telefono";
                }
            }
            else
            {
                Mensaje = "Ha ocurrido un error al enviar los datos";
            }

            return Mensaje;
        }

        [HttpPost]
        public string Eliminar(int ID)
        {
            string Mensaje;
            Validacion ObjValidar = new Validacion();
            Sucursal ObjSucursal = new Sucursal();

            if (ObjValidar.EsNumerico(ID.ToString()) && ID > 0)
            {
                Mensaje = ObjSucursal.QuitarSucursal(ID);
            }
            else
            {
                Mensaje = "Ha ocurrido un error al enviar los datos";
            }

            return Mensaje;
        }
    }
}