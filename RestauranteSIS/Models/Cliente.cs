using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RestauranteSIS.Models
{
    public class Cliente : ConexionDBC
    {
        public partial class ClienteReservacionVM
        {
            public virtual IList<Cliente> Clientes { get; set; }
            public virtual IList<Reservacion> Reservaciones { get; set; }
        }

        public partial class DataClienteMenu
        {
            public virtual IList<Cliente> Clientes { get; set; }
            public virtual IList<Reservacion> Reservaciones { get; set; }
            public virtual IList<Menu> Platos { get; set; }
        }

        public int ID { get; set; }
        public string Nombre { get; set; }
        public string NIT { get; set; }
        public string DUI { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        private SqlConnection SqlCn;
        private SqlCommand SqlCmd;

        private SqlParameter ParamClienteID;
        private SqlParameter ParamClienteNombre;
        private SqlParameter ParamClienteNIT;
        private SqlParameter ParamClienteDUI;
        private SqlParameter ParamClienteTelefono;

        //Agregar clientes
        private void CrearParametros()
        {
            ParamClienteID = new SqlParameter();
            ParamClienteID.ParameterName = "id";
            ParamClienteID.SqlDbType = SqlDbType.Int;
            ParamClienteID.Direction = ParameterDirection.Input;

            ParamClienteNombre = new SqlParameter();
            ParamClienteNombre.ParameterName = "nombre";
            ParamClienteNombre.SqlDbType = SqlDbType.VarChar;
            ParamClienteNombre.Size = 75;
            ParamClienteNombre.Direction = ParameterDirection.Input;

            ParamClienteNIT = new SqlParameter();
            ParamClienteNIT.ParameterName = "nit";
            ParamClienteNIT.SqlDbType = SqlDbType.VarChar;
            ParamClienteNIT.Size = 17;
            ParamClienteNIT.Direction = ParameterDirection.Input;

            ParamClienteDUI = new SqlParameter();
            ParamClienteDUI.ParameterName = "dui";
            ParamClienteDUI.SqlDbType = SqlDbType.VarChar;
            ParamClienteDUI.Size = 10;
            ParamClienteDUI.Direction = ParameterDirection.Input;

            ParamClienteTelefono = new SqlParameter();
            ParamClienteTelefono.ParameterName = "tel";
            ParamClienteTelefono.SqlDbType = SqlDbType.VarChar;
            ParamClienteTelefono.Size = 9;
            ParamClienteTelefono.Direction = ParameterDirection.Input;
        }

        //Agregar cliente
        public string Agregar(Cliente ClienteDTO)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {                    
                    SqlCmd = new SqlCommand("sp_agregarCliente", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamClienteNombre.Value = ClienteDTO.Nombre;
                    ParamClienteNIT.Value = ClienteDTO.NIT;
                    if (ClienteDTO.DUI != null)
                    {
                        ParamClienteDUI.Value = ClienteDTO.DUI;
                    }
                    else
                    {
                        ParamClienteDUI.Value = "";
                    }
                    if (ClienteDTO.Telefono != null)
                    {
                        ParamClienteTelefono.Value = ClienteDTO.Telefono;
                    }
                    else
                    {
                        ParamClienteTelefono.Value = "";
                    }                    
                    SqlCmd.Parameters.Add(ParamClienteNombre);
                    SqlCmd.Parameters.Add(ParamClienteNIT);
                    SqlCmd.Parameters.Add(ParamClienteDUI);
                    SqlCmd.Parameters.Add(ParamClienteTelefono);
                    Mensaje = (SqlCmd.ExecuteNonQuery() > 0) ? "OK" : "Erro al agregar el cliente";
                }
                catch (Exception Ex)
                {
                    Mensaje = $"ERROR: {Ex.Message.ToString()}";
                }
                SqlCn.Close();
            }

            return Mensaje;
        }

        public List<Cliente> ListaCliente()
        {
            List<Cliente> Lista = new List<Cliente>();

            string Mensaje;
            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_listaClientes", SqlCn);

                    SqlDataReader Datos = SqlCmd.ExecuteReader();

                    while (Datos.Read())
                    {
                        Lista.Add(new Cliente() { 
                            ID = Int32.Parse(Datos["id"].ToString()),
                            Nombre = Datos["nombre"].ToString(),
                            NIT = Datos["nit"].ToString(),
                            DUI = Datos["dui"].ToString(),
                            Telefono = Datos["telefono"].ToString()
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