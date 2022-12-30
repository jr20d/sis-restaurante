using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

using RestauranteSIS.Models;
using RestauranteSIS.Operaciones;
using Newtonsoft.Json;

namespace RestauranteSIS.Controllers
{
    public class MenuController : Controller
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

        [HttpGet]
        public ActionResult Categoria()
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
        public string AgregarCategoria(Categoria CategoriaDTO)
        {
            string Mensaje;
            Validacion ObjValidacion = new Validacion();
            if (CategoriaDTO != null)
            {
                if (ObjValidacion.HayContenido(CategoriaDTO.Nombre) && CategoriaDTO.Nombre.Length <= 15 && ObjValidacion.HayContenido(CategoriaDTO.Imagen))
                {
                    Categoria ObjCategoria = new Categoria();
                    Mensaje = ObjCategoria.Agregar(CategoriaDTO);
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
        public string SubirImagen()
        {
            string Mensaje;
            HttpPostedFileBase Archivo = HttpContext.Request.Files["Archivo"];

            if (Archivo != null)
            {
                if (Archivo.ContentType == "image/jpeg" || Archivo.ContentType == "image/png")
                {
                    Validacion Validar = new Validacion();
                    string img = Path.Combine(HttpContext.Server.MapPath("~/Recursos/menu/"), HttpContext.Request["Nombre"] + ".jpg");
                    Archivo.SaveAs(img);
                    Mensaje = Validar.GenerarRutaArchivo(img);
                }
                else
                {
                    Mensaje = "ERROR";
                }
            }
            else
            {
                Mensaje = "ERROR";
            }
            return Mensaje;
        }

        [HttpPost]
        public string Agregar(Menu MenuDTO)
        {
            string Mensaje;

            if (MenuDTO != null)
            {
                Validacion Validar = new Validacion();

                if (Validar.HayContenido(MenuDTO.Nombre) && MenuDTO.Nombre.Length <= 35 && Validar.EsNumerico(MenuDTO.Precio.ToString()) && Validar.EsNumerico(MenuDTO.Descuento.ToString()) && Validar.EsNumerico(MenuDTO.CategoriaID.ToString()) && Validar.HayContenido(MenuDTO.Descripcion) && Validar.EsNumerico(MenuDTO.Estado.ToString()) && Validar.HayContenido(MenuDTO.Imagen))
                {
                    Menu ObjMenu = new Menu();
                    Mensaje = ObjMenu.Agregar(MenuDTO);
                }
                else
                {
                    Mensaje = "ERROR!. Datos incompletos";
                }
            }
            else
            {
                Mensaje = "ERROR";
            }

            return Mensaje;
        }

        //Quitar elemento del menú
        [HttpPost]
        public string Quitar(int ID)
        {
            string Mensaje;
            Validacion Validar = new Validacion();
            if (Validar.HayContenido(ID.ToString()) && Validar.EsNumerico(ID.ToString()))
            {
                Menu ObjMenu = new Menu();
                Mensaje = ObjMenu.Quitar(ID);           
            }
            else
            {
                Mensaje = "Error al enviar los datos";
            }
            return Mensaje;
        }

        //Quitar imagen del servidor
        [HttpPost]
        public string QuitarImagen(string Ruta)
        {
            string ModificarRuta, Mensaje, RutaServidor;
            Validacion Validar = new Validacion();
            if (Validar.HayContenido(Ruta))
            {
                ModificarRuta = Ruta.Replace("..", "~");
                RutaServidor = Server.MapPath(ModificarRuta);
                if (System.IO.File.Exists(RutaServidor))
                {
                    try
                    {
                        System.IO.File.Delete(Server.MapPath(ModificarRuta));
                        Mensaje = "OK";
                    }
                    catch
                    {
                        Mensaje = "ERROR";
                    }
                }
                else
                {
                    Mensaje = "OK";
                }
            }
            else
            {
                Mensaje = "Error al eliminar la imágen";
            }

            return Mensaje;
        }

        //Actualizar datos del elemento del menú
        public string Editar(Menu MenuDTO)
        {
            string Mensaje;
            if (MenuDTO != null)
            {
                Validacion Validar = new Validacion();
                if (Validar.HayContenido(MenuDTO.ID.ToString()) && Validar.EsNumerico(MenuDTO.ID.ToString()) && Validar.EsNumerico(MenuDTO.Precio.ToString()) && Validar.EsNumerico(MenuDTO.Descuento.ToString()) && Validar.EsNumerico(MenuDTO.CategoriaID.ToString()) && Validar.HayContenido(MenuDTO.Descripcion) && Validar.EsNumerico(MenuDTO.Estado.ToString()) && Validar.HayContenido(MenuDTO.Imagen))
                {
                    Menu ObjMenu = new Menu();
                    Mensaje = ObjMenu.Editar(MenuDTO);
                }
                else
                {
                    Mensaje = "ERROR!. Datos incompletos";
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