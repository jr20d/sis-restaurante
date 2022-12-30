using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RestauranteSIS.Models
{
    public class Categoria : ConexionDBC
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }

        private SqlConnection SqlCn;
        private SqlCommand SqlCmd;

        private SqlParameter ParamCategoriaID;
        private SqlParameter ParamCategoriaNombre;
        private SqlParameter ParamCategoriaImagen;

        private void CrearParametros()
        {
            ParamCategoriaID = new SqlParameter();
            ParamCategoriaID.ParameterName = "id";
            ParamCategoriaID.SqlDbType = SqlDbType.Int;
            ParamCategoriaID.Direction = ParameterDirection.Input;

            ParamCategoriaNombre = new SqlParameter();
            ParamCategoriaNombre.ParameterName = "nombre";
            ParamCategoriaNombre.SqlDbType = SqlDbType.VarChar;
            ParamCategoriaNombre.Size = 15;
            ParamCategoriaNombre.Direction = ParameterDirection.Input;

            ParamCategoriaImagen = new SqlParameter();
            ParamCategoriaImagen.ParameterName = "imagen";
            ParamCategoriaImagen.SqlDbType = SqlDbType.VarChar;
            ParamCategoriaImagen.Size = 250;
            ParamCategoriaImagen.Direction = ParameterDirection.Input;
        }

        public List<Categoria> ListaCategorias()
        {
            List<Categoria> Lista = new List<Categoria>();
            string MsgError;
            SqlCn = Conectar(out MsgError);
            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlDataReader Datos;
                    SqlCmd = new SqlCommand("sp_listaCategoria", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    Datos = SqlCmd.ExecuteReader();
                    while (Datos.Read())
                    {
                        Lista.Add(new Categoria() { 
                            ID = Int32.Parse(Datos["id"].ToString()),
                            Nombre = Datos["nombre"].ToString(),
                            Imagen = Datos["imagen"].ToString()
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

        //Agregar nueva categoria

        public string Agregar(Categoria CategoriaDTO)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_agregarCategoria", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamCategoriaNombre.Value = CategoriaDTO.Nombre;
                    ParamCategoriaImagen.Value = CategoriaDTO.Imagen;
                    SqlCmd.Parameters.Add(ParamCategoriaNombre);
                    SqlCmd.Parameters.Add(ParamCategoriaImagen);

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
    }
}