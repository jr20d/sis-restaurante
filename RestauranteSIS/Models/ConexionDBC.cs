using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace RestauranteSIS.Models
{
    public class ConexionDBC
    {
        private string CadenaDBC;
        public ConexionDBC()
        {
            CadenaDBC = WebConfigurationManager.ConnectionStrings["RestDBC"].ConnectionString;
        }
        public SqlConnection Conectar(out string Error)
        {
            SqlConnection cn = new SqlConnection(CadenaDBC);
            try
            {
                cn.Open();
                Error = null;
            }
            catch (Exception Ex)
            {
                Error = $"Error: {Ex.Message.ToString()}";
            }

            return cn;
        }
    }
}