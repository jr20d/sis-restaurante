using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RestauranteSIS.Models
{
    public class Menu : ConexionDBC
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public float Precio { get; set; }
        public int Descuento { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public int Estado { get; set; }
        public float Pagar { get; set; }
        public int CategoriaID { get; set; }
        public string CategoriaNombre { get; set; }
        public int Cantidad { get; set; }

        private SqlParameter ParamMenuID;
        private SqlParameter ParamMenuNombre;
        private SqlParameter ParamMenuPrecio;
        private SqlParameter ParamMenuDescuento;
        private SqlParameter ParamMenuDescripcion;
        private SqlParameter ParamMenuImagen;
        private SqlParameter ParamMenuEstado;
        private SqlParameter ParamMenuCategoriaID;

        //Método para crear los parametros
        private void CrearParametros()
        {
            ParamMenuID = new SqlParameter();
            ParamMenuID.ParameterName = "id";
            ParamMenuID.SqlDbType = SqlDbType.Int;
            ParamMenuID.Direction = ParameterDirection.Input;

            ParamMenuNombre = new SqlParameter();
            ParamMenuNombre.ParameterName = "nombre";
            ParamMenuNombre.SqlDbType = SqlDbType.VarChar;
            ParamMenuNombre.Size = 35;
            ParamMenuNombre.Direction = ParameterDirection.Input;

            ParamMenuPrecio = new SqlParameter();
            ParamMenuPrecio.ParameterName = "precio";
            ParamMenuPrecio.SqlDbType = SqlDbType.SmallMoney;
            ParamMenuPrecio.Direction = ParameterDirection.Input;

            ParamMenuDescuento = new SqlParameter();
            ParamMenuDescuento.ParameterName = "descuento";
            ParamMenuDescuento.SqlDbType = SqlDbType.Int;
            ParamMenuDescuento.Direction = ParameterDirection.Input;

            ParamMenuDescripcion = new SqlParameter();
            ParamMenuDescripcion.ParameterName = "descripcion";
            ParamMenuDescripcion.SqlDbType = SqlDbType.Text;
            ParamMenuDescripcion.Direction = ParameterDirection.Input;

            ParamMenuImagen = new SqlParameter();
            ParamMenuImagen.ParameterName = "imagen";
            ParamMenuImagen.SqlDbType = SqlDbType.VarChar;
            ParamMenuImagen.Size = 250;
            ParamMenuImagen.Direction = ParameterDirection.Input;

            ParamMenuEstado = new SqlParameter();
            ParamMenuEstado.ParameterName = "estado";
            ParamMenuEstado.SqlDbType = SqlDbType.Int;
            ParamMenuEstado.Direction = ParameterDirection.Input;

            ParamMenuCategoriaID = new SqlParameter();
            ParamMenuCategoriaID.ParameterName = "categoriaID";
            ParamMenuCategoriaID.SqlDbType = SqlDbType.Int;
            ParamMenuCategoriaID.Direction = ParameterDirection.Input;
        }

        public partial class MenuCategoriaVM
        {
            public virtual IList<Menu> ListaMenu { get; set; }
            public virtual IList<Categoria> Categorias { get; set; }
        }

        private SqlConnection SqlCn;
        private SqlCommand SqlCmd;

        public List<Menu> ListaMenu()
        {
            List<Menu> Lista = new List<Menu>();
            string MsgError;
            SqlCn = Conectar(out MsgError);
            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlDataReader Datos;
                    SqlCmd = new SqlCommand("sp_listaMenu", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    Datos = SqlCmd.ExecuteReader();
                    while (Datos.Read())
                    {
                        Lista.Add(new Menu()
                        {
                            ID = Int32.Parse(Datos["id"].ToString()),
                            Nombre = Datos["nombre"].ToString(),
                            Precio = float.Parse(Datos["precio"].ToString()),
                            Descuento = Int32.Parse(Datos["descuento"].ToString()),
                            Descripcion = Datos["descripcion"].ToString(),
                            Imagen = Datos["imagen"].ToString(),
                            Estado = Int32.Parse(Datos["estado"].ToString()),
                            CategoriaID = Int32.Parse(Datos["categoriaID"].ToString()),
                            Pagar = float.Parse(Datos["pagar"].ToString()),
                            CategoriaNombre = Datos["categoriaNombre"].ToString(),
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

        //Agregar elementos al menú
        public string Agregar(Menu MenuDTO)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_agregarMenu", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamMenuNombre.Value = MenuDTO.Nombre;
                    ParamMenuPrecio.Value = MenuDTO.Precio;
                    ParamMenuDescuento.Value = MenuDTO.Descuento;
                    ParamMenuDescripcion.Value = MenuDTO.Descripcion;
                    ParamMenuImagen.Value = MenuDTO.Imagen;
                    ParamMenuEstado.Value = MenuDTO.Estado;
                    ParamMenuCategoriaID.Value = MenuDTO.CategoriaID;

                    SqlCmd.Parameters.Add(ParamMenuNombre);
                    SqlCmd.Parameters.Add(ParamMenuPrecio);
                    SqlCmd.Parameters.Add(ParamMenuDescuento);
                    SqlCmd.Parameters.Add(ParamMenuDescripcion);
                    SqlCmd.Parameters.Add(ParamMenuImagen);
                    SqlCmd.Parameters.Add(ParamMenuEstado);
                    SqlCmd.Parameters.Add(ParamMenuCategoriaID);

                    Mensaje = (SqlCmd.ExecuteNonQuery() > 0) ? "OK" : "No se pudo agregar el elemento al menú";
                }
                catch (Exception Ex)
                {
                    Mensaje = $"ERROR: {Ex.Message.ToString()}";
                }
                SqlCn.Close();
            }

            return Mensaje;
        }

        //Quitar elemento del menú
        public string Quitar(int MenuID)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_quitarDelMenu", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamMenuID.Value = MenuID;
                    SqlCmd.Parameters.Add(ParamMenuID);
                    Mensaje = (SqlCmd.ExecuteNonQuery() > 0) ? "OK" : "No se pudo eliminar el elemento del menú";
                }
                catch (Exception Ex)
                {
                    Mensaje = $"ERROR: {Ex.Message.ToString()}";
                }
                SqlCn.Close();
            }

            return Mensaje;
        }

        //Editar elemento del menú
        public string Editar(Menu MenuDTO)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_editarMenu", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamMenuID.Value = MenuDTO.ID;
                    ParamMenuPrecio.Value = MenuDTO.Precio;
                    ParamMenuDescuento.Value = MenuDTO.Descuento;
                    ParamMenuDescripcion.Value = MenuDTO.Descripcion;
                    ParamMenuImagen.Value = MenuDTO.Imagen;
                    ParamMenuEstado.Value = MenuDTO.Estado;
                    ParamMenuCategoriaID.Value = MenuDTO.CategoriaID;
                    SqlCmd.Parameters.Add(ParamMenuID);
                    SqlCmd.Parameters.Add(ParamMenuPrecio);
                    SqlCmd.Parameters.Add(ParamMenuDescuento);
                    SqlCmd.Parameters.Add(ParamMenuDescripcion);
                    SqlCmd.Parameters.Add(ParamMenuImagen);
                    SqlCmd.Parameters.Add(ParamMenuEstado);
                    SqlCmd.Parameters.Add(ParamMenuCategoriaID);
                    Mensaje = (SqlCmd.ExecuteNonQuery() > 0) ? "OK" : "No se pudo actualizar el elemento del menú";
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