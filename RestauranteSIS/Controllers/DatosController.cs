using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data;
using RestauranteSIS.Models;

namespace RestauranteSIS.Controllers
{
    public class DatosController : Controller
    {
        [HttpGet]
        public string MostrarEmpleados()
        {
            List<EmpleadoSucursal> EmpleadoSucursalVM = new List<EmpleadoSucursal>();
            EmpleadoSucursal ObjEmpleadoSucursal = new EmpleadoSucursal();
            Empleado ObjEmpleado = new Empleado();
            Sucursal ObjSucursal = new Sucursal();
            Rol ObjRol = new Rol();
            Estado ObjEstado = new Estado();
            EmpleadoSucursalVM.Add(new EmpleadoSucursal() { 
                RelEmpleadoSucursales = ObjEmpleadoSucursal.ListaRelEmpleadoSucursal(),
                Empleados = ObjEmpleado.ListaEmpleados(),
                Sucursales = ObjSucursal.ListaSucursales(),
                Roles = ObjRol.ListaRoles(),
                Estados = ObjEstado.ListaEstados()

            });
            return JsonConvert.SerializeObject(EmpleadoSucursalVM);
        }

        [HttpGet]
        public string MostrarRoles()
        {
            Rol ObjRol = new Rol();
            string Registros = JsonConvert.SerializeObject(ObjRol.ListaRoles());
            return Registros;
        }

        [HttpGet]
        public string MostrarSucursales()
        {
            Sucursal ObjSucursal = new Sucursal();
            string Registros = JsonConvert.SerializeObject(ObjSucursal.ListaSucursales());
            return Registros;
        }

        [HttpGet]
        public string MostrarCategorias()
        {
            Categoria ObjCategoria = new Categoria();
            string Registros = JsonConvert.SerializeObject(ObjCategoria.ListaCategorias());

            return Registros;
        }

        [HttpGet]
        public string MostrarRelCategoriaMenu()
        {
            Categoria CategoriaVM = new Categoria();
            Menu MenuVM = new Menu();
            List<Menu.MenuCategoriaVM> ListaMenuCategoriaVM = new List<Menu.MenuCategoriaVM>();

            ListaMenuCategoriaVM.Add(new Menu.MenuCategoriaVM() { 
                ListaMenu = MenuVM.ListaMenu(),
                Categorias = CategoriaVM.ListaCategorias()
            });

            string Registros = JsonConvert.SerializeObject(ListaMenuCategoriaVM);

            return Registros;
        }

        [HttpGet]
        public string ClientesReservacion()
        {
            List<Cliente.DataClienteMenu> ListaVM = new List<Cliente.DataClienteMenu>();
            Cliente ObjCliente = new Cliente();
            Reservacion ObjReservacion = new Reservacion();
            Menu ObjMenu = new Menu();
            ListaVM.Add(new Cliente.DataClienteMenu() { 
                Clientes = ObjCliente.ListaCliente(),
                Reservaciones = ObjReservacion.ReservacionesVM(),
                Platos = ObjMenu.ListaMenu()
            });
            return JsonConvert.SerializeObject(ListaVM);
        }
    }
}