using DatingApp.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Interfaces;
using DatingApp.API.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using DatingApp.API.Helpers;
using AutoMapper;
using DatingApp.API.DataStructures;

namespace DatingApp.API
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
            services.AddAutoMapper(); //AutoMapper.Extensions.Microsoft.DependencyInjection; it's for mapping models to Dtos
            services.AddScoped<IValueRepository, ValueRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddScoped<IPhotoRepository, PhotoRepository>();
            services.AddScoped<IPhotosHosting, PhotosHosting>();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings")); //Bring it into class by injecting IOptions<CloudinarySettings>
            services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling // because in model User has many Photos and a Photo has one user (referencing each other)
                    = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddCors();

            //Authentication middleware
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //Telling Asp.NetCore what kind of Token authentication we're using
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false, //our localhost
                        ValidateAudience = false //our localhost
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler(builder => builder.Run(async context => 
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                            
                    }));
                //app.UseHsts();
            }


            //app.UseHttpsRedirection();

            //seeder.SeedUsers();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); //don't use AllowCredentials(), it'll send cookies and we're using JwT
            app.UseAuthentication(); //JwT tokens
            app.UseMvc();
        }
    }
}
