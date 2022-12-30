using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RestauranteSIS.Models
{
    public class Estado : ConexionDBC
    {
        public int ID { get; set; }
        public string Nombre { get; set; }

        private SqlConnection SqlCn;
        private SqlCommand SqlCmd;

        public List<Estado> ListaEstados()
        {
            List<Estado> Registros = new List<Estado>();
            string MsgError;
            SqlCn = Conectar(out MsgError);
            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlDataReader Datos;
                    SqlCmd = new SqlCommand("listaEstados", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    Datos = SqlCmd.ExecuteReader();
                    while (Datos.Read())
                    {
                        Registros.Add(new Estado()
                        {
                            ID = Int32.Parse(Datos["id"].ToString()),
                            Nombre = Datos["nombre"].ToString()
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
    }
}