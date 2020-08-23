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

            #region ���SQL���ݿ�����

            var sqlConnection = Configuration.GetConnectionString("SqlServerConnection");
            Console.WriteLine(sqlConnection);
            services.AddDbContext<UserDbContext>(option => option.UseSqlServer(sqlConnection));
            #endregion

            #region ���Swagger����
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


                #region Token�󶨵�ConfigureServices


                // ������ȨС��
                option.OperationFilter<AddResponseHeadersFilter>();
                option.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                // ��header�����token�����ݵ���̨
                option.OperationFilter<SecurityRequirementsOperationFilter>();

                // Jwt Bearer ��֤�������� oauth2
                option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey
                });
                #endregion
            });
            #endregion

            #region ע��Mapper����
            services.AddAutoMapper(typeof(ServiceProfiles));
            #endregion

            #region CORS
            services.AddCors(c =>
            {
                //һ��������ַ���
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

            //ע��ȫ���쳣����
            services.AddControllers(o =>
            {
                // ȫ���쳣����
                o.Filters.Add(typeof(GlobalExceptionsFilter));
                // ȫ��·��Ȩ�޹�Լ
                o.Conventions.Insert(0, new GlobalRouteAuthorizeConvention());
                // ȫ��·��ǰ׺��ͳһ�޸�·��
              //  o.Conventions.Insert(0, new GlobalRoutePrefixFilter(new RouteAttribute(RoutePrefix.Name)));
            })
            //ȫ������Json���л�����
            .AddNewtonsoftJson(options =>
            {
                //����ѭ������
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //��ʹ���շ���ʽ��key
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //����ʱ���ʽ
                //options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
            });

            #endregion

            #region ���ַ���ע��-netcore�Դ�����
            // ����ע��
            services.AddScoped<ICaching, MemoryCaching>();
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });
            // log��־ע��
            services.AddSingleton<ILoggerHelper, LogHelper>();
            #endregion

            #region Authorize Ȩ����֤������
            #region ����
            //��ȡ�����ļ�
            var audienceConfig = Configuration.GetSection("Audience");
              var symmetricKeyAsBase64 = audienceConfig["Secret"];
              var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
              var signingKey = new SymmetricSecurityKey(keyByteArray);

              var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

             //���Ҫ���ݿ⶯̬�󶨣������������գ���ߴ������ﶯ̬��ֵ
              var permission = new List<PermissionItem>();

             //��ɫ��ӿڵ�Ȩ��Ҫ�����
             var permissionRequirement = new PermissionRequirement(
                "/api/denied",// �ܾ���Ȩ����ת��ַ��Ŀǰ���ã�
                permission,
                ClaimTypes.Role,//���ڽ�ɫ����Ȩ
                audienceConfig["Issuer"],//������
                audienceConfig["Audience"],//����
                signingCredentials,//ǩ��ƾ��
                expiration: TimeSpan.FromSeconds(60 * 60)//�ӿڵĹ���ʱ��
                );
              #endregion
            Console.WriteLine("Ȩ�޵�һ��������");
            #region ��Ȩ
            //����Ȩ��
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Permissions.Name,
                         policy => policy.Requirements.Add(permissionRequirement));
            });
            #endregion
            Console.WriteLine("Ȩ�޵ڶ�������Ȩ");
            #region ��֤
            // ������֤����
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Issuer"],//������
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],//������
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
                         // ������ڣ����<�Ƿ����>��ӵ�������ͷ��Ϣ��
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
            Console.WriteLine("Ȩ�޵���������֤");
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //�ض���
            app.UseHttpsRedirection();

            #region Swagger
            // ���Swagger�й��м��
            //Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            //Swagger
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint("/swagger/sparktodo/swagger.json", "sparktodo Docs");
            });
            #endregion
            //����
            app.UseCors("LimitRequests");
            // ʹ�þ�̬�ļ�
            app.UseStaticFiles();
            // ʹ��cookie
            app.UseCookiePolicy();
            // ���ش�����
            app.UseStatusCodePages();
            // Routing
            app.UseRouting();
            //��֤�м��
            app.UseAuthentication();
            //��Ȩ
            app.UseAuthorization();
           

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
