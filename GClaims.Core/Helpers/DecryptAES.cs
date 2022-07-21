using System.Security.Cryptography;
using System.Text;

namespace GClaims.Core.Helpers;

public static class DecryptAES
{
    public static string DecryptStringAES(string encryptedValue)
    {
        try
        {
            var keybytes = Encoding.UTF8.GetBytes("wastec2020041090");
            var iv = Encoding.UTF8.GetBytes("wastec2020041090");

            //DECRYPT FROM CRIPTOJS
            var encrypted = Convert.FromBase64String(encryptedValue);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);

            return decriptedFromJavascript;
        }
        catch
        {
            return "Senha no formato incorreto";
        }
    }

    private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
    {
        try
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold
            // the decrypted text.
            string? plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var aes = Aes.Create())
            {
                //Settings
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.FeedbackSize = 128;

                aes.Key = key;
                aes.IV = iv;

                // Create a decrytor to perform the stream transform.
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
        catch
        {
            return "falha ao decriptar";
        }
    }

    public static string EncryptStringToBytes(string plainText)
    {
        try
        {
            var Key = Encoding.UTF8.GetBytes("wastec2020041090");
            var IV = Encoding.UTF8.GetBytes("wastec2020041090");

            string strEncrypted;
            // Check arguments. 
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }

            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }

            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }

            byte[] encrypted;
            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.FeedbackSize = 128;

                aes.Key = Key;
                aes.IV = IV;

                // Create a decryptor to perform the stream transform.
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Create the streams used for encryption. 
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }

                        encrypted = msEncrypt.ToArray();
                        strEncrypted = Convert.ToBase64String(encrypted);
                    }
                }
                //byte[] decrypted = encryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
                //strEncrypted = Encoding.UTF8.GetString(decrypted);
            }

            // Return the encrypted bytes from the memory stream. 
            return strEncrypted;
        }
        catch
        {
            return "falha ao encriptar";
        }
    }
}