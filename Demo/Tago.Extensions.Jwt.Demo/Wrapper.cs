using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using Tago.Extensions.Jwt.Abstractions.Config;
using Tago.Extensions.Jwt.Abstractions.Interfaces;
using Tago.Extensions.Jwt.Abstractions.Model;
using Tago.Extensions.Jwt.Handlers;

namespace JwtWrapper
{
    public class JwtWrapperOptions
    {
        public JwtConfigurationSettings ConfigurationSettings { get; set; }
        public string JwksUrl { get; set; }
    }

    public class JwtConfigurationSettings
    {
        public string ConnectionString { get; set; }
    }

    public static class Extensions
    {
        public static IServiceCollection AddJwtWrapper(this IServiceCollection services, Action<JwtWrapperOptions> options)
        {
            JwtWrapperOptions cfg = new JwtWrapperOptions();
            if (options != null)
            {
                options.Invoke(cfg);
            }

            Options.Create(cfg.ConfigurationSettings);

            services.AddSingleton(cfg.ConfigurationSettings);


            Tago.Extensions.Jwt.Mvc.Extension.AddJwt(services, opts =>
            {
                opts.Configure(new JwtSettings
                {
                    DefaultJwtJwks = new JwtJwks
                    {
                        Path = cfg.JwksUrl
                    }
                    //DefaultJwt = new JwtConfig
                    //{
                    //    PublicKeysSettings = new JwtSigningSettings
                    //    {
                    //        Jwks = new JwtJwks
                    //        {
                    //            Path = cfg.JwksUrl
                    //        }
                    //    }
                    //}
                });

                opts.SetTokenValidatorGetter<TokenValidatorGetter>();
                opts.SetSignerSettingsGetter<SignerSettingsGetter>();
                opts.SetValidationSettingsGetter<ValidationSettingsGetter>();

            });


            //services.AddSingleton<IJwksHandler, JwksGetterHandler>();            


            return services;
        }
    }


    public class TokenValidatorGetter : ITokenValidatorGetter
    {
        private readonly JwtConfigurationSettings settings;

        private TokenValidator[] defaultTokenValidator = null;
        public TokenValidatorGetter(JwtConfigurationSettings settings)
        {
            this.settings = settings;

            var tmp = new TokenValidator
            {
                Fields = new System.Collections.Generic.List<JwtField>(),
            };

            tmp.Fields.Add(new JwtField()
            {
                Type = JwtFieldType.Issuer,
                MatchType = ValidationMatchType.Contains,
                IgnoreCase = true,
                Values = new string[] { "me*", "meee" }
            });
            tmp.Fields.Add(new JwtField()
            {
                Type = JwtFieldType.Audience,
                MatchType = ValidationMatchType.NotContains,
                IgnoreCase = true,
                Values = new string[] { "me*", "meee" }
            });


            defaultTokenValidator = new TokenValidator[] { tmp };

        }
        public TokenValidator[] Get(JwtSecurityToken token, HttpContext context)
        {
            return defaultTokenValidator;
        }
    }
    public class ValidationSettingsGetter : IValidationSettingsGetter
    {
        public ValidationSettingsGetter()
        {
        }

        public TimeSpan? GetCacheTimeOut()
        {
            return null;
        }

        public JwtValidationConfig GetValidationSettings(string kid, string issuer = null)
        {
            if (kid != null)
            {
                var ks = new JwtSigningSettings
                {
                    SymmetricKey = new JwtSymmetricKey
                    {
                        Key = "veryVerySecretKey",
                        SecurityAlgorithm = "HS256",
                    }
                };

                var cfg = new JwtValidationConfig
                {
                    ValidAudience = "Me",
                    ValidIssuer = "Me",
                    ValidateLifetime = true,
                    //KeySettings = ks,
                };

                return cfg;
            }

            return null;
        }
    }
    public class SignerSettingsGetter : ISignerSettingsGetter
    {
        public SignerSettingsGetter()
        {
        }

        public TimeSpan? GetCacheTimeOut()
        {
            return null;
        }

        public JwtSignerConfig GetSignerSettings(string kid, string issuer = null)
        {
            if (kid != null)
            {
                var ks = new JwtSigningSettings
                {
                    SymmetricKey = new JwtSymmetricKey
                    {
                        Key = "veryVerySecretKey",
                        SecurityAlgorithm = "HS256",
                    }
                };

                return new JwtSignerConfig
                {
                    Audience = "Me",
                    Issuer = "Me",
                    Expiration = TimeSpan.FromSeconds(120),
                    KeySettings = ks,
                };
            }

            return null;
        }
    }

}
