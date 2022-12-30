using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestauranteSIS.Models;
using RestauranteSIS.Operaciones;

namespace RestauranteSIS.Controllers
{
    public class ClienteController : Controller
    {
        [HttpPost]
        public string Agregar(Cliente ClienteDTO)
        {
            string Resultado;
            if (ClienteDTO != null)
            {
                Validacion Validar = new Validacion();
                if (Validar.HayContenido(ClienteDTO.Nombre) && ClienteDTO.Nombre.Trim().Length <= 75 && Validar.HayContenido(ClienteDTO.NIT) && ClienteDTO.NIT.Trim().Length == 17)
                {
                    Cliente ObjCliente = new Cliente();
                    Resultado = ObjCliente.Agregar(ClienteDTO);
                }
                else
                {
                    Resultado = "Datos incompletos";
                }
            }
            else
            {
                Resultado = "Error al enviar los datos";
            }
            return Resultado;
        }
    }
}