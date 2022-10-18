using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Rotom.Controllers;

namespace Rotom
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public const string DATABASE_SETTINGS_KEY = "Database";
        public const string JWT_SETTINGS_KEY = "JWT";
        public const string COOKIES_SETTINGS_KEY = "Cookies";
        public const string GENERAL_SETTINGS_KEY = "General";
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<Abstract.Services.ISecurityService, Concrete.Services.SecurityService>();
            services.AddScoped<Abstract.Services.IUserService, Concrete.Services.UserService>();
            services.AddScoped<Abstract.Services.ICurrentUserService, Services.CurrentUserService>();
            services.AddScoped<Abstract.Services.IRoleService, Concrete.Services.RoleService>();
            services.AddScoped<Abstract.Services.ISettingsService, Concrete.Services.SettingsService>();

            services.Configure<Settings.DatabaseSettings>(Configuration.GetSection(DATABASE_SETTINGS_KEY));
            services.Configure<Abstract.Settings.JwtSettings>(Configuration.GetSection(JWT_SETTINGS_KEY));
            services.Configure<Settings.CookieSettings>(Configuration.GetSection(COOKIES_SETTINGS_KEY));
            services.Configure<Abstract.Settings.CookieSettings>(Configuration.GetSection(COOKIES_SETTINGS_KEY));
            services.Configure<Abstract.Settings.GeneralSettings>(Configuration.GetSection(GENERAL_SETTINGS_KEY));

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc(options =>
            {
                options.Filters.Add<Attributes.SmartMeterExceptionHandlingFilterAttribute>();
                options.Filters.Add<Attributes.AttachErrorFilterAttribute>();
            })
                .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            var dbsettings = Configuration.GetSection(DATABASE_SETTINGS_KEY).Get<Settings.DatabaseSettings>();
            services.AddDbContextPool<Data.SmartMeterContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(
                        $"server={dbsettings.Host};user={dbsettings.User};password={dbsettings.Password};database={dbsettings.Database}",
                        ServerVersion.Parse($"{dbsettings.MajorVersion}.{dbsettings.MinorVersion}.{dbsettings.BuildVersion}")
                    )
            );
        }

        private static void MigrateDB(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<Data.SmartMeterContext>();

            if (context != null) context.Database.Migrate();
        }

        private static void SeedAdminUser(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
            var roleService = serviceScope.ServiceProvider.GetService<Abstract.Services.IRoleService>();
            var userService = serviceScope.ServiceProvider.GetService<Abstract.Services.IUserService>();

            int adminRole = roleService.SeedRoles();

            userService.CreateAdminUser(adminRole);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler(HomeController.Actions.Error);
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            var supportedCultures = new[] { "en-US", "nl" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<Middlewares.AuthenticationMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            MigrateDB(app);

            SeedAdminUser(app);
        }
    }
}
