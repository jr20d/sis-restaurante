using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RestauranteSIS.Models
{
    public class Factura : ConexionDBC
    {
        public partial class FacturaVM
        {
            public int ID { get; set; }
            public string Correlativo { get; set; }
            public DateTime Fecha { get; set; }
            public string TipoFactura { get; set; }
        }
        public int ID { get; set; }
        public int TipoFacturaID { get; set; }
        public int EmpleadoID { get; set; }
        public int ClienteID { get; set; }
        public int TipoPagoID { get; set; }
        public int MesaID { get; set; }
        public string Correlativo { get; set; }
        public float SubTotal { get; set; }
        public float IVA { get; set; }
        public float Descuento { get; set; }
        public float Total { get; set; }
        public DateTime Fecha { get; set; }

        private SqlConnection SqlCn;
        private SqlCommand SqlCmd;

        private SqlParameter ParamFacturaID;
        private SqlParameter ParamFacturaTipoID;
        private SqlParameter ParamFacturaEmpleadoID;
        private SqlParameter ParamFacturaClienteID;
        private SqlParameter ParamFacturaTipoPagoID;
        private SqlParameter ParamFacturaMesaID;
        private SqlParameter ParamFacturaCorrelativo;
        private SqlParameter ParamFacturaSubTotal;
        private SqlParameter ParamFacturaIVA;
        private SqlParameter ParamFacturaDescuento;
        private SqlParameter ParamFacturaTotal;
        private SqlParameter ParamFacturaFecha;

        private void CrearParametros()
        {
            ParamFacturaID = new SqlParameter();
            ParamFacturaID.ParameterName = "id";
            ParamFacturaID.SqlDbType = SqlDbType.Int;
            ParamFacturaID.Direction = ParameterDirection.Input;

            ParamFacturaTipoID = new SqlParameter();
            ParamFacturaTipoID.ParameterName = "tipoID";
            ParamFacturaTipoID.SqlDbType = SqlDbType.Int;
            ParamFacturaTipoID.Direction = ParameterDirection.Input;

            ParamFacturaEmpleadoID = new SqlParameter();
            ParamFacturaEmpleadoID.ParameterName = "empleadoID";
            ParamFacturaEmpleadoID.SqlDbType = SqlDbType.Int;
            ParamFacturaEmpleadoID.Direction = ParameterDirection.Input;

            ParamFacturaClienteID = new SqlParameter();
            ParamFacturaClienteID.ParameterName = "clienteID";
            ParamFacturaClienteID.SqlDbType = SqlDbType.Int;
            ParamFacturaClienteID.Direction = ParameterDirection.Input;

            ParamFacturaTipoPagoID = new SqlParameter();
            ParamFacturaTipoPagoID.ParameterName = "pagoID";
            ParamFacturaTipoPagoID.SqlDbType = SqlDbType.Int;
            ParamFacturaTipoPagoID.Direction = ParameterDirection.Input;

            ParamFacturaMesaID = new SqlParameter();
            ParamFacturaMesaID.ParameterName = "mesaID";
            ParamFacturaMesaID.SqlDbType = SqlDbType.Int;
            ParamFacturaMesaID.Direction = ParameterDirection.Input;

            ParamFacturaCorrelativo = new SqlParameter();
            ParamFacturaCorrelativo.ParameterName = "correlativo";
            ParamFacturaCorrelativo.SqlDbType = SqlDbType.VarChar;
            ParamFacturaCorrelativo.Size = 12;            
            ParamFacturaCorrelativo.Direction = ParameterDirection.Input;

            ParamFacturaSubTotal = new SqlParameter();
            ParamFacturaSubTotal.ParameterName = "subtotal";
            ParamFacturaSubTotal.SqlDbType = SqlDbType.SmallMoney;
            ParamFacturaSubTotal.Direction = ParameterDirection.Input;

            ParamFacturaIVA = new SqlParameter();
            ParamFacturaIVA.ParameterName = "iva";
            ParamFacturaIVA.SqlDbType = SqlDbType.SmallMoney;
            ParamFacturaIVA.Direction = ParameterDirection.Input;

            ParamFacturaDescuento = new SqlParameter();
            ParamFacturaDescuento.ParameterName = "descuento";
            ParamFacturaDescuento.SqlDbType = SqlDbType.SmallMoney;
            ParamFacturaDescuento.Direction = ParameterDirection.Input;

            ParamFacturaTotal = new SqlParameter();
            ParamFacturaTotal.ParameterName = "total";
            ParamFacturaTotal.SqlDbType = SqlDbType.SmallMoney;
            ParamFacturaTotal.Direction = ParameterDirection.Input;

            ParamFacturaFecha = new SqlParameter();
            ParamFacturaFecha.ParameterName = "fecha";
            ParamFacturaFecha.SqlDbType = SqlDbType.Date;
            ParamFacturaFecha.Direction = ParameterDirection.Input;
        }

        //Buscar el correlativo máximo por tipo de factura
        public int BuscarCorrelativo(int TipoID)
        {
            int Numero;
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_ultimoCorrelativo", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamFacturaTipoID.Value = TipoID;
                    SqlCmd.Parameters.Add(ParamFacturaTipoID);

                    SqlDataReader Campo = SqlCmd.ExecuteReader();

                    if (Campo.Read())
                    {
                        if (Campo["maximo"].ToString().Length > 0)
                        {
                            Numero = Int32.Parse(Campo["maximo"].ToString());
                        }
                        else
                        {
                            Numero = 0;
                        }                        
                        Campo.Close();
                    }
                    else
                    {
                        Numero = -1;
                    }
                    
                }
                catch
                {
                    Numero = -1;
                }
                SqlCn.Close();
            }
            else
            {
                Numero = -1;
            }

            return Numero;
        }

        //Facturar
        public int Facturar(Factura FacturaDTO)
        {
            int ID;
            string Mensaje;

            SqlCn = Conectar(out Mensaje);

            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_facturar", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamFacturaTipoID.Value = FacturaDTO.TipoFacturaID;
                    ParamFacturaEmpleadoID.Value = FacturaDTO.EmpleadoID;
                    ParamFacturaClienteID.Value = FacturaDTO.ClienteID;
                    ParamFacturaTipoPagoID.Value = FacturaDTO.TipoPagoID;
                    ParamFacturaMesaID.Value = FacturaDTO.MesaID;
                    ParamFacturaCorrelativo.Value = FacturaDTO.Correlativo;
                    ParamFacturaSubTotal.Value = FacturaDTO.SubTotal;
                    ParamFacturaIVA.Value = FacturaDTO.IVA;
                    ParamFacturaDescuento.Value = FacturaDTO.Descuento;
                    ParamFacturaTotal.Value = FacturaDTO.Total;
                    SqlCmd.Parameters.Add(ParamFacturaTipoID);
                    SqlCmd.Parameters.Add(ParamFacturaEmpleadoID);
                    SqlCmd.Parameters.Add(ParamFacturaClienteID);
                    SqlCmd.Parameters.Add(ParamFacturaTipoPagoID);
                    SqlCmd.Parameters.Add(ParamFacturaMesaID);
                    SqlCmd.Parameters.Add(ParamFacturaCorrelativo);
                    SqlCmd.Parameters.Add(ParamFacturaSubTotal);
                    SqlCmd.Parameters.Add(ParamFacturaIVA);
                    SqlCmd.Parameters.Add(ParamFacturaDescuento);
                    SqlCmd.Parameters.Add(ParamFacturaTotal);

                    SqlDataReader Campo = SqlCmd.ExecuteReader();

                    if (Campo.Read())
                    {
                        ID = Int32.Parse(Campo["id"].ToString());
                        Campo.Close();
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

        //Mostrar factura
        public DataTable VerFactura(int FacturaID)
        {
            DataTable Registro = new DataTable();
            string MsgError;
            SqlCn = Conectar(out MsgError);
            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCmd = new SqlCommand("sp_verFactura", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    CrearParametros();
                    ParamFacturaID.Value = FacturaID;
                    SqlCmd.Parameters.Add(ParamFacturaID);
                    SqlDataAdapter Registros = new SqlDataAdapter(SqlCmd);
                    Registros.Fill(Registro);
                    Registros.Dispose();
                }
                catch
                {
                    Registro = null;
                }
                SqlCn.Close();
            }
            return Registro;
        }

        //Mostrar Facturas
        public List<Factura.FacturaVM> FacturasVM()
        {
            List<Factura.FacturaVM> Registros = new List<Factura.FacturaVM>();
            string MsgError;
            SqlCn = Conectar(out MsgError);
            if (SqlCn.State == ConnectionState.Open)
            {
                try
                {
                    SqlDataReader Datos;
                    SqlCmd = new SqlCommand("sp_mostrarFacturas", SqlCn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    Datos = SqlCmd.ExecuteReader();
                    while (Datos.Read())
                    {
                        Registros.Add(new FacturaVM()
                        {
                            ID = Int32.Parse(Datos["id"].ToString()),
                            Correlativo = Datos["correlativo"].ToString(),
                            Fecha = DateTime.Parse(Datos["fecha"].ToString()),
                            TipoFactura = Datos["nombre"].ToString()
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