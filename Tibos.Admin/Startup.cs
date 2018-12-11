﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tibos.Admin.Filters;
using Tibos.Confing.autofac;
using Tibos.ConfingModel.model;

namespace Tibos.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) //默认
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)//增加环境配置文件，新建项目默认有
                //.AddJsonFile(env.ContentRootPath + @"\bin\Debug\netcoreapp2.0\application\ManageConfig.json", optional: true)//增加配置 (自定义配置路径)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //替换控制器所有者
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            //添加AutoMapper
            services.AddAutoMapper();

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ResourceFilterAttribute));
                options.Filters.Add(typeof(ActionFilterAttribute));
                options.Filters.Add(typeof(ExceptionFilterAttribute));
                options.Filters.Add(typeof(ResultFilterAttribute));

            }).AddJsonOptions(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //设置时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            services.AddTransient<HttpContextAccessor>();

            //缓存
            services.AddMemoryCache();

            //配置跨域处理
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.AllowAnyOrigin() //允许任何来源的主机访问
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//指定处理cookie
                });
            });

            //添加options
            services.AddOptions();
            services.Configure<ManageConfig>(Configuration.GetSection("ManageConfig"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //配置session的有效时间,单位秒
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(30); 
            });

            //权限验证
            services.AddAuthorization();

            var containerBuilder = new ContainerBuilder();
            //模块化注入
            containerBuilder.RegisterModule<DefaultModule>();
            //属性注入控制器

            //containerBuilder.RegisterControllers(Assembly.GetExecutingAssembly());  //注入所有Controller
            containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).PropertiesAutowired();

            containerBuilder.Populate(services);
            var container = containerBuilder.Build();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            return new AutofacServiceProvider(container);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSession();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "CMS",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                routes.MapRoute(
                 name: "SYS",
                 template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
               );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Login}/{id?}");
            });

        }
    }
}
