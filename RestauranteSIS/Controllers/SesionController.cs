using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestauranteSIS.Models;
using Newtonsoft.Json;
using RestauranteSIS.Operaciones;

namespace RestauranteSIS.Controllers
{
    public class SesionController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return PartialView();
            }
            else
            {
                return RedirectToAction("Index", "Principal");
            }
        }

        [HttpPost]
        public string Iniciar(string Usuario, string Password)
        {
            if (Usuario != null && Password != null)
            {
                Sesion ObjSesion = new Sesion();
                int ID = ObjSesion.BuscarID(Usuario);
                if (ID > 0)
                {
                    List<Sesion.Credenciales> Credencial = ObjSesion.CredencialesVM(ID);
                    byte[] Resultado;
                    byte[] HashPassword;
                    var Respuesta = "";
                    Credencial.ForEach((item) => {
                        Resultado = GeneradorPassword.GenerarHash(Password, item.SalPassword);
                        HashPassword = item.HashPassword;
                        if (Convert.ToBase64String(Resultado) == Convert.ToBase64String(item.HashPassword))
                        {
                            if (item.DatosSesion.EstadoID == 1)
                            {
                                List<Sesion> DatosSesion = new List<Sesion>();
                                DatosSesion.Add(item.DatosSesion);
                                Session["usuario"] = JsonConvert.SerializeObject(DatosSesion);
                                Session["nombre"] = item.DatosSesion.Nombre;
                                Session["foto"] = item.DatosSesion.Foto;
                                Session["acceso"] = item.DatosSesion.Acceso;
                                Session["sucursalID"] = item.DatosSesion.SucursalID;
                                Session["id"] = item.DatosSesion.UsuarioID;
                                Respuesta = "OK";
                            }
                            else
                            {
                                Respuesta = "Su cuenta está inhabilitada";
                            }                            
                        }
                        else
                        {
                            Respuesta = "Contraseña incorrecta";
                        }
                    });
                    return Respuesta;
                }
                else
                {
                    return "Error en el nombre de usuario";
                }                
            }
            else
            {
                return ("Ingresar usuario y contraseña");
            }
        }
        [HttpPost]
        public string Salir()
        {
            Session["usuario"] = null;
            Session["id"] = null;
            Session["nombre"] = null;
            Session["foto"] = null;
            Session["acceso"] = null;
            Session["sucursalID"] = null;
            if (Session["usuario"] == null)
            {
                return "OK";
            }
            else
            {
                return "ERROR";
            }
        }
    }
}