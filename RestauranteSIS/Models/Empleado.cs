using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RestauranteSIS.Models
{
    public class Empleado : ConexionDBC
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Genero { get; set; }
        public string DUI { get; set; }
        public string NIT { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }
        public byte[] SalPassword { get; set; }
        public byte[] HashPassword { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Foto { get; set; }

        private SqlParameter ParamEmpleadoID;
        private SqlParameter ParamEmpleadoNombre;
        private SqlParameter ParamEmpleadoGenero;
        private SqlParameter ParamEmpleadoDUI;
        private SqlParameter ParamEmpleadoNIT;
        private SqlParameter ParamEmpleadoTelefono;
        private SqlParameter ParamEmpleadoDireccion;
        private SqlParameter ParamEmpleadoEmail;
        private SqlParameter ParamEmpleadoUsuario;
        private SqlParameter ParamEmpleadoSal;
        private SqlParameter ParamEmpleadoHash;
        private SqlParameter ParamEmpleadoFecha;
        private SqlParameter ParamEmpleadoFoto;

        private void CrearParametros()
        {
            ParamEmpleadoID = new SqlParameter();
            ParamEmpleadoID.ParameterName = "id";
            ParamEmpleadoID.SqlDbType = SqlDbType.Int;
            ParamEmpleadoID.Direction = ParameterDirection.Input;

            ParamEmpleadoNombre = new SqlParameter();
            ParamEmpleadoNombre.ParameterName = "nombre";
            ParamEmpleadoNombre.SqlDbType = SqlDbType.VarChar;
            ParamEmpleadoNombre.Size = 75;
            ParamEmpleadoNombre.Direction = ParameterDirection.Input;

            ParamEmpleadoGenero = new SqlParameter();
            ParamEmpleadoGenero.ParameterName = "genero";
            ParamEmpleadoGenero.SqlDbType = SqlDbType.VarChar;
            ParamEmpleadoGenero.Size = 10;
            ParamEmpleadoGenero.Direction = ParameterDirection.Input;

            ParamEmpleadoDUI = new SqlParameter();
            ParamEmpleadoDUI.ParameterName = "dui";
            ParamEmpleadoDUI.SqlDbType = SqlDbType.VarChar;
            ParamEmpleadoDUI.Size = 10;
            ParamEmpleadoDUI.Direction = ParameterDirection.Input;

            ParamEmpleadoNIT = new SqlParameter();
            ParamEmpleadoNIT.ParameterName = "nit";
            ParamEmpleadoNIT.SqlDbType = SqlDbType.VarChar;
            ParamEmpleadoNIT.Size = 17;
            ParamEmpleadoNIT.Direction = ParameterDirection.Input;

            ParamEmpleadoTelefono = new SqlParameter();
            ParamEmpleadoTelefono.ParameterName = "telefono";
            ParamEmpleadoTelefono.SqlDbType = SqlDbType.VarChar;
            ParamEmpleadoTelefono.Size = 9;
            ParamEmpleadoTelefono.Direction = ParameterDirection.Input;

            ParamEmpleadoDireccion = new SqlParameter();
            ParamEmpleadoDireccion.ParameterName = "direccion";
            ParamEmpleadoDireccion.SqlDbType = SqlDbType.Text;
            ParamEmpleadoDireccion.Direction = ParameterDirection.Input;

            ParamEmpleadoEmail = new SqlParameter();
            ParamEmpleadoEmail.ParameterName = "email";
            ParamEmpleadoEmail.SqlDbType = SqlDbType.VarChar;
            ParamEmpleadoEmail.Size = 50;
            ParamEmpleadoEmail.Direction = ParameterDirection.Input;

            ParamEmpleadoUsuario = new SqlParameter();
            ParamEmpleadoUsuario.ParameterName = "usuario";
            ParamEmpleadoUsuario.SqlDbType = SqlDbType.VarChar;
            ParamEmpleadoUsuario.Size = 25;
            ParamEmpleadoUsuario.Direction = ParameterDirection.Input;

            ParamEmpleadoSal = new SqlParameter();
            ParamEmpleadoSal.ParameterName = "sal";
            ParamEmpleadoSal.SqlDbType = SqlDbType.Binary;
            ParamEmpleadoSal.Size = 16;
            ParamEmpleadoSal.Direction = ParameterDirection.Input;

            ParamEmpleadoHash = new SqlParameter();
            ParamEmpleadoHash.ParameterName = "hash";
            ParamEmpleadoHash.SqlDbType = SqlDbType.Binary;
            ParamEmpleadoHash.Size = 64;
            ParamEmpleadoHash.Direction = ParameterDirection.Input;

            ParamEmpleadoFecha = new SqlParameter();
            ParamEmpleadoFecha.ParameterName = "fecha";
            ParamEmpleadoFecha.SqlDbType = SqlDbType.Date;
            ParamEmpleadoFecha.Direction = ParameterDirection.Input;

            ParamEmpleadoFoto = new SqlParameter();
            ParamEmpleadoFoto.ParameterName = "foto";
            ParamEmpleadoFoto.SqlDbType = SqlDbType.VarChar;
            ParamEmpleadoFoto.Size = 250;
            ParamEmpleadoFoto.Direction = ParameterDirection.Input;
        }

        private SqlConnection SqlCn;
        private SqlCommand SqlCmd;

        public List<Empleado> ListaEmpleados()
        {
            List<Empleado> Registros = new List<Empleado>();
            string MsgError;
            SqlCn = Conectar(out MsgError);
            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlDataReader Datos;
                    SqlCmd = new SqlCommand("sp_listaEmpleados", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    Datos = SqlCmd.ExecuteReader();
                    while (Datos.Read())
                    {
                        Registros.Add(new Empleado()
                        {
                            ID = Int32.Parse(Datos["id"].ToString()),
                            Nombre = Datos["nombre"].ToString(),
                            Genero = Datos["genero"].ToString(),
                            DUI = Datos["dui"].ToString(),
                            NIT = Datos["nit"].ToString(),
                            Telefono = Datos["telefono"].ToString(),
                            Direccion = Datos["direccion"].ToString(),
                            Email = Datos["email"].ToString(),
                            Usuario = Datos["usuario"].ToString(),
                            FechaNacimiento = DateTime.Parse(Datos["fechaNacimiento"].ToString()),
                            Foto = Datos["foto"].ToString()
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

        public string Agregar(Empleado EmpleadoDTO)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_agregarEmpleado", SqlCn);                    
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamEmpleadoNombre.Value = EmpleadoDTO.Nombre;
                    ParamEmpleadoGenero.Value = EmpleadoDTO.Genero;
                    ParamEmpleadoDUI.Value = EmpleadoDTO.DUI;
                    ParamEmpleadoNIT.Value = EmpleadoDTO.NIT;
                    ParamEmpleadoTelefono.Value = EmpleadoDTO.Telefono;
                    ParamEmpleadoDireccion.Value = EmpleadoDTO.Direccion;
                    ParamEmpleadoEmail.Value = EmpleadoDTO.Email;
                    ParamEmpleadoUsuario.Value = EmpleadoDTO.Usuario;
                    ParamEmpleadoSal.Value = EmpleadoDTO.SalPassword;
                    ParamEmpleadoHash.Value = EmpleadoDTO.HashPassword;
                    ParamEmpleadoFecha.Value = EmpleadoDTO.FechaNacimiento;
                    ParamEmpleadoFoto.Value = EmpleadoDTO.Foto;
                    SqlCmd.Parameters.Add(ParamEmpleadoNombre);
                    SqlCmd.Parameters.Add(ParamEmpleadoGenero);
                    SqlCmd.Parameters.Add(ParamEmpleadoDUI);
                    SqlCmd.Parameters.Add(ParamEmpleadoNIT);
                    SqlCmd.Parameters.Add(ParamEmpleadoTelefono);
                    SqlCmd.Parameters.Add(ParamEmpleadoDireccion);
                    SqlCmd.Parameters.Add(ParamEmpleadoEmail);
                    SqlCmd.Parameters.Add(ParamEmpleadoUsuario);
                    SqlCmd.Parameters.Add(ParamEmpleadoSal);
                    SqlCmd.Parameters.Add(ParamEmpleadoHash);                    
                    SqlCmd.Parameters.Add(ParamEmpleadoFecha);
                    SqlCmd.Parameters.Add(ParamEmpleadoFoto);

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

        public string Actualizar(Empleado EmpleadoDTO)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    if (EmpleadoDTO.Password != null)
                    {
                        SqlCmd = new SqlCommand("sp_actualizarEmpleado", SqlCn);
                    }
                    else
                    {
                        SqlCmd = new SqlCommand("sp_actualizarEmpleado2", SqlCn);
                    }                    
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamEmpleadoID.Value = EmpleadoDTO.ID;
                    ParamEmpleadoTelefono.Value = EmpleadoDTO.Telefono;
                    ParamEmpleadoDireccion.Value = EmpleadoDTO.Direccion;
                    ParamEmpleadoEmail.Value = EmpleadoDTO.Email;
                    if (EmpleadoDTO.Password != null)
                    {
                        ParamEmpleadoSal.Value = EmpleadoDTO.SalPassword;
                        ParamEmpleadoHash.Value = EmpleadoDTO.HashPassword;
                    }                    
                    ParamEmpleadoFoto.Value = EmpleadoDTO.Foto;
                    SqlCmd.Parameters.Add(ParamEmpleadoID);
                    SqlCmd.Parameters.Add(ParamEmpleadoTelefono);
                    SqlCmd.Parameters.Add(ParamEmpleadoDireccion);
                    SqlCmd.Parameters.Add(ParamEmpleadoEmail);
                    if (EmpleadoDTO.Password != null)
                    {
                        SqlCmd.Parameters.Add(ParamEmpleadoSal);
                        SqlCmd.Parameters.Add(ParamEmpleadoHash);
                    }                    
                    SqlCmd.Parameters.Add(ParamEmpleadoFoto);

                    Mensaje = (SqlCmd.ExecuteNonQuery() > 0) ? "OK" : "Error al actualizar los datos del empleado";
                }
                catch (Exception Ex)
                {
                    Mensaje = $"ERROR: {Ex.Message.ToString()}";
                }
                SqlCn.Close();
            }

            return Mensaje;
        }

        //Cambiar contraseña
        public string CambiarPassword(int EmpleadoID, byte[] Sal, byte[] Hash)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_cambiarPassword", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamEmpleadoID.Value = EmpleadoID;
                    ParamEmpleadoSal.Value = Sal;
                    ParamEmpleadoHash.Value = Hash;
                    SqlCmd.Parameters.Add(ParamEmpleadoID);
                    SqlCmd.Parameters.Add(ParamEmpleadoSal);
                    SqlCmd.Parameters.Add(ParamEmpleadoHash);

                    Mensaje = (SqlCmd.ExecuteNonQuery() > 0) ? "OK" : "No se pudo cambiar la contraseña";
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