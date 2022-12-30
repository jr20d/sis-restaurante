using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RestauranteSIS.Models
{
    public class Sucursal : ConexionDBC
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public int Eliminar { get; set; }

        public partial class SucursalVM
        {
            public int ID { get; set; }
            public string Nombre { get; set; }
            public double Ingreso { get; set; }
        }

        private SqlConnection SqlCn;
        private SqlCommand SqlCmd;

        private SqlParameter ParamSucursalID;
        private SqlParameter ParamSucursalNombre;
        private SqlParameter ParamSucursalTelefono;
        private SqlParameter ParamSucursalDireccion;
        private SqlParameter ParamSucursalEliminar;

        private void CrearParametros()
        {
            ParamSucursalID = new SqlParameter();
            ParamSucursalID.ParameterName = "id";
            ParamSucursalID.SqlDbType = SqlDbType.Int;
            ParamSucursalID.Direction = ParameterDirection.Input;

            ParamSucursalNombre = new SqlParameter();
            ParamSucursalNombre.ParameterName = "nombre";
            ParamSucursalNombre.SqlDbType = SqlDbType.VarChar;
            ParamSucursalNombre.Size = 50;
            ParamSucursalNombre.Direction = ParameterDirection.Input;

            ParamSucursalTelefono = new SqlParameter();
            ParamSucursalTelefono.ParameterName = "telefono";
            ParamSucursalTelefono.SqlDbType = SqlDbType.VarChar;
            ParamSucursalTelefono.Size = 9;
            ParamSucursalTelefono.Direction = ParameterDirection.Input;

            ParamSucursalDireccion = new SqlParameter();
            ParamSucursalDireccion.ParameterName = "direccion";
            ParamSucursalDireccion.SqlDbType = SqlDbType.Text;
            ParamSucursalDireccion.Direction = ParameterDirection.Input;

            ParamSucursalEliminar = new SqlParameter();
            ParamSucursalEliminar.ParameterName = "eliminar";
            ParamSucursalEliminar.SqlDbType = SqlDbType.Int;
            ParamSucursalEliminar.Direction = ParameterDirection.Input;
        }

        public List<Sucursal> ListaSucursales()
        {
            List<Sucursal> Registros = new List<Sucursal>();
            string MsgError;
            SqlCn = Conectar(out MsgError);
            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlDataReader Datos;
                    SqlCmd = new SqlCommand("sp_listaSucursales", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    Datos = SqlCmd.ExecuteReader();
                    while (Datos.Read())
                    {
                        Registros.Add(new Sucursal()
                        {
                            ID = Int32.Parse(Datos["id"].ToString()),
                            Nombre = Datos["nombre"].ToString(),
                            Telefono = Datos["telefono"].ToString(),
                            Direccion = Datos["direccion"].ToString(),
                            Eliminar = Int32.Parse(Datos["eliminar"].ToString())
                        });
                    }
                    Datos.Close();
                }
                catch
                {
                    Registros = null;
                }
                SqlCn.Close();
            }
            return Registros;
        }

        public string AgregarSucursal(Sucursal SucursalDTO)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_agregarSucursal", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamSucursalNombre.Value = SucursalDTO.Nombre;
                    ParamSucursalTelefono.Value = SucursalDTO.Telefono;
                    ParamSucursalDireccion.Value = SucursalDTO.Direccion;
                    ParamSucursalEliminar.Value = SucursalDTO.Eliminar;
                    SqlCmd.Parameters.Add(ParamSucursalNombre);
                    SqlCmd.Parameters.Add(ParamSucursalTelefono);
                    SqlCmd.Parameters.Add(ParamSucursalDireccion);
                    SqlCmd.Parameters.Add(ParamSucursalEliminar);

                    SqlDataReader Dato = SqlCmd.ExecuteReader();

                    if (Dato.Read())
                    {
                        Mensaje = Dato["id"].ToString();
                    }
                }
                catch (Exception Ex)
                {
                    Mensaje = $"ERROR: {Ex.Message.ToString()}";
                }
                SqlCn.Close();
            }

            return Mensaje;
        }

        public string EditarSucursal(Sucursal SucursalDTO)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_actualizarSucursal", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamSucursalID.Value = SucursalDTO.ID;
                    ParamSucursalTelefono.Value = SucursalDTO.Telefono;
                    SqlCmd.Parameters.Add(ParamSucursalID);
                    SqlCmd.Parameters.Add(ParamSucursalTelefono);

                    Mensaje = (SqlCmd.ExecuteNonQuery() > 0) ? "OK" : "ERROR: No se pudieron actualizar los datos de la sucursal";
                }
                catch (Exception Ex)
                {
                    Mensaje = $"ERROR: {Ex.Message.ToString()}";
                }
                SqlCn.Close();
            }

            return Mensaje;
        }

        public string QuitarSucursal(int SucursalID)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_ocultarSucursal", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamSucursalID.Value = SucursalID;
                    SqlCmd.Parameters.Add(ParamSucursalID);

                    Mensaje = (SqlCmd.ExecuteNonQuery() > 0) ? "OK" : "ERROR: No se pudo quitar la sucursal";
                }
                catch (Exception Ex)
                {
                    Mensaje = $"ERROR: {Ex.Message.ToString()}";
                }
                SqlCn.Close();
            }

            return Mensaje;
        }

        //Mostrar sucursales con sus ingresos
        public List<SucursalVM> SucursalesIngreso()
        {
            List<SucursalVM> Lista = new List<SucursalVM>();
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_ingresos", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;

                    SqlDataReader Datos = SqlCmd.ExecuteReader();

                    while (Datos.Read())
                    {
                        Lista.Add(new SucursalVM() { 
                            ID = Int32.Parse(Datos["id"].ToString()),
                            Nombre = Datos["nombre"].ToString(),
                            Ingreso = (Datos["ingreso"].ToString() != "") ? Double.Parse(Datos["ingreso"].ToString()) : 0
                        });
                    }
                    Datos.Close();
                }
                catch
                {
                    Lista = null;
                }

                SqlCn.Close();
            }
            else
            {
                Lista = null;
            }

            return Lista;
        }
    }
}