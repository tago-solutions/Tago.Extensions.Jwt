using JwtWrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using Tago.Extensions.Jwt.Abstractions.Config;
using Tago.Extensions.Jwt.Abstractions.Model;
using Tago.Extensions.Jwt.Mvc;

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
            services.AddJwt(opts => {
                opts.Configure(Configuration.GetSection("Jwt:Settings"));
                opts.ConfigurePolicies(Configuration.GetSection("Jwt:Policies"));
                opts.ConfigureValildators(Configuration.GetSection("Jwt:Validators"));
            });
        }

        private void ConfigureJwtWrapper(IServiceCollection services)
        {
            services.AddJwtWrapper(opts => {
                opts.ConfigurationSettings = new JwtConfigurationSettings
                {
                    ConnectionString = "",
                };
                opts.JwksUrl = @".\Jwks\{kid}\.well-known\jwks.json";
            });
        }

        private void ConfigureJwt(IServiceCollection services)
        {
            services.AddJwt(opts => {
                opts.Configure(Configuration.GetSection("Jwt:Settings"));
                opts.ConfigureValildators(Configuration.GetSection("Jwt:Validators"));
                opts.ConfigurePolicies(GetExamplePolicies());
            });
        }

        private JwtSettings GetExampleSettings()
        {
            JwtSettings settings = new JwtSettings
            {
                ValidatorKey = "%kid%",
                SecurityKeysCacheExpiration = TimeSpan.FromMinutes(5),
            };

            var ks = new JwtSigningSettings
            {
                SymmetricKey = new JwtSymmetricKey
                {
                    Key = "veryVerySecretKey",
                    SecurityAlgorithm = "HS256",
                }
            };

            var cfg1 = new JwtConfig
            {
                SignerSettings = new JwtSignerConfig
                {
                    Audience = "me",
                    Issuer = "me",
                    KeySettings = ks
                },
                ValidationSettings = new JwtValidationConfig
                {
                    KeySettings = ks,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidIssuer = "me",
                    ValidAudience = "me",
                }
            };

            TokenValidator validator1 = new TokenValidator();
            validator1.Fields.Add(new JwtField()
            {
                Type = JwtFieldType.Issuer,
                MatchType = ValidationMatchType.Contains,
                IgnoreCase = true,
                Values = new string[] { "me*", "meee" }
            });
            validator1.Fields.Add(new JwtField()
            {
                Type = JwtFieldType.Audience,
                MatchType = ValidationMatchType.NotContains,
                Values = new string[] { "me*", "meee" }
            });

            TokenValidator validator2 = new TokenValidator();
            validator2.Fields.Add(new JwtField()
            {
                Type = JwtFieldType.Issuer,
                Values = new string[] { "me" }
            });

            //cfg1.TokenValidator = validator1;


            settings.Keys.Add("test", cfg1);


            JwtPoliciesSettings jwtPolicies = new JwtPoliciesSettings();
            jwtPolicies.Items.Add("Jwt1", new TokenValidator[] { validator2 });


            JsonSerializerSettings serSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,

            };
            serSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            var set1 = JsonConvert.SerializeObject(settings, serSettings);
            var set2 = JsonConvert.SerializeObject(jwtPolicies, serSettings);


            return settings;

            //services.AddJwt(opts => {
            //    //opts.Configure(settings);
            //    opts.Configure(Configuration.GetSection("Jwt:Settings"));
            //    opts.ConfigureValildators(Configuration.GetSection("Jwt:Validators"));
            //    opts.ConfiguePolicies(jwtPolicies);
            //});
        }

        private JwtValidators GetExampleJwtValidators()
        {
            JwtValidators obj = new JwtValidators();

            TokenValidator validator1 = new TokenValidator();
            validator1.Fields.Add(new JwtField()
            {
                Type = JwtFieldType.Issuer,
                MatchType = ValidationMatchType.Contains,
                IgnoreCase = true,
                Values = new string[] { "me*", "meee" }
            });
            validator1.Fields.Add(new JwtField()
            {
                Type = JwtFieldType.Audience,
                MatchType = ValidationMatchType.NotContains,
                Values = new string[] { "me*", "meee" }
            });

            TokenValidator validator2 = new TokenValidator();
            validator2.Fields.Add(new JwtField()
            {
                Type = JwtFieldType.Issuer,
                Values = new string[] { "me" }
            });

            obj.Add("test_me", new TokenValidator[] { validator1, validator2 });
            return obj;            
        }

        private JwtPoliciesSettings GetExamplePolicies()
        {
            TokenValidator validator2 = new TokenValidator();
            validator2.Fields.Add(new JwtField()
            {
                MatchType = ValidationMatchType.NotContains,
                Type = JwtFieldType.Issuer,
                Values = new string[] { "me" }
            });


            JwtPoliciesSettings jwtPolicies = new JwtPoliciesSettings();
            jwtPolicies.Items.Add("Jwt1", new TokenValidator[] { validator2 });

            JsonSerializerSettings serSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };
            serSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            var set2 = JsonConvert.SerializeObject(jwtPolicies, serSettings);

            return jwtPolicies;
        }
    }
}
