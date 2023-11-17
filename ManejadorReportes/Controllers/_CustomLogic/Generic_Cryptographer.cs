using System;
using System.Text;

namespace ManejadorReportes.Controllers._CustomLogic
{
    public class Generic_Cryptographer
    {
        public static string Encode(string InToEncode)
        {
            string encoded = string.Empty;
            byte[] encrypted = Encoding.Unicode.GetBytes(InToEncode);
            encoded = Convert.ToBase64String(encrypted);
            return encoded;
        }

        public static string Decode(string InToDecode)
        {
            string decoded = string.Empty;
            byte[] decrypted = Convert.FromBase64String(InToDecode);
            decoded = Encoding.Unicode.GetString(decrypted);
            return decoded;
        }


        /// <summary>
        /// Equivalente a la funcion atob de javascript (Decdifica una cadena de entrada)
        /// </summary>
        /// <param name="InToDecode">Cadena a Decodificar</param>
        /// <returns></returns>
        public static string Decode_Atob(string InToDecode)
        {
            string decoded = string.Empty;
            byte[] decrypted = Convert.FromBase64String(InToDecode);
            decoded = Encoding.ASCII.GetString(decrypted);
            return decoded;
        }

        /// <summary>
        /// Equivalente a la funcion btoa de javascript (Codifica una cadena de entrada)
        /// </summary>
        /// <param name="InToEncode">Cadena a Codificar</param>
        /// <returns></returns>
        public static string Encode_Atob(string InToEncode)
        {
            string encoded = string.Empty;
            byte[] encrypted = Encoding.ASCII.GetBytes(InToEncode);
            encoded = Convert.ToBase64String(encrypted);
            return encoded;
        }
    }
}