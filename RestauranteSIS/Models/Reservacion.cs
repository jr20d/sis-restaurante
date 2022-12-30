using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RestauranteSIS.Models
{
    public class Reservacion : ConexionDBC
    {
        public int ID { get; set; }
        public int ClienteID { get; set; }
        public int MesaID { get; set; }
        public DateTime Fecha { get; set; }

        private SqlConnection SqlCn;
        private SqlCommand SqlCmd;

        public List<Reservacion> ReservacionesVM()
        {
            List<Reservacion> Lista = new List<Reservacion>();

            string Mensaje;
            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_listaResevacion", SqlCn);

                    SqlDataReader Datos = SqlCmd.ExecuteReader();

                    while (Datos.Read())
                    {
                        Lista.Add(new Reservacion() { 
                            ID = Int32.Parse(Datos["id"].ToString()),
                            ClienteID = Int32.Parse(Datos["clienteID"].ToString()),
                            MesaID = Int32.Parse(Datos["mesaID"].ToString()),
                            Fecha = DateTime.Parse(Datos["fecha"].ToString())
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

            return Lista;
        }
    }
}