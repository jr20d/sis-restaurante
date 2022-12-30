using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RestauranteSIS.Models
{
    public class DetalleFactura : ConexionDBC
    {
        public int FacturaID { get; set; }
        public int MenuID { get; set; }
        public string Descripcion { get; set; }
        public float Precio { get; set; }
        public int Cantidad { get; set; }
        public float Importe { get; set; }

        private SqlConnection SqlCn;
        private SqlCommand SqlCmd;

        private SqlParameter ParamDetalleFacturaID;
        private SqlParameter ParamDetalleMenuID;
        private SqlParameter ParamDetalleDescripcion;
        private SqlParameter ParamDetallePrecio;
        private SqlParameter ParamDetalleCantidad;
        private SqlParameter ParamDetalleImporte;

        private void CrearParametros()
        {
            ParamDetalleFacturaID = new SqlParameter();
            ParamDetalleFacturaID.ParameterName = "facturaID";
            ParamDetalleFacturaID.SqlDbType = SqlDbType.Int;
            ParamDetalleFacturaID.Direction = ParameterDirection.Input;

            ParamDetalleMenuID = new SqlParameter();
            ParamDetalleMenuID.ParameterName = "menuID";
            ParamDetalleMenuID.SqlDbType = SqlDbType.Int;
            ParamDetalleMenuID.Direction = ParameterDirection.Input;

            ParamDetalleDescripcion = new SqlParameter();
            ParamDetalleDescripcion.ParameterName = "descripcion";
            ParamDetalleDescripcion.SqlDbType = SqlDbType.VarChar;
            ParamDetalleDescripcion.Size = 35;
            ParamDetalleDescripcion.Direction = ParameterDirection.Input;

            ParamDetallePrecio = new SqlParameter();
            ParamDetallePrecio.ParameterName = "precio";
            ParamDetallePrecio.SqlDbType = SqlDbType.SmallMoney;
            ParamDetallePrecio.Direction = ParameterDirection.Input;

            ParamDetalleCantidad = new SqlParameter();
            ParamDetalleCantidad.ParameterName = "cantidad";
            ParamDetalleCantidad.SqlDbType = SqlDbType.Int;
            ParamDetalleCantidad.Direction = ParameterDirection.Input;

            ParamDetalleImporte = new SqlParameter();
            ParamDetalleImporte.ParameterName = "importe";
            ParamDetalleImporte.SqlDbType = SqlDbType.SmallMoney;
            ParamDetalleImporte.Direction = ParameterDirection.Input;
        }

        //Guardar detalle de factura
        public string GuardarDetalle(DetalleFactura DetalleDTO)
        {
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_guadarDetalle", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamDetalleFacturaID.Value = DetalleDTO.FacturaID;
                    ParamDetalleMenuID.Value = DetalleDTO.MenuID;
                    ParamDetalleDescripcion.Value = DetalleDTO.Descripcion;
                    ParamDetallePrecio.Value = DetalleDTO.Precio;
                    ParamDetalleCantidad.Value = DetalleDTO.Cantidad;
                    ParamDetalleImporte.Value = DetalleDTO.Importe;

                    SqlCmd.Parameters.Add(ParamDetalleFacturaID);
                    SqlCmd.Parameters.Add(ParamDetalleMenuID);
                    SqlCmd.Parameters.Add(ParamDetalleDescripcion);
                    SqlCmd.Parameters.Add(ParamDetallePrecio);
                    SqlCmd.Parameters.Add(ParamDetalleCantidad);
                    SqlCmd.Parameters.Add(ParamDetalleImporte);

                    Mensaje = (SqlCmd.ExecuteNonQuery() > 0) ? "OK" : "Error al agregar un elemento al detalle de la factura";
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