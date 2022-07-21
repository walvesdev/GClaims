namespace GClaims.Core.Auth
{
    public static class AuthPolicy
    {
        public const string ADMIN = "ADMIN";
        public const string MASTER = "MASTER";
        public const string MANAGER = "MANAGER";
        public const string SELLER = "SELLER";
        public const string ECOMMERCE = "ECOMMERCE";
        public const string PARTHNER = "PARTHNER";
        public const string PROVIDER = "PROVIDER";
        public const string SUPORT = "SUPORT";
        public const string USER = "USER";
    }

    public static class AuthRoles
    {
        public const string ALL = "ADMIN,MASTER,MANAGER,SELLER,ECOMMERCE,PARTHNER,PROVIDER,SUPORT,USER";
        public const string MASTER_ADMIN = "ADMIN,MASTER";
        public const string MASTER_ADMIN_MANAGER = "ADMIN,MASTER,MANAGER";
    }
}