using WFC_Godot.API.Services;
using WFC_Godot.API.Services.Interfaces;

namespace WFC_Godot.API.Extension
{
    public static class ServiceCollectionExtension
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddScoped<IRecognitionService, RecognitionService>();
        }
    }
}
