using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Tago.Extensions.Jwt.Abstractions;
using Tago.Extensions.Jwt.Config;

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
            ConfigureJwtFronConfiguration(services);
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

        private void ConfigureJwtFronConfiguration(IServiceCollection services)
        {
            services.AddJwt(opts => {
                opts.Configure(Configuration.GetSection("Jwt:Settings"));
                opts.ConfiguePolicies(Configuration.GetSection("Jwt:Policies"));
            });
        }

        private void ConfigureJwt(IServiceCollection services)
        {
            JwtSettings settings = new JwtSettings
            {
                DefaultJwt = new JwtConfig
                {
                    Audience = "me",
                    Issuer = "me",
                    ValidateIssuer = true,
                }
                //UnauthorizedResultCode = System.Net.HttpStatusCode.Unauthorized,                
            };


            var cfg1 = new JwtConfig
            {
                Audience = "me",
                Issuer = "me",
                //Key = "tessst",
                SigningSettings = new JwtSigningSettings
                {
                    SymmetricKey = new JwtSymmetricKey
                    {
                        Key = "veryVerySecretKey",
                        SecurityAlgorithm = "HS256",
                    }
                }
            };

            TokenValidator validator1 = new TokenValidator();
            validator1.Fields.Add(new JwtField()
            {
                Type = JwtFieldType.Issuer,
                AllowedValues = new string[] { "me" }
            });

            TokenValidator validator2 = new TokenValidator();
            validator2.Fields.Add(new JwtField()
            {
                Type = JwtFieldType.Issuer,
                DisallowedValues = new string[] { "me" }
            });

            cfg1.TokenValidator = validator1;


            settings.Keys.Add("test", cfg1);


            JwtPoliciesSettings jwtPolicies = new JwtPoliciesSettings();
            jwtPolicies.Items.Add("Jwt1", validator2);

            JsonSerializerSettings serSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };
            serSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            var set1 = JsonConvert.SerializeObject(settings, serSettings);
            var set2 = JsonConvert.SerializeObject(jwtPolicies, serSettings);



            services.AddJwt(opts => {
                opts.Configure(settings);
                opts.ConfiguePolicies(jwtPolicies);
            });


        }
    }
}
