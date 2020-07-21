using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.RegularExpressions;
using Tago.Extensions.Jwt.Abstractions.Interfaces;
using Tago.Extensions.Jwt.Abstractions.Model;
using Tago.Extensions.Jwt.Configuration;

namespace JwtWrapper
{
    public class JwtValidatorOptions
    {
        public JwtConfigurationSettings ConfigurationSettings { get; set; }
        public string JwksUrl { get; set; }
    }
    public class JwtConfigurationSettings
    {
        public string ConnectionString { get; set; }
    }


    //public class JwtSignerOptions
    //{
    //}


    public static class Extensions
    {
        public static IServiceCollection AddValidator(this IServiceCollection services, Action<JwtValidatorOptions> options)
        {
            JwtValidatorOptions cfg = new JwtValidatorOptions();
            if (options != null)
            {
                options.Invoke(cfg);
            }

            Options.Create(cfg.ConfigurationSettings);
            services.AddSingleton(cfg.ConfigurationSettings);


            services.AddJwt(opts =>
            {
                opts.Configure(o =>
                    {
                        o.SetDefaultValidationSettings(
                            new JwtValidationConfig
                            {
                                KeySettings = new JwtSigningSettings
                                {
                                    Jwks = new JwtJwks
                                    {
                                        Path = cfg.JwksUrl,
                                    },
                                }
                            
                        });                        
                     
                    }
                );

                opts.SetTokenValidatorGetter<TokenValidatorGetter>();
                opts.SetValidationSettingsGetter<JwtWrapper.ValidationSettingsGetter>();
            });


            //services.AddSingleton<IJwksHandler, JwksGetterHandler>();            


            return services;
        }

        public static IServiceCollection AddSigner(this IServiceCollection services, Action<JwtSignerOptions> options = null)
        {
            services.AddJwtSigner(options);
            services.AddSingleton<ITokenSigner, TokenSigner>();
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

            //tmp.Fields.Add(new JwtField()
            //{
            //    Type = JwtFieldType.Issuer,
            //    MatchType = ValidationMatchType.Contains,
            //    IgnoreCase = true,
            //    Values = new string[] { "me*", "meee" }
            //});
            //tmp.Fields.Add(new JwtField()
            //{
            //    Type = JwtFieldType.Audience,
            //    MatchType = ValidationMatchType.NotContains,
            //    IgnoreCase = true,
            //    Values = new string[] { "me*", "meee" }
            //});


            defaultTokenValidator = new TokenValidator[] { tmp };

        }
        public TokenValidator[] Get(JwtSecurityToken token, HttpContext context)
        {
            return defaultTokenValidator;
        }
    }
    public class ValidationSettingsGetter : Tago.Extensions.Jwt.Abstractions.Interfaces.IValidationSettingsGetter
    {
        private JwtSettings options;
        private JwtValidationConfig _default = null;
        public ValidationSettingsGetter(IOptionsMonitor<JwtSettings> options)
        {
            this.options = options.CurrentValue;
            options.OnChange(o => {
                this.options = o;
            });
        }

        public TimeSpan? GetCacheTimeOut()
        {
            return null;
        }

        public JwtValidationConfig GetDefaultValidationSettings()
        {
            if (_default == null)
            {
                var ks = new JwtSigningSettings
                {
                    SymmetricKey = new Tago.Extensions.Security.SecurityKeySymmetricKey
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
                    KeySettings = ks,
                };

                _default = cfg;
            }

            return _default;
        }

        public JwtValidationConfig GetValidationSettings(string kid, string issuer = null)
        {
            if (kid != null)
            {
                if (this.options?.Keys?.Count > 0)
                {
                    JwtValidationConfig opt = this.options.Keys.OrderBy(o=>o.Priority).FirstOrDefault(o => o.IsMatch(kid));
                    if( opt != null)
                    {
                        return opt;
                    }
                }                

                return GetDefaultValidationSettings();
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

        public JwtSignerConfig GetSignerSettings(string key, JwtPayload claims)
        {

            //Tago.Extensions.Jwt.Requirements

            if (key != null)
            {
                var ks = new JwtSigningSettings
                {
                    SymmetricKey = new Tago.Extensions.Security.SecurityKeySymmetricKey
                    {
                        Key = "veryVerySecretKey",
                        SecurityAlgorithm = "HS256",
                    }
                };

                return new JwtSignerConfig
                {                    
                    Kid = key,
                    Audience = "Me",
                    Issuer = "Me",
                    Expiration = TimeSpan.FromSeconds(120),
                    KeySettings = ks,
                };
            }

            return null;
        }
    }



    public interface ITokenSigner : ITokenGenerator
    {
        //Task<string> SignAsync(JwtToken token, string key);
    }


    internal class TokenSigner : ITokenSigner
    {
        private readonly ITokenGenerator tokenGenerator;

        public TokenSigner(ITokenGenerator tokenGenerator)
        {
            this.tokenGenerator = tokenGenerator;
        }

        public string GenerateTest()
        {
            return tokenGenerator.GenerateTest();
        }
        

        public string GenerateUnsigned(JwtPayload token)
        {
            return tokenGenerator.GenerateUnsigned(token);
        }

        public string Sign(JwtSecurityToken token, string configurationKey, DateTime? validFrom = null, DateTime? validTo = null)
        {
            return tokenGenerator.Sign(token, configurationKey, validFrom, validTo);
        }     

        public string Sign(JwtPayload token, string configurationKey)
        {
            return tokenGenerator.Sign(token, configurationKey);
        }
    }



}
