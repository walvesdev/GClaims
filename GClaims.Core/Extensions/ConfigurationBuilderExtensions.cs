using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;

namespace GClaims.Core.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddCertificates(this IConfigurationBuilder builder, string root)
    {
        // var pathCertIconePfx = Path.Combine(root, "Certificate", "cert.pfx");
        //
        // var certPfx = new X509Certificate2(pathCertIconePfx, "101015",
        //     X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
        //
        // //When LocalMachine is used, .Add() requires that you run the app as an administrator in order to work.
        // var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        // store.Open(OpenFlags.MaxAllowed);
        // store.Add(certPfx);
        // store.Close();

        return builder;
    }
}