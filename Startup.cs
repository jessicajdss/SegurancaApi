using System;
using modelobasicoefjwt.Repositorio;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using modeloaulaefjwt.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace modelobasicoefjwt {
    public class Startup {
        public Startup (IConfiguration configuration) {
            this.configuration = configuration;

        }
        public IConfiguration configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices (IServiceCollection services) {
            services.AddDbContext<AutenticacaoContext> (opt => opt.UseSqlServer (
                configuration.GetConnectionString ("BancoAutenticacao")));

            var signingConfigurations = new SigningConfigurations();

            services.AddSingleton (signingConfigurations);

            var tokenConfigurations = new TokenConfigurations ();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                configuration.GetSection("TokenConfigurations"))
                .Configure(tokenConfigurations);    

            services.AddSingleton (tokenConfigurations); 

            services.AddAuthentication(authOptions =>{
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(JwtBearerOptions=>{
                var parametrosValidacao = JwtBearerOptions.TokenValidationParameters;
                parametrosValidacao.IssuerSigningKey = signingConfigurations.Key;
                parametrosValidacao.ValidAudience = tokenConfigurations.Audience;

                parametrosValidacao.ValidateIssuerSigningKey = true;
            });

            services.AddAuthorization(auth=>{
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().Build());
            });

            services.AddMvc ();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            app.UseMvc ();
        }
    }
}