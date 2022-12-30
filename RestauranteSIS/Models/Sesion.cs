using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RestauranteSIS.Models
{
    public class Sesion : ConexionDBC
    {
        public string Nombre { get; set; }
        public int UsuarioID { get; set; }
        public string Usuario { get; set; }
        public int SucursalID { get; set; }
        public int Acceso { get; set; }
        public string Foto { get; set; }
        public int EstadoID { get; set; }

        private SqlConnection SqlCn;
        private SqlCommand SqlCmd;

        public partial class Credenciales
        {
            public virtual Sesion DatosSesion { get; set; }
            public byte[] SalPassword { get; set; }
            public byte[] HashPassword { get; set; }
        }

        public int BuscarID(string Usuario)
        {
            int ID;
            string Mensaje;
            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_buscarID", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter ParamUsuario = new SqlParameter();
                    ParamUsuario.ParameterName = "usuario";
                    ParamUsuario.SqlDbType = SqlDbType.VarChar;
                    ParamUsuario.Size = 25;
                    ParamUsuario.Direction = ParameterDirection.Input;
                    ParamUsuario.Value = Usuario;
                    SqlCmd.Parameters.Add(ParamUsuario);
                    SqlDataReader Datos = SqlCmd.ExecuteReader();

                    if (Datos.Read())
                    {
                        if (Datos["id"].ToString() != null && Datos["id"].ToString() != "")
                        {
                            ID = Int32.Parse(Datos["id"].ToString());
                        }
                        else
                        {
                            ID = -1;
                        }
                    }
                    else
                    {
                        ID = 0;
                    }
                }
                catch
                {
                    ID = 0;
                }
                SqlCn.Close();
            }
            else
            {
                ID = 0;
            }

            return ID;
        }

        //Buscar usuario
        public List<Credenciales> CredencialesVM(int ID)
        {
            List<Credenciales> CredencialVM = new List<Credenciales>();
            string Mensaje;
            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_datosSesion", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter ParamEmpleadoID = new SqlParameter();
                    ParamEmpleadoID.ParameterName = "empleadoID";
                    ParamEmpleadoID.SqlDbType = SqlDbType.Int;
                    ParamEmpleadoID.Direction = ParameterDirection.Input;
                    ParamEmpleadoID.Value = ID;
                    SqlCmd.Parameters.Add(ParamEmpleadoID);
                    SqlDataReader Datos = SqlCmd.ExecuteReader();

                    while (Datos.Read())
                    {
                        CredencialVM.Add(new Credenciales() { 
                            DatosSesion = new Sesion() { 
                                Nombre = Datos["nombre"].ToString(),
                                UsuarioID = Int32.Parse(Datos["id"].ToString()),
                                Usuario = Datos["usuario"].ToString(),
                                SucursalID = Int32.Parse(Datos["sucursalID"].ToString()),
                                Foto = Datos["foto"].ToString(),
                                Acceso = Int32.Parse(Datos["acceso"].ToString()),
                                EstadoID = Int32.Parse(Datos["estadoID"].ToString())
                            },
                            SalPassword = Datos["salPassword"] as byte[],
                            HashPassword = Datos["hashPassword"] as byte[]
                        });
                    }

                    Datos.Close();
                }
                catch
                {
                    CredencialVM = null;
                }
                SqlCn.Close();
            }

            return CredencialVM;
        }
    }
}