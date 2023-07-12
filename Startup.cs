using Api.Filter;
using CleanStartup.Policy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

namespace CleanStartup
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region AutenticacaoAPIKey

            services.AddAuthentication("ApiKeyAuthorization")
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthorizationHandler>("ApiKeyAuthorization", null);
            services.AddSingleton<IAuthorizationHandler, AuthorizationGrupoHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("MyPolicy", policy =>
                    policy.Requirements.Add(new ApiKeysValidas()));
            });
            
            #endregion AutenticacaoAPIKey

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CleanStartup", Version = "v1" });
                c.OperationFilter<AddHeaderApikeyAndCorrelationId>();
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CleanStartup"));

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
