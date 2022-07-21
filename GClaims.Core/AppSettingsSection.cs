using System.Runtime.Serialization;

namespace GClaims.Core;

public enum MailProvider
{
    [EnumMember(Value = "SmtpRelay")] SmtpRelay,
    [EnumMember(Value = "SendGrid")] SendGrid
}

public class AppSettingsSection
{
    public string LoginUrl { get; set; }
    
    public string CookieName { get; set; }

    public TokenConfigurations TokenConfig { get; set; }
    public SecuritySection Security { get; set; }
    public RedisConnectionSection Redis { get; set; }

    public MailSection Mail { get; set; }

    public SftpSection SFTP { get; set; }

    public SendGridSection SendGrid { get; set; }

    public HangfireSection Hangfire { get; set; }

    public RabbitMQSection RabbitMQ { get; set; }
    
    public EventStore EventConfig { get; set; }

    public class TokenConfigurations
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int Minutes { get; set; }
    }

    public class EventStore
    {
        public bool Enabled { get; set; }
    }
    
    public class SecuritySection
    {
        public string AuthUrl { get; set; }
        public List<string> CorsOrigins { get; set; }
        public string CertificateName { get; set; }
        public string CertificatePassword { get; set; }
        public string IssuerUri { get; set; }
        public string AppClientSecret { get; set; }
    }

    public class MailSection
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string SMTPServer { get; set; }

        public int SMTPPort { get; set; }

        public bool EnableSsl { get; set; }

        public int TimeoutInMinutes { get; set; }

        public MailProvider Provider { get; set; } = MailProvider.SendGrid;
    }

    public class SftpSection
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }

    public class RedisConnectionSection
    {
        public enum ConnectionType
        {
            /// <summary>
            /// host:port,ssl={True|False},password={pass}
            /// </summary>
            Default,

            /// <summary>
            /// host:port?ssl={true|false}&password={pass}
            /// </summary>
            ServiceStack
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public bool Ssl { get; set; }

        public string Password { get; set; }

        public string GetConnectionString(ConnectionType connectionType)
        {
            if (connectionType == ConnectionType.ServiceStack)
            {
                return $"{Host}:{Port}?ssl={Ssl.ToString().ToLower()}&password={Password}";
            }

            return $"{Host}:{Port},ssl={Ssl},password={Password}";
        }
    }

    public class SendGridSection
    {
        public string ApiKeyId { get; set; }

        public string ApiKey { get; set; }
    }

    public class HangfireSection
    {
        public RedisSection Redis { get; set; }

        public class RedisSection
        {
            public int Db { get; set; }

            public string Prefix { get; set; }
        }
    }

    public class RabbitMQSection
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }
    }
}