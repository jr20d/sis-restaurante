using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RestauranteSIS.Models
{
    public class Mesa : ConexionDBC
    {
        public int ID { get; set; }
        public int Numero { get; set; }
        public int Ocupado { get; set; }
        public int SucursalID { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual IList<Menu> Platos { get; set; }

        private SqlConnection SqlCn;
        private SqlCommand SqlCmd;

        private SqlParameter ParamMesaID;
        private SqlParameter ParamMesaNumero;

        private void CrearParametros()
        {
            ParamMesaID = new SqlParameter();
            ParamMesaID.ParameterName = "id";
            ParamMesaID.SqlDbType = SqlDbType.Int;
            ParamMesaID.Direction = ParameterDirection.Input;

            ParamMesaNumero = new SqlParameter();
            ParamMesaNumero.ParameterName = "numero";
            ParamMesaNumero.SqlDbType = SqlDbType.Int;
            ParamMesaNumero.Direction = ParameterDirection.Input;
        }

        public string AgregarMesa(int MesaNumero, int SucursalID)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlParameter ParamSucursalID = new SqlParameter();
                    ParamSucursalID.ParameterName = "idSucursal";
                    ParamSucursalID.SqlDbType = SqlDbType.Int;
                    ParamSucursalID.Direction = ParameterDirection.Input;
                    SqlCmd = new SqlCommand("sp_agregarMesa", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamMesaNumero.Value = MesaNumero;
                    ParamSucursalID.Value = SucursalID;
                    SqlCmd.Parameters.Add(ParamMesaNumero);
                    SqlCmd.Parameters.Add(ParamSucursalID);
                    Mensaje = (SqlCmd.ExecuteNonQuery() > 0) ? "OK" : "Error al agregar lal mesas de la sucursal";
                }
                catch (Exception Ex)
                {
                    Mensaje = $"ERROR: {Ex.Message.ToString()}";
                }
                SqlCn.Close();
            }

            return Mensaje;
        }

        //Mesas por sucursal
        public List<Mesa> ListaMesas(int SucursalID)
        {
            List<Mesa> Mesas = new List<Mesa>();
            string Mensaje;
            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlParameter ParamSucursalID = new SqlParameter();
                    ParamSucursalID.ParameterName = "idSucursal";
                    ParamSucursalID.SqlDbType = SqlDbType.Int;
                    ParamSucursalID.Direction = ParameterDirection.Input;
                    SqlCmd = new SqlCommand("sp_listaMesas", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamSucursalID.Value = SucursalID;
                    SqlCmd.Parameters.Add(ParamSucursalID);

                    SqlDataReader Datos = SqlCmd.ExecuteReader();

                    while(Datos.Read())
                    {
                        Mesas.Add(new Mesa() { 
                            ID = Int32.Parse(Datos["id"].ToString()),
                            Numero = Int32.Parse(Datos["numero"].ToString()),
                            SucursalID = Int32.Parse(Datos["sucursalID"].ToString()),
                            Cliente = new Cliente()
                        });
                    }

                    Datos.Close();
                }
                catch
                {
                    Mesas = null;
                }
                SqlCn.Close();
            }
            return Mesas;
        }
    }
}