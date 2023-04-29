namespace webAPIAutores.Servicios;

public class EscribirEnArchivo : IHostedService
{
    private readonly IWebHostEnvironment env;
    private readonly string nombreArchivo = "Archivo.txt";
    private Timer timer;

    public EscribirEnArchivo(IWebHostEnvironment _env)
    {
        env = _env;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        Escribir("Proceso Iniciado");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Escribir("Proceso Finalizado");
        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        Escribir($"Proceso en ejecucion: {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}");
    }
    private void Escribir(string mensaje)
    {
        var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
        using (StreamWriter writer = new StreamWriter(ruta, append: true))
        {
            writer.WriteLine(mensaje);
        }
    }
}