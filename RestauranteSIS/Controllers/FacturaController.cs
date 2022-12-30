using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestauranteSIS.Models;
using RestauranteSIS.Operaciones;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Reporting.WebForms;

namespace RestauranteSIS.Controllers
{
    public class FacturaController : Controller
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
        public string Correlativo(int TipoID)
        {
            Validacion Validar = new Validacion();
            if (Validar.EsNumerico(TipoID.ToString()))
            {                
                Factura ObjFactura = new Factura();
                int Numero = ObjFactura.BuscarCorrelativo(TipoID);

                if (Numero > -1)
                {
                    return Validar.GenerarCorrelativo(Numero, 12);
                }
                else
                {
                    return "ERROR";
                }
            }
            else
            {
                return "ERROR";
            }
        }

        [HttpPost]
        public string Facturar(Factura FacturaDTO, string DatosDetalle)
        {
            string Datos = Session["usuario"].ToString();
            List<Sesion> DatosSesion = JsonConvert.DeserializeObject<List<Sesion>>(Datos);
            List<DetalleFactura> Detalles = JsonConvert.DeserializeObject<List<DetalleFactura>>(DatosDetalle);

            string Mensaje = "";

            DatosSesion.ForEach((d) => {
                FacturaDTO.EmpleadoID = d.UsuarioID;
            });

            Factura ObjFactura = new Factura();
            DetalleFactura ObjDetalle = new DetalleFactura();

            int ID = ObjFactura.Facturar(FacturaDTO);

            if (ID > 0)
            {
                Detalles.ForEach((d) => {
                    if (Mensaje == "OK" || Mensaje == "")
                    {
                        d.FacturaID = ID;
                        Mensaje = ObjDetalle.GuardarDetalle(d);
                    }                    
                });
                return ID.ToString();
            }
            else
            {
                return "Error";
            }
        }

        //Crear factura
        [HttpPost]
        public string GenerarFactura(int ID)
        {
            LocalReport Factura = new LocalReport();
            Validacion Validar = new Validacion();
            string ruta = Path.Combine(Server.MapPath("~/Recursos/facturas/"), "Facturas.rdlc");
            if (System.IO.File.Exists(ruta))
            {
                Factura.ReportPath = ruta;
            }
            else
            {
                RedirectToAction("Index");
            }            

            Factura.Refresh();
            Factura ObjFactura = new Factura();
            Factura.DataSources.Add(new ReportDataSource("DataFactura", ObjFactura.VerFactura(ID)));

            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            string Info =
                "<DeviceInfo>" +
                "<OutputFormat>PDF</OutputFormat>" +
                "<PageWidth>8.5in</PageWidth>" +
                "<PageHeight>11in</PageHeight>" +
                "<MarginTop>0.5in</MarginTop>" +
                "<MarginLeft>1in</MarginLeft>" +
                "<MarginRight>1in</MarginRight>" +
                "<MarginBottom>0.5in</MarginBottom>" +
                "<EmbedFonts>None</EmbedFonts>" +
                "</DeviceInfo>";

            try
            {
                byte[] DataReporte = Factura.Render("PDF", Info, out mimeType, out encoding, out extension, out streamids, out warnings);
                string rutaPDF = Path.Combine(Server.MapPath("~/Recursos/facturas/"), "Factura-" + ID + ".pdf");
                using (FileStream fs = new FileStream(rutaPDF, FileMode.Create))
                {
                    fs.Write(DataReporte, 0, DataReporte.Length);
                }
                Factura.Dispose();
                return Validar.GenerarRutaArchivo(rutaPDF);
            }
            catch
            {
                return "ERROR";
            }
        }

        //Mostrar Facturas
        public string Facturas()
        {
            Factura ObjFactura = new Factura();
            return JsonConvert.SerializeObject(ObjFactura.FacturasVM());
        }
    }
}