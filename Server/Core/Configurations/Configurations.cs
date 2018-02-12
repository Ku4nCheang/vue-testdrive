using System;
using Microsoft.AspNetCore.Identity;

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
        public bool UseHttps { get; set; } = false;
        public bool UseGZip { get; set; } = true;
        public CertificateSettings Certificate { get; set; } = new CertificateSettings();
    }

    public class AuthenticateSettings
    {
        public IdentitySettings Identity { get; set; } = new IdentitySettings();
        public JwtBearerSettings JwtBearer { get; set; } = new JwtBearerSettings();
    }


    public class DatabaseSettings
    {
        public bool UseInMemory { get; set; }
        public string SQLServer { get; set; }
        public string Redis { get; set; }
    }

    public class CertificateSettings
    {
        public string CertName { get; set; }
        public string Password { get; set; }
    }

    public class IdentitySettings
    {
        public IdentityPasswordSettings Password { get; set; } = new IdentityPasswordSettings();
        public IdentityLockoutSettings Lockout { get; set; } = new IdentityLockoutSettings();
        public IdentityUserSettings User { get; set; } = new IdentityUserSettings();
    }

    public class IdentityPasswordSettings
    {
        public bool RequireDigit { get; set; } = true;
        public int RequiredLength { get; set; } = 8;
        public bool RequireNonAlphanumeric { get; set; } = false;
        public bool RequireUppercase { get; set; } = true;
        public bool RequireLowercase { get; set; } = false;
        public int RequiredUniqueChars { get; set; } = 6;
    }

    public class IdentityLockoutSettings 
    {
        public int DefaultLockoutTimeSpanValue { get; set; } = 30;
        public int MaxFailedAccessAttempts { get; set; } = 10;
        public bool AllowedForNewUsers { get; set; } = true;
        public TimeSpan DefaultLockoutTimeSpan { get { return new TimeSpan(DefaultLockoutTimeSpanValue); } }
    }

    public class IdentityUserSettings
    {
        public bool RequireUniqueEmail { get; set; } = true;
    }

    public class JwtBearerSettings 
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
    }
}
