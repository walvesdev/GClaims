namespace GClaims.Core;

public static class Constants
{
    public static class Redis
    {
        public static class Keys
        {
            public const string SALES_TOKEN = "Sales-Token";
        }
    }

    public static class Roles
    {
        /// <summary>
        /// Administrador do sistema
        /// </summary>
        public const string ADMIN = "ADM";

        /// <summary>
        /// Usuario
        /// </summary>
        public const string USER = "USR";
    }
}