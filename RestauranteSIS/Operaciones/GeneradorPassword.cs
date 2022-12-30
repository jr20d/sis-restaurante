using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;

namespace RestauranteSIS.Operaciones
{
    public class GeneradorPassword
    {

        public static byte[] GenerarSal()
        {
            RNGCryptoServiceProvider Criptografia = new RNGCryptoServiceProvider();
            byte[] Sal = new byte[16];
            Criptografia.GetBytes(Sal);

            return Sal;
        }

        public static byte[] GenerarHash(string Password, byte[] Sal)
        {
            Rfc2898DeriveBytes Hash = new Rfc2898DeriveBytes(Password, Sal);
            return Hash.GetBytes(64);
        }
    }
}