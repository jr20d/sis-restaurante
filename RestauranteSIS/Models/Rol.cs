using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RestauranteSIS.Models
{
    public class Rol : ConexionDBC
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public decimal Salario { get; set; }
        public int Acceso { get; set; }
        public int Unico { get; set; }

        private SqlConnection SqlCn;
        private SqlCommand SqlCmd;

        private SqlParameter ParamRolID;
        private SqlParameter ParamRolNombre;
        private SqlParameter ParamRolSalario;
        private SqlParameter ParamRolAcceso;
        private SqlParameter ParamRolUnico;

        private void CrearParametros()
        {
            ParamRolID = new SqlParameter();
            ParamRolID.ParameterName = "id";
            ParamRolID.SqlDbType = SqlDbType.Int;
            ParamRolID.Direction = ParameterDirection.Input;

            ParamRolNombre = new SqlParameter();
            ParamRolNombre.ParameterName = "nombre";
            ParamRolNombre.SqlDbType = SqlDbType.VarChar;
            ParamRolNombre.Size = 50;
            ParamRolNombre.Direction = ParameterDirection.Input;

            ParamRolSalario = new SqlParameter();
            ParamRolSalario.ParameterName = "salario";
            ParamRolSalario.SqlDbType = SqlDbType.SmallMoney;
            ParamRolSalario.Direction = ParameterDirection.Input;

            ParamRolAcceso = new SqlParameter();
            ParamRolAcceso.ParameterName = "acceso";
            ParamRolAcceso.SqlDbType = SqlDbType.Int;
            ParamRolAcceso.Direction = ParameterDirection.Input;

            ParamRolUnico = new SqlParameter();
            ParamRolUnico.ParameterName = "unico";
            ParamRolUnico.SqlDbType = SqlDbType.Int;
            ParamRolUnico.Direction = ParameterDirection.Input;
        }

        public List<Rol> ListaRoles()
        {
            List<Rol> Registros = new List<Rol>();
            string MsgError;
            SqlCn = Conectar(out MsgError);
            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlDataReader Datos;
                    SqlCmd = new SqlCommand("sp_listaRoles", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    Datos = SqlCmd.ExecuteReader();
                    while (Datos.Read())
                    {
                        Registros.Add(new Rol()
                        {
                            ID = Int32.Parse(Datos["id"].ToString()),
                            Nombre = Datos["nombre"].ToString(),
                            Salario = Decimal.Parse(Datos["salario"].ToString()),
                            Acceso = Int32.Parse(Datos["acceso"].ToString()),
                            Unico = Int32.Parse(Datos["unico"].ToString())
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

        public string AgregarRol(Rol DatosRol)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_agregarRol", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamRolNombre.Value = DatosRol.Nombre;
                    ParamRolSalario.Value = DatosRol.Salario;
                    ParamRolAcceso.Value = DatosRol.Acceso;
                    ParamRolUnico.Value = DatosRol.Unico;
                    SqlCmd.Parameters.Add(ParamRolNombre);
                    SqlCmd.Parameters.Add(ParamRolSalario);
                    SqlCmd.Parameters.Add(ParamRolAcceso);
                    SqlCmd.Parameters.Add(ParamRolUnico);
                    Mensaje = (SqlCmd.ExecuteNonQuery() > 0) ? "OK" : "No se pudo agregar el rol";
                }
                catch (Exception Ex)
                {
                    Mensaje = $"ERROR: {Ex.Message.ToString()}";
                }
                SqlCn.Close();
            }

            return Mensaje;
        }

        public string EditarRol(Rol DatosRol)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_actualizarRol", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamRolID.Value = DatosRol.ID;
                    ParamRolSalario.Value = DatosRol.Salario;
                    SqlCmd.Parameters.Add(ParamRolID);
                    SqlCmd.Parameters.Add(ParamRolSalario);
                    Mensaje = (SqlCmd.ExecuteNonQuery() > 0) ? "OK" : "No se pudo actualizar el rol";
                }
                catch (Exception Ex)
                {
                    Mensaje = $"ERROR: {Ex.Message.ToString()}";
                }
                SqlCn.Close();
            }

            return Mensaje;
        }
    }
}