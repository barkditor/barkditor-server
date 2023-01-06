using BarkditorServer.BusinessLogic.Services;

namespace BarkditorServer.Presentation;

class Program 
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddGrpc();

        var app = builder.Build();

        app.MapGrpcService<ProjectFilesService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
    }
}

