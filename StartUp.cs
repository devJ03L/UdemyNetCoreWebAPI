using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using webAPIAutores.Filtros;
using webAPIAutores.Middlewares;
using webAPIAutores.Servicios;

namespace webAPIAutores;

public class StartUp
{
    public IConfiguration Configuration { get; }
    public StartUp(IConfiguration configuration) => Configuration = configuration;


    public void ConfigureServices(IServiceCollection services)
    {        
        services
            .AddControllers(
                opciones => opciones.Filters.Add(typeof(FiltroDeException)))
            .AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"))
        );

        services.AddTransient<MiFiltroDeAccion>();
        //services.AddHostedService<EscribirEnArchivo>();

        services.AddResponseCaching();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<StartUp> logger)
    {
        app.UseLoguearRespuestaHTTP();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseResponseCaching();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
            endpoints.MapControllers()
        );
    }
}