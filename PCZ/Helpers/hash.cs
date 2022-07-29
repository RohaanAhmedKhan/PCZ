using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PCZ.Helpers
{
    public class hash
    {

        #region --> Salt / Hash

        internal static byte[] getSalt(int maxLength)
        {

            byte[] salt = new byte[maxLength];

            using (var rand = new RNGCryptoServiceProvider())
            {
                rand.GetNonZeroBytes(salt);
            }

            return salt;
        }


        internal static string generateHash(string username, string password, byte[] salt)
        {
            byte[] originalBytes = Encoding.UTF8.GetBytes(username + password);

            byte[] saltedOriginalBytes = new byte[originalBytes.Length + salt.Length];

            for (int i = 0; i < originalBytes.Length; i++)
                saltedOriginalBytes[i] = originalBytes[i];

            for (int i = 0; i < salt.Length; i++)
                saltedOriginalBytes[originalBytes.Length + i] = salt[i];

            HashAlgorithm sha512 = new SHA512Managed();

            byte[] hashedBytes = sha512.ComputeHash(saltedOriginalBytes);
            byte[] saltedHashedBytes = new byte[hashedBytes.Length + salt.Length];

            for (int i = 0; i < hashedBytes.Length; i++)
                saltedHashedBytes[i] = hashedBytes[i];

            for (int i = 0; i < salt.Length; i++)
                saltedHashedBytes[hashedBytes.Length + i] = salt[i];

            string a = Convert.ToBase64String(saltedHashedBytes);

            return a;

        }


        #endregion


        public static bool compareHash(string uName, string password, byte[] salt, string savedHash)
        {
            try
            {
                return (savedHash == generateHash(uName, password, salt));
            }
            catch
            {
                throw;
            }

        }

    }
}