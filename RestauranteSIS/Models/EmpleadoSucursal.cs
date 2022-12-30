using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RestauranteSIS.Models
{
    public class EmpleadoSucursal : ConexionDBC
    {
        public partial class RelEmpleadoSucursal
        {
            public int EmpleadoID { get; set; }
            public int SucursalID { get; set; }
            public int RolID { get; set; }
            public int EstadoID { get; set; }
            public DateTime FechaIngreso { get; set; }
            public DateTime? FechaRetiro { get; set; }
        }

        public IList<RelEmpleadoSucursal> RelEmpleadoSucursales { get; set; } 
        public IList<Empleado> Empleados { get; set; }
        public IList<Sucursal> Sucursales { get; set; }
        public IList<Rol> Roles { get; set; }
        public IList<Estado> Estados { get; set; }

        private SqlConnection SqlCn;
        private SqlCommand SqlCmd;

        private SqlParameter ParamRelEmpleadoID;
        private SqlParameter ParamRelSucursalID;
        private SqlParameter ParamRelRolID;
        private SqlParameter ParamRelEstadoID;

        private void CrearParametros()
        {
            ParamRelEmpleadoID = new SqlParameter();
            ParamRelEmpleadoID.ParameterName = "empleadoID";
            ParamRelEmpleadoID.SqlDbType = SqlDbType.Int;
            ParamRelEmpleadoID.Direction = ParameterDirection.Input;

            ParamRelSucursalID = new SqlParameter();
            ParamRelSucursalID.ParameterName = "sucursalID";
            ParamRelSucursalID.SqlDbType = SqlDbType.Int;
            ParamRelSucursalID.Direction = ParameterDirection.Input;

            ParamRelRolID = new SqlParameter();
            ParamRelRolID.ParameterName = "rolID";
            ParamRelRolID.SqlDbType = SqlDbType.Int;
            ParamRelRolID.Direction = ParameterDirection.Input;

            ParamRelEstadoID = new SqlParameter();
            ParamRelEstadoID.ParameterName = "estadoID";
            ParamRelEstadoID.SqlDbType = SqlDbType.Int;
            ParamRelEstadoID.Direction = ParameterDirection.Input;
        }
        
        public List<RelEmpleadoSucursal> ListaRelEmpleadoSucursal()
        {
            List<RelEmpleadoSucursal> ListaRel = new List<RelEmpleadoSucursal>();
            string MsgError;
            SqlCn = Conectar(out MsgError);
            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlDataReader Datos;
                    SqlCmd = new SqlCommand("sp_listaEmpleadosSucursal", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    Datos = SqlCmd.ExecuteReader();
                    while (Datos.Read())
                    {
                        if (Datos.IsDBNull(5))
                        {
                            ListaRel.Add(new RelEmpleadoSucursal()
                            {
                                EmpleadoID = Int32.Parse(Datos["empleadoID"].ToString()),
                                SucursalID = Int32.Parse(Datos["sucursalID"].ToString()),
                                RolID = Int32.Parse(Datos["rolID"].ToString()),
                                EstadoID = Int32.Parse(Datos["estadoID"].ToString()),
                                FechaIngreso = DateTime.Parse(Datos["fecha_Ingreso"].ToString())
                            });
                        }
                        else
                        {
                            ListaRel.Add(new RelEmpleadoSucursal()
                            {
                                EmpleadoID = Int32.Parse(Datos["empleadoID"].ToString()),
                                SucursalID = Int32.Parse(Datos["sucursalID"].ToString()),
                                RolID = Int32.Parse(Datos["rolID"].ToString()),
                                EstadoID = Int32.Parse(Datos["estadoID"].ToString()),
                                FechaIngreso = DateTime.Parse(Datos["fecha_Ingreso"].ToString()),
                                FechaRetiro = DateTime.Parse(Datos["fecha_Retiro"].ToString())
                            });
                        }                        
                    }
                    Datos.Close();
                }
                catch
                {
                    ListaRel = null;
                }
                SqlCn.Close();
            }
            return ListaRel;
        }

        public string Agregar(RelEmpleadoSucursal EmpleadoSucursalDTO)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_agregarEmpleadoSucursal", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamRelEmpleadoID.Value = EmpleadoSucursalDTO.EmpleadoID;
                    ParamRelSucursalID.Value = EmpleadoSucursalDTO.SucursalID;
                    ParamRelRolID.Value = EmpleadoSucursalDTO.RolID;
                    ParamRelEstadoID.Value = EmpleadoSucursalDTO.EstadoID;
                    SqlCmd.Parameters.Add(ParamRelEmpleadoID);
                    SqlCmd.Parameters.Add(ParamRelSucursalID);
                    SqlCmd.Parameters.Add(ParamRelRolID);
                    SqlCmd.Parameters.Add(ParamRelEstadoID);
                    Mensaje = (SqlCmd.ExecuteNonQuery() > 0) ? "OK" : "No se pudo anexar el empleado a la sucursal";
                }
                catch (Exception Ex)
                {
                    Mensaje = $"ERROR: {Ex.Message.ToString()}";
                }
                SqlCn.Close();
            }
            return Mensaje;
        }
        
        public string Actualizar(RelEmpleadoSucursal EmpleadoSucursalDTO)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_actualizarRel", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamRelEmpleadoID.Value = EmpleadoSucursalDTO.EmpleadoID;
                    ParamRelEstadoID.Value = EmpleadoSucursalDTO.EstadoID;
                    SqlCmd.Parameters.Add(ParamRelEmpleadoID);
                    SqlCmd.Parameters.Add(ParamRelEstadoID);
                    Mensaje = (SqlCmd.ExecuteNonQuery() > 0) ? "OK" : "Error al actualizar";
                }
                catch (Exception Ex)
                {
                    Mensaje = $"ERROR: {Ex.Message.ToString()}";
                }
                SqlCn.Close();
            }
            return Mensaje;
        }
        public DataTable ListaDatosEmpleadoSucursal(int ID)
        {
            DataTable ListaRel = new DataTable();
            string MsgError;
            SqlCn = Conectar(out MsgError);
            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_DatosEmpleado", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamRelEmpleadoID.Value = ID;
                    SqlCmd.Parameters.Add(ParamRelEmpleadoID);
                    SqlDataAdapter Registros = new SqlDataAdapter(SqlCmd);
                    Registros.Fill(ListaRel);
                    Registros.Dispose();
                }
                catch
                {
                    ListaRel = null;
                }
                SqlCn.Close();
            }
            return ListaRel;
        }
    }
}