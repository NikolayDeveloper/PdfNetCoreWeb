using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace PdfGenNetCoreWeb
{
    internal class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }
        protected override IntPtr LoadUnmanagedDll(String unmanagedDllName)
        {
            return LoadUnmanagedDllFromPath(unmanagedDllName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
    }
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
            CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
            string DLLPath = Path.Combine(Environment.CurrentDirectory, "libwkhtmltox.dll");
            //string DLLPath = Path.Combine(Environment.CurrentDirectory, "libwkhtmltox.so");
            // Windows: Подгружаем библиотеку из главной дирректории
            // Linux: Нужно скопировать libwkhtmltox.so в /usr/lib
            context.LoadUnmanagedLibrary(DLLPath);
            services.AddSingleton(typeof(IConverter),new SynchronizedConverter(new PdfTools()));
          

      
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,Microsoft.AspNetCore.Hosting.IHostingEnvironment env2)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
          //  RotativaConfiguration.Setup(env2);
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
