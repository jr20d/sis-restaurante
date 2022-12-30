using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace RestauranteSIS.Operaciones
{
    public class Validacion
    {
        public bool EsNumerico(string Valor)
        {
            bool Resultado = false;
            decimal Numero;
            try
            {
                Numero = Convert.ToDecimal(Valor);
                Resultado = true;
            }
            catch
            {
                Resultado = false;
            }
            return Resultado;
        }

        public bool SoloTexto(string Cadena)
        {
            bool Resultado = true;

            if (Cadena != null)
            {
                foreach (char Caracter in Cadena)
                {
                    if (EsNumerico(Caracter.ToString()))
                    {
                        Resultado = false;
                    }
                }
            }
            else
            {
                Resultado = false;
            }

            return Resultado;
        }

        public bool HayContenido(string Cadena)
        {
            bool Resultado;
            try
            {
                if (Cadena.Trim().Length > 0)
                {
                    Resultado = true;
                }
                else
                {
                    Resultado = false;
                }
            }
            catch
            {
                Resultado = false;
            }
            return Resultado;
        }

        public bool ValidarCorreo(string Correo)
        {
            bool Resultado;
            string Expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

            if (Correo.Trim().Length > 0)
            {
                if (Regex.IsMatch(Correo, Expresion))
                {
                    Resultado = true;
                }
                else
                {
                    Resultado = false;
                }
            }
            else
            {
                Resultado = false;
            }

            return Resultado;
        }

        public string GenerarRutaArchivo(string Imagen)
        {
            string RutaCorrecta = "../", Identificar;
            bool AgregarCaracter = false;

            for (int i = 0; i < Imagen.Length; i++)
            {
                if (RutaCorrecta == "../" && AgregarCaracter == false)
                {
                    Identificar = Imagen.Substring(i, 8);
                    if (Identificar == "Recursos")
                    {
                        AgregarCaracter = true;
                    }
                }
                if (AgregarCaracter == true)
                {
                    RutaCorrecta += Imagen.Substring(i, 1);
                }
            }

            RutaCorrecta = RutaCorrecta.Replace('\\', '/');

            return RutaCorrecta;
        }

        //Generar correlativo
        public string GenerarCorrelativo(int Numero, int Cantidad)
        {
            string Correlativo;
            int NuevoCorrelativo = Numero + 1;

            if (NuevoCorrelativo.ToString().Length < Cantidad)
            {
                Correlativo = "";
                for (int i = 0; i < Cantidad - (NuevoCorrelativo.ToString().Length); i++)
                {
                    Correlativo += "0";
                }
                Correlativo += NuevoCorrelativo.ToString();
            }
            else
            {
                Correlativo = NuevoCorrelativo.ToString();
            }
            return Correlativo;
        }
    }
}