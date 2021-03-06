using System;
using System.IO;
using System.Reflection;
using BooksApiMongoDb.Data.Configuration;
using BooksApiMongoDb.Repositories;
using BooksApiMongoDb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Books.Api.MongoDb
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
            services.Configure<BooksDatabaseSettings>(
                Configuration.GetSection(nameof(BooksDatabaseSettings)));

            services.AddSingleton<IBooksDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<BooksDatabaseSettings>>().Value);

            services.AddSingleton<IMongoRepository, MongoRepository>();
            services.AddSingleton<IBookService, BookService>();

            services.AddControllers()
                .AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null)
                .AddNewtonsoftJson(options => options.UseMemberCasing());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Books Api",
                    Version = "v1",
                    Description = "Improved version of the web API taught on the Microsoft website",
                    Contact = new OpenApiContact
                    {
                        Name = "Guilhermano Silva",
                        Email = "guilhermanodev@gmail.com",
                        Url = new Uri("https://linkedin.com/in/guilhermanosilva"),
                    },
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Books.Api.MongoDb v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
