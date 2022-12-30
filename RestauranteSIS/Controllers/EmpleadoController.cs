using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Security.Cryptography;
using RestauranteSIS.Models;
using RestauranteSIS.Operaciones;
using Newtonsoft.Json;
using Microsoft.Web;
using System.IO;
using Microsoft.Reporting.WebForms;

namespace RestauranteSIS.Controllers
{
    public class EmpleadoController : Controller
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
        public string Agregar(EmpleadoSucursal.RelEmpleadoSucursal EmpleadoSucursalDTO, Empleado EmpleadoDTO)
        {
            string Mensaje;
            Validacion ObjValidar = new Validacion();
            if (EmpleadoDTO != null && EmpleadoSucursalDTO != null)
            {
                if (EmpleadoDTO.Email != null)
                {
                    if (!ObjValidar.ValidarCorreo(EmpleadoDTO.Email))
                    {
                        return "Error en el correo";
                    }
                }
                else
                {
                    EmpleadoDTO.Email = "";
                }
                byte[] Sal = GeneradorPassword.GenerarSal();
                byte[] Hash = GeneradorPassword.GenerarHash(EmpleadoDTO.Password, Sal);
                EmpleadoDTO.SalPassword = Sal;
                EmpleadoDTO.HashPassword = Hash;
                EmpleadoSucursalDTO.EstadoID = 1;
                Mensaje = EmpleadoDTO.Agregar(EmpleadoDTO);
                if (ObjValidar.EsNumerico(Mensaje))
                {
                    int id = Int32.Parse(Mensaje);
                    EmpleadoSucursalDTO.EmpleadoID = Int32.Parse(Mensaje);
                    EmpleadoSucursal ObjEmpleadoSucursal = new EmpleadoSucursal();
                    Mensaje = ObjEmpleadoSucursal.Agregar(EmpleadoSucursalDTO);
                }
            }
            else
            {
                Mensaje = "Error al eviar los datos";
            }
            return Mensaje;
        }

        [HttpPost]
        public string SubirFoto()
        {
            string Mensaje;
            HttpPostedFileBase Archivo = HttpContext.Request.Files["Foto"];

            if (Archivo != null)
            {                
                if (Archivo.ContentType == "image/jpeg" || Archivo.ContentType == "image/png")
                {
                    Validacion Validar = new Validacion();
                    string foto = Path.Combine(HttpContext.Server.MapPath("~/Recursos/fotoempleados/"), HttpContext.Request["Nombre"] + ".jpg");
                    Archivo.SaveAs(foto);
                    Mensaje = Validar.GenerarRutaArchivo(foto);
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
        public string Reporte(Empleado EmpleadoDTO, Estado EstadoDTO)
        {
            LocalReport RepEmpleado = new LocalReport();
            Validacion Validar = new Validacion();
            string ruta = Path.Combine(Server.MapPath("~/Recursos/reportes/"), "Empleado.rdlc");
            if (System.IO.File.Exists(ruta))
            {
                RepEmpleado.ReportPath = ruta;
            }
            else
            {
                RedirectToAction("Index");
            }

            RepEmpleado.Refresh();

            RepEmpleado.SetParameters(new ReportParameter("NombreEmpleado", EmpleadoDTO.Nombre));
            RepEmpleado.SetParameters(new ReportParameter("Estado", EstadoDTO.Nombre));
            RepEmpleado.SetParameters(new ReportParameter("Nacimiento", EmpleadoDTO.FechaNacimiento.ToString()));

            EmpleadoSucursal ObjRel = new EmpleadoSucursal();
            RepEmpleado.DataSources.Add(new ReportDataSource("Registros", ObjRel.ListaDatosEmpleadoSucursal(EmpleadoDTO.ID)));

            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            string Info = 
                "<DeviceInfo>"+
                "<OutputFormat>PDF</OutputFormat>"+
                "<PageWidth>8.5in</PageWidth>"+
                "<PageHeight>11in</PageHeight>"+
                "<MarginTop>0.5in</MarginTop>"+
                "<MarginLeft>1in</MarginLeft>"+
                "<MarginRight>1in</MarginRight>"+
                "<MarginBottom>0.5in</MarginBottom>"+
                "<EmbedFonts>None</EmbedFonts>"+
                "</DeviceInfo>";
            
            try
            {
                byte[] DataReporte = RepEmpleado.Render("PDF", Info, out mimeType, out encoding, out extension, out streamids, out warnings);
                string rutaPDF = Path.Combine(Server.MapPath("~/Recursos/reportes/"), EmpleadoDTO.Usuario + ".pdf");
                using (FileStream fs = new FileStream(rutaPDF, FileMode.Create))
                {
                    fs.Write(DataReporte, 0, DataReporte.Length);
                }
                RepEmpleado.Dispose();
                return Validar.GenerarRutaArchivo(rutaPDF);
            }
            catch
            {
                return null;
            }            
        }

        [HttpPost]
        public string Editar(EmpleadoSucursal.RelEmpleadoSucursal RelDTO, Empleado EmpleadoDTO, bool ActualizarREl)
        {
            string Mensaje;
            Validacion ObjValidar = new Validacion();
            if (EmpleadoDTO != null && RelDTO != null)
            {
                if (EmpleadoDTO.Email != null)
                {
                    if (!ObjValidar.ValidarCorreo(EmpleadoDTO.Email))
                    {
                        return "Error en el correo";
                    }
                }
                else
                {
                    EmpleadoDTO.Email = "";
                }

                if (EmpleadoDTO.Password != null)
                {
                    byte[] Sal = GeneradorPassword.GenerarSal();
                    byte[] Hash = GeneradorPassword.GenerarHash(EmpleadoDTO.Password, Sal);
                    EmpleadoDTO.SalPassword = Sal;
                    EmpleadoDTO.HashPassword = Hash;                    
                }

                EmpleadoSucursal ObjEmpleadoSucursal = new EmpleadoSucursal();
                Mensaje = EmpleadoDTO.Actualizar(EmpleadoDTO);
                if (ActualizarREl == true)
                {
                    if (Mensaje == "OK")
                    {
                        if (RelDTO.EstadoID == 1)
                        {
                            RelDTO.EstadoID = 2;
                            Mensaje = ObjEmpleadoSucursal.Actualizar(RelDTO);
                            RelDTO.EstadoID = 1;
                        }
                        else
                        {
                            RelDTO.EstadoID = 2;
                            Mensaje = ObjEmpleadoSucursal.Actualizar(RelDTO);
                        }

                        if (Mensaje == "OK")
                        {
                            Mensaje = ObjEmpleadoSucursal.Agregar(RelDTO);
                        }
                    }
                }                            
            }
            else
            {
                Mensaje = "Error al eviar los datos";
            }
            return Mensaje;
        }

        //Cambiar contraseña
        public string CambiarPassword(string Nuevo, string Anterior)
        {
            if (Nuevo.Trim().Length > 0 && Anterior.Trim().Length > 0)
            {
                Sesion ObjSesion = new Sesion();
                Empleado ObjEmpleado = new Empleado();
                List<Sesion.Credenciales> ListaCredenciales = ObjSesion.CredencialesVM(Int32.Parse(Session["id"].ToString()));

                string Mensaje = "";

                ListaCredenciales.ForEach((c) => { 
                    if (Convert.ToBase64String(c.HashPassword) == Convert.ToBase64String(GeneradorPassword.GenerarHash(Anterior, c.SalPassword)))
                    {
                        if (Convert.ToBase64String(GeneradorPassword.GenerarHash(Nuevo, c.SalPassword)) != Convert.ToBase64String(c.HashPassword))
                        {
                            byte[] Sal = GeneradorPassword.GenerarSal();
                            byte[] Hash = GeneradorPassword.GenerarHash(Nuevo, Sal);
                            Mensaje = ObjEmpleado.CambiarPassword(c.DatosSesion.UsuarioID, Sal, Hash);
                        }
                        else
                        {
                            Mensaje = "Error: la nueva contraseña ingresada es igual a la anterior";
                        }                        
                    }
                    else
                    {
                        Mensaje = "La contraseña anterior no es correcta";
                    }
                });
                return Mensaje;
            }
            else
            {
                return "Ingresar la contraseña nueva y la anterior";
            }
        }
    }
}