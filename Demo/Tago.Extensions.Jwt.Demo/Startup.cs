using JwtWrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using Tago.Extensions.Jwt.Abstractions.Model;
using Tago.Extensions.Jwt.Configuration;
//using Tago.Extensions.JwkUtils;

namespace Tago.Extensions.Jwt.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Model.JwksSettings>(Configuration.GetSection("Jwks"));
            //services.AddSingleton<ITokenSigner, TokenSigner>();
            ConfigureJwtFromConfiguration(services);
            //ConfigureJwtWrapper(services);
            // Or ConfigureJwt(services);
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseJwt();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureJwtFromConfiguration(IServiceCollection services)
        {
            //services.AddJwt(opts => {
            //    opts.Configure(Configuration.GetSection("Jwt:Settings"));
            //    opts.ConfigurePolicies(Configuration.GetSection("Jwt:Policies"));
            //    opts.ConfigureValildators(Configuration.GetSection("Jwt:Validators"));
            //});

            var ks = new JwtSigningSettings
            {
                SymmetricKey = new Security.SecurityKeySymmetricKey
                {
                    Key = "veryVerySecretKey",
                    SecurityAlgorithm = "HS256",
                }
            };

            services.AddJwt(o =>
            {
                //o.Configure(Configuration.GetSection("Jwt:Settings"));
                o.Configure(cfg=> {
                    cfg.Keys.Add(new JwtValidationConfig
                    {
                        Priority = -1,
                        Selector = ".*",
                        SelectorType = ValidationKeyMatcherTypes.Regex,                        
                        KeySettings = new JwtSigningSettings
                        {
                            Jwks = new JwtJwks
                            {
                                Path = "c:\\jwks\\jwks.json"
                            }
                        }

                    });
                    //cfg.SetDefaultValidationSettings(new JwtValidationConfig
                    //{
                    //    KeySettings = new JwtSigningSettings
                    //    {
                    //        Jwks = new JwtJwks
                    //        {
                    //            Path = "c:\\jwks\\jwks.json"
                    //        }
                    //    }
                    //});
                });

                o.ConfigureValildators(builder => {
                    builder.Add("iss", new TokenValidator[] {
                        new TokenValidator
                        {
                            Fields = new System.Collections.Generic.List<JwtField>
                            {
                                new JwtField
                                {
                                    Name = "iss",
                                    Mandatory = true,
                                }
                            }
                        }
                });                
                });

                //o.SetValidationSettingsGetter<JwtWrapper.ValidationSettingsGetter>();
                //o.Configure(opts =>
                //{
                //    opts.DefaultValidationSettings = new JwtValidationConfig
                //    {
                //        KeySettings = ks
                //    };                   

                //});
                //o.Configure

            });

            services.AddSigner(o=> {
                o.Configure(Configuration.GetSection("Jwt:Signer"));
            });
            //{
            //    //o.SetSignerSettingsGetter<SignerSettingsGetter>();
            //    //o.Configure(cfg =>
            //    //{
            //    //    cfg.Keys.Add("test", new JwtSignerConfig("test")
            //    //    {                        
            //    //        KeySettings = ks
            //    //    });
            //    //});
            //});
            //services.AddSingleton<ISignerSettingsGetter, SignerSettingsGetter>();
            //services.AddSingleton<ISecurityKeyProvider, SecurityKeyProviderEx>();
        }

        //private void ConfigureJwtWrapper(IServiceCollection services)
        //{
        //    services.AddValidator(opts => {
        //        opts.ConfigurationSettings = new JwtConfigurationSettings
        //        {
        //            ConnectionString = "bla",
        //        };
        //        opts.JwksUrl = @"C:\Jwks\{kid}\.well-known\jwks.json";
        //    });

        //    services.AddSigner(opts => {
        //        //opts.SetTokenValidatorGetter
        //    });
        //}

        //private void ConfigureJwt(IServiceCollection services)
        //{
        //    services.AddJwt(opts => {
        //        opts.Configure(Configuration.GetSection("Jwt:Settings"));
        //        opts.ConfigureValildators(Configuration.GetSection("Jwt:Validators"));
        //        opts.ConfigurePolicies(GetExamplePolicies());
        //    });
        //}

        //private JwtSettings GetExampleSettings()
        //{
        //    JwtSettings settings = new JwtSettings
        //    {
        //        ValidatorKey = "%kid%",
        //        SecurityKeysCacheExpiration = TimeSpan.FromMinutes(5),
        //    };

        //    var ks = new JwtSigningSettings
        //    {
        //        SymmetricKey = new Security.SecurityKeySymmetricKey
        //        {
        //            Key = "veryVerySecretKey",
        //            SecurityAlgorithm = "HS256",
        //        }
        //    };

        //    var cfg1 = new JwtValidationConfig("test")
        //    {
        //        //SignerSettings = new JwtSignerConfig
        //        //{
        //        //    Audience = "me",
        //        //    Issuer = "me",
        //        //    KeySettings = ks
        //        //},
        //        KeySettings = ks,
        //        ValidateAudience = true,
        //        ValidateIssuer = true,
        //        ValidIssuer = "me",
        //        ValidAudience = "me",

        //    };

        //    TokenValidator validator1 = new TokenValidator();
        //    validator1.Fields.Add(new JwtField()
        //    {
        //        Type = JwtFieldType.Issuer,
        //        MatchType = ValidationMatchType.Contains,
        //        IgnoreCase = true,
        //        Values = new string[] { "me*", "meee" }
        //    });
        //    validator1.Fields.Add(new JwtField()
        //    {
        //        Type = JwtFieldType.Audience,
        //        MatchType = ValidationMatchType.NotContains,
        //        Values = new string[] { "me*", "meee" }
        //    });

        //    TokenValidator validator2 = new TokenValidator();
        //    validator2.Fields.Add(new JwtField()
        //    {
        //        Type = JwtFieldType.Issuer,
        //        Values = new string[] { "me" }
        //    });

        //    //cfg1.TokenValidator = validator1;


        //    settings.Keys.Add(cfg1);


        //    JwtPoliciesSettings jwtPolicies = new JwtPoliciesSettings();
        //    jwtPolicies.Items.Add("Jwt1", new TokenValidator[] { validator2 });


        //    JsonSerializerSettings serSettings = new JsonSerializerSettings()
        //    {
        //        Formatting = Formatting.Indented,
        //        NullValueHandling = NullValueHandling.Ignore,

        //    };
        //    serSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

        //    var set1 = JsonConvert.SerializeObject(settings, serSettings);
        //    var set2 = JsonConvert.SerializeObject(jwtPolicies, serSettings);


        //    return settings;           
        //}

        //private JwtValidators GetExampleJwtValidators()
        //{
        //    JwtValidators obj = new JwtValidators();

        //    TokenValidator validator1 = new TokenValidator();
        //    validator1.Fields.Add(new JwtField()
        //    {
        //        Type = JwtFieldType.Issuer,
        //        MatchType = ValidationMatchType.Contains,
        //        IgnoreCase = true,
        //        Values = new string[] { "me*", "meee" }
        //    });
        //    validator1.Fields.Add(new JwtField()
        //    {
        //        Type = JwtFieldType.Audience,
        //        MatchType = ValidationMatchType.NotContains,
        //        Values = new string[] { "me*", "meee" }
        //    });

        //    TokenValidator validator2 = new TokenValidator();
        //    validator2.Fields.Add(new JwtField()
        //    {
        //        Type = JwtFieldType.Issuer,
        //        Values = new string[] { "me" }
        //    });

        //    obj.Add("test_me", new TokenValidator[] { validator1, validator2 });
        //    return obj;
        //}

        //private JwtPoliciesSettings GetExamplePolicies()
        //{
        //    TokenValidator validator2 = new TokenValidator();
        //    validator2.Fields.Add(new JwtField()
        //    {
        //        MatchType = ValidationMatchType.NotContains,
        //        Type = JwtFieldType.Issuer,
        //        Values = new string[] { "me" }
        //    });


        //    JwtPoliciesSettings jwtPolicies = new JwtPoliciesSettings();
        //    jwtPolicies.Items.Add("Jwt1", new TokenValidator[] { validator2 });

        //    JsonSerializerSettings serSettings = new JsonSerializerSettings()
        //    {
        //        Formatting = Formatting.Indented
        //    };
        //    serSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

        //    var set2 = JsonConvert.SerializeObject(jwtPolicies, serSettings);

        //    return jwtPolicies;
        //}
    }
}
