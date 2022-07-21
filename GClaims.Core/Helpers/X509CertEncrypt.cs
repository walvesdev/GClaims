using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace GClaims.Core.Helpers;

public static class X509CertEncrypt
{
    /// <summary>
    /// Get the certificate from store
    /// GetCertificateFromStore("CN=CERT_SIGN_TEST_CERT")
    /// </summary>
    /// <param name="certNameCN"></param>
    /// <param name="storeLocation"></param>
    /// <returns></returns>
    public static X509Certificate2? GetCertificateFromStore(string certNameCN,
        StoreLocation storeLocation = StoreLocation.CurrentUser)
    {
        // Get the certificate store for the current user.
        var store = new X509Store(StoreName.My, storeLocation);
        try
        {
            store.Open(OpenFlags.MaxAllowed);

            // Place all certificates in an X509Certificate2Collection object.
            var certCollection = store.Certificates;

            // If using a certificate with a trusted root you do not need to FindByTimeValid, instead:
            // currentCerts.Find(X509FindType.FindBySubjectName, certName, true);
            var currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
            var signingCert = currentCerts.Find(X509FindType.FindBySubjectName, certNameCN, false);

            return signingCert.Count == 0 ? null : signingCert[0];

            // Return the first certificate in the collection, has the right name and is current.
        }
        finally
        {
            store.Close();
        }
    }

    /// <summary>
    /// Encrypt the file using the public key from the certificate.
    /// EncryptFile(originalFile, (RSA)cert.PublicKey.Key);
    /// </summary>
    /// <param name="inFile"></param>
    /// <param name="rsaPublicKey"></param>
    /// <param name="folderToEncrypt"></param>
    public static void EncryptFile(string inFile, RSA rsaPublicKey, string folderToEncrypt)
    {
        using var aes = Aes.Create();
        // Create instance of Aes for
        // symetric encryption of the data.
        aes.KeySize = 256;
        aes.Mode = CipherMode.CBC;
        using var transform = aes.CreateEncryptor();
        var keyFormatter = new RSAPKCS1KeyExchangeFormatter(rsaPublicKey);
        var keyEncrypted = keyFormatter.CreateKeyExchange(aes.Key, aes.GetType());

        // Create byte arrays to contain
        // the length values of the key and IV.
        var LenK = new byte[4];
        var LenIV = new byte[4];

        var lKey = keyEncrypted.Length;
        LenK = BitConverter.GetBytes(lKey);
        var lIV = aes.IV.Length;
        LenIV = BitConverter.GetBytes(lIV);

        // Write the following to the FileStream
        // for the encrypted file (outFs):
        // - length of the key
        // - length of the IV
        // - ecrypted key
        // - the IV
        // - the encrypted cipher content

        var startFileName = inFile.LastIndexOf("\\", StringComparison.Ordinal) + 1;
        // Change the file's extension to ".enc"
        var outFile = string.Concat(folderToEncrypt,
            inFile.AsSpan(startFileName, inFile.LastIndexOf(".", StringComparison.Ordinal) - startFileName), ".enc");
        Directory.CreateDirectory(folderToEncrypt);

        using var outFs = new FileStream(outFile, FileMode.Create);
        outFs.Write(LenK, 0, 4);
        outFs.Write(LenIV, 0, 4);
        outFs.Write(keyEncrypted, 0, lKey);
        outFs.Write(aes.IV, 0, lIV);

        // Now write the cipher text using
        // a CryptoStream for encrypting.
        using (var outStreamEncrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
        {
            // By encrypting a chunk at
            // a time, you can save memory
            // and accommodate large files.

            // blockSizeBytes can be any arbitrary size.
            var blockSizeBytes = aes.BlockSize / 8;
            var data = new byte[blockSizeBytes];
            var bytesRead = 0;

            using (var inFs = new FileStream(inFile, FileMode.Open))
            {
                var count = 0;
                do
                {
                    count = inFs.Read(data, 0, blockSizeBytes);
                    outStreamEncrypted.Write(data, 0, count);
                    bytesRead += count;
                } while (count > 0);

                inFs.Close();
            }

            outStreamEncrypted.FlushFinalBlock();
            outStreamEncrypted.Close();
        }

        outFs.Close();
    }

    /// <summary>
    /// Decrypt the file using the public key from the certificate.
    /// DecryptFile(encryptedFile, cert.GetRSApublicKey());
    /// </summary>
    /// <param name="inFile"></param>
    /// <param name="rsapublicKey"></param>
    /// <param name="folderToEncrypt"></param>
    /// <param name="folderToDecrypt"></param>
    public static void DecryptFile(string inFile, RSA rsapublicKey, string folderToEncrypt, string folderToDecrypt)
    {
        // Create instance of Aes for
        // symetric decryption of the data.
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.Mode = CipherMode.CBC;

        // Create byte arrays to get the length of
        // the encrypted key and IV.
        // These values were stored as 4 bytes each
        // at the beginning of the encrypted package.
        var LenK = new byte[4];
        var LenIV = new byte[4];

        // Construct the file name for the decrypted file.
        var outFile = folderToDecrypt + inFile[..inFile.LastIndexOf(".", StringComparison.Ordinal)] + ".txt";

        // Use FileStream objects to read the encrypted
        // file (inFs) and save the decrypted file (outFs).
        using var inFs = new FileStream(folderToEncrypt + inFile, FileMode.Open);
        inFs.Seek(0, SeekOrigin.Begin);
        inFs.Seek(0, SeekOrigin.Begin);
        _ = inFs.Read(LenK, 0, 3);
        inFs.Seek(4, SeekOrigin.Begin);
        _ = inFs.Read(LenIV, 0, 3);

        // Convert the lengths to integer values.
        var lenK = BitConverter.ToInt32(LenK, 0);
        var lenIV = BitConverter.ToInt32(LenIV, 0);

        // Determine the start position of
        // the cipher text (startC)
        // and its length(lenC).
        var startC = lenK + lenIV + 8;

        // Create the byte arrays for
        // the encrypted Aes key,
        // the IV, and the cipher text.
        var KeyEncrypted = new byte[lenK];
        var IV = new byte[lenIV];

        // Extract the key and IV
        // starting from index 8
        // after the length values.
        inFs.Seek(8, SeekOrigin.Begin);
        _ = inFs.Read(KeyEncrypted, 0, lenK);
        inFs.Seek(8 + lenK, SeekOrigin.Begin);
        _ = inFs.Read(IV, 0, lenIV);
        Directory.CreateDirectory(folderToDecrypt);
        // Use RSA
        // to decrypt the Aes key.
        var KeyDecrypted = rsapublicKey.Decrypt(KeyEncrypted, RSAEncryptionPadding.Pkcs1);

        // Decrypt the key.
        using var transform = aes.CreateDecryptor(KeyDecrypted, IV);
        // Decrypt the cipher text from
        // from the FileSteam of the encrypted
        // file (inFs) into the FileStream
        // for the decrypted file (outFs).
        using (var outFs = new FileStream(outFile, FileMode.Create))
        {
            var blockSizeBytes = aes.BlockSize / 8;
            var data = new byte[blockSizeBytes];

            // By decrypting a chunk a time,
            // you can save memory and
            // accommodate large files.

            // Start at the beginning
            // of the cipher text.
            inFs.Seek(startC, SeekOrigin.Begin);
            using (var outStreamDecrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
            {
                var count = 0;
                do
                {
                    count = inFs.Read(data, 0, blockSizeBytes);
                    outStreamDecrypted.Write(data, 0, count);
                } while (count > 0);

                outStreamDecrypted.FlushFinalBlock();
                outStreamDecrypted.Close();
            }

            outFs.Close();
        }

        inFs.Close();
    }

    /// <summary>
    /// Print info for X509Certificate2 object from .cer file in consolee.
    /// </summary>
    /// <param name="x509"></param>
    [Obsolete("Obsolete")]
    public static void GetCertInfoInConsole(string certNameCN)
    {
        var x509 = GetCertificateFromStore(certNameCN);

        try
        {
            //X509Certificate2 x509 = new X509Certificate2();
            //x509.Import(rawData);

            //Print to console information contained in the certificate.
            Console.WriteLine("{0}Subject: {1}{0}", Environment.NewLine, x509?.Subject);
            Console.WriteLine("{0}Issuer: {1}{0}", Environment.NewLine, x509?.Issuer);
            Console.WriteLine("{0}Version: {1}{0}", Environment.NewLine, x509?.Version);
            Console.WriteLine("{0}Valid Date: {1}{0}", Environment.NewLine, x509?.NotBefore);
            Console.WriteLine("{0}Expiry Date: {1}{0}", Environment.NewLine, x509?.NotAfter);
            Console.WriteLine("{0}Thumbprint: {1}{0}", Environment.NewLine, x509?.Thumbprint);
            Console.WriteLine("{0}Serial Number: {1}{0}", Environment.NewLine, x509?.SerialNumber);
            Console.WriteLine("{0}Friendly Name: {1}{0}", Environment.NewLine, x509?.PublicKey.Oid.FriendlyName);
            Console.WriteLine("{0}Public Key Format: {1}{0}", Environment.NewLine,
                x509?.PublicKey.EncodedKeyValue.Format(true));
            Console.WriteLine("{0}Raw Data Length: {1}{0}", Environment.NewLine, x509?.RawData.Length);
            Console.WriteLine("{0}Certificate to string: {1}{0}", Environment.NewLine, x509?.ToString(true));
            if (x509?.PublicKey.Key != null)
            {
                Console.WriteLine("{0}Certificate to XML String: {1}{0}", Environment.NewLine,
                    x509.PublicKey.Key.ToXmlString(false));
            }

            //Add the certificate to a X509Store.
            var store = new X509Store();
            store.Open(OpenFlags.MaxAllowed);
            store.Add(x509!);
            store.Close();
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("Error: The directory specified could not be found.");
        }
        catch (IOException)
        {
            Console.WriteLine("Error: A file in the directory could not be accessed.");
        }
        catch (NullReferenceException)
        {
            Console.WriteLine("File must be a .cer file. Program does not have access to that type of file.");
        }
    }

    /// <summary>
    /// Get Public Key like .cer file
    /// </summary>
    /// <param name="x509"></param>
    /// <returns></returns>
    public static string GetPublicKey(string certNameCN)
    {
        var x509 = GetCertificateFromStore(certNameCN);
        var publicKey = x509!.GetRSAPublicKey();
        var publicKeyStr = Convert.ToBase64String(publicKey!.ExportSubjectPublicKeyInfo());

        //var pKey = Convert.ToBase64String(x509.GetPublicKey());
        //var issuer = x509.GetNameInfo(X509NameType.DnsName, true);
        //var subject = x509.GetNameInfo(X509NameType.DnsName, false);
        //var eq = pKey.Equals(publicKeyStr);

        return publicKeyStr;
    }

    /// <summary>
    /// Get Private Key like .cer file
    /// </summary>
    /// <param name="x509"></param>
    /// <returns></returns>
    public static string GetPrivateKey(string certNameCN)
    {
        var x509 = GetCertificateFromStore(certNameCN);
        var privateKey = x509!.GetRSAPrivateKey();
        return Convert.ToBase64String(privateKey!.ExportRSAPrivateKey());
    }
}