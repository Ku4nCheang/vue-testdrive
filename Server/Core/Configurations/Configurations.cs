namespace netcore.Core.Configurations
{
    public class AppSettings
    {
        public AuthenticateSettings Authenticate { get; set; } = new AuthenticateSettings();
        public DatabaseSettings Database { get; set; } = new DatabaseSettings();
        public ServerSettings Server { get; set; } = new ServerSettings();
    }

    public class ServerSettings
    {
        public int Port { get; set; }
        public int SSLPort { get; set; }
        public bool UseHttps { get; set; }
        public CertificateSettings Certificate { get; set; } = new CertificateSettings();
    }

    public class AuthenticateSettings
    {
        public JwtBearerSettings JwtBearer { get; set; } = new JwtBearerSettings();
    }


    public class DatabaseSettings
    {
        public bool UseInMemory { get; set; }
        public string SQLServer { get; set; }
        // public string Redis { get; set; }
    }

    public class CertificateSettings
    {
        public string CertName { get; set; }
        public string Password { get; set; }
    }

    public class JwtBearerSettings 
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
    }
}
