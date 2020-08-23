using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Filters;
using WebApi.AutoMapper;
using WebApi.Catch;
using WebApi.EF;
using WebApi.Filter;
using WebApi.JWT;
using WebApi.Log;
using WebApi.Model;
using WebApi.Tool;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private readonly string AllowSpecificOrigin = "AllowSpecificOrigin";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            #region 添加SQL数据库连接

            var sqlConnection = Configuration.GetConnectionString("SqlServerConnection");
            Console.WriteLine(sqlConnection);
            services.AddDbContext<UserDbContext>(option => option.UseSqlServer(sqlConnection));
            #endregion

            #region 添加Swagger配置
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("sparktodo", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "SparkTodo API",
                    Description = "API for SparkTodo",
                    Contact = new OpenApiContact() { Name = "WeihanLi", Email = "weihanli@outlook.com" }
                });

                // include document file
                option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Startup).Assembly.GetName().Name}.xml"), true);


                #region Token绑定到ConfigureServices


                // 开启加权小锁
                option.OperationFilter<AddResponseHeadersFilter>();
                option.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                // 在header中添加token，传递到后台
                option.OperationFilter<SecurityRequirementsOperationFilter>();

                // Jwt Bearer 认证，必须是 oauth2
                option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });
                #endregion
            });
            #endregion

            #region 注册Mapper服务
            services.AddAutoMapper(typeof(ServiceProfiles));
            #endregion

            #region CORS
            services.AddCors(c =>
            {
                //一般采用这种方法
                c.AddPolicy("LimitRequests", policy =>
                {
                    policy
                    .WithOrigins("http://localhost:8080")
                    .AllowAnyHeader()//Ensures that the policy allows any header.
                  .AllowAnyMethod()
               .AllowCredentials();
                });
            });
            #endregion

            #region MVC + GlobalExceptions

            //注入全局异常捕获
            services.AddControllers(o =>
            {
                // 全局异常过滤
                o.Filters.Add(typeof(GlobalExceptionsFilter));
                // 全局路由权限公约
                o.Conventions.Insert(0, new GlobalRouteAuthorizeConvention());
                // 全局路由前缀，统一修改路由
              //  o.Conventions.Insert(0, new GlobalRoutePrefixFilter(new RouteAttribute(RoutePrefix.Name)));
            })
            //全局配置Json序列化处理
            .AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //设置时间格式
                //options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
            });

            #endregion

            #region 部分服务注入-netcore自带方法
            // 缓存注入
            services.AddScoped<ICaching, MemoryCaching>();
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });
            // log日志注入
            services.AddSingleton<ILoggerHelper, LogHelper>();
            #endregion

            #region Authorize 权限认证三步走
            #region 参数
            //读取配置文件
            var audienceConfig = Configuration.GetSection("Audience");
              var symmetricKeyAsBase64 = audienceConfig["Secret"];
              var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
              var signingKey = new SymmetricSecurityKey(keyByteArray);

              var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

             //如果要数据库动态绑定，这里先留个空，后边处理器里动态赋值
              var permission = new List<PermissionItem>();

             //角色与接口的权限要求参数
             var permissionRequirement = new PermissionRequirement(
                "/api/denied",// 拒绝授权的跳转地址（目前无用）
                permission,
                ClaimTypes.Role,//基于角色的授权
                audienceConfig["Issuer"],//发行人
                audienceConfig["Audience"],//听众
                signingCredentials,//签名凭据
                expiration: TimeSpan.FromSeconds(60 * 60)//接口的过期时间
                );
              #endregion
            Console.WriteLine("权限第一步：参数");
            #region 授权
            //【授权】
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Permissions.Name,
                         policy => policy.Requirements.Add(permissionRequirement));
            });
            #endregion
            Console.WriteLine("权限第二步：授权");
            #region 认证
            // 令牌验证参数
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Issuer"],//发行人
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true,
            };
            services.AddAuthentication("Bearer")
             .AddJwtBearer(o =>
             {
                 o.TokenValidationParameters = tokenValidationParameters;
                 o.Events = new JwtBearerEvents
                 {
                     OnAuthenticationFailed = context =>
                     {
                         // 如果过期，则把<是否过期>添加到，返回头信息中
                         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                         {
                             context.Response.Headers.Add("Token-Expired", "true");
                         }
                         return Task.CompletedTask;
                     }
                 };
             })
             .AddScheme<AuthenticationSchemeOptions, ApiResponseHandler>(nameof(ApiResponseHandler), o => { });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton(permissionRequirement);
            #endregion
            Console.WriteLine("权限第三步：认证");
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //重定向
            app.UseHttpsRedirection();

            #region Swagger
            // 添加Swagger有关中间件
            //Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            //Swagger
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint("/swagger/sparktodo/swagger.json", "sparktodo Docs");
            });
            #endregion
            //跨域
            app.UseCors("LimitRequests");
            // 使用静态文件
            app.UseStaticFiles();
            // 使用cookie
            app.UseCookiePolicy();
            // 返回错误码
            app.UseStatusCodePages();
            // Routing
            app.UseRouting();
            //认证中间件
            app.UseAuthentication();
            //授权
            app.UseAuthorization();
           

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
