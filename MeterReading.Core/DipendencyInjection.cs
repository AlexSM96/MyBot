using MeterReading.Core.Services;
using MeterReading.Core.Services.Base;
using MeterReading.Domain;
using MeterReading.Persistance.DatabaseContext;
using MeterReading.Persistance.Interfaces;
using MeterReading.Persistance.Repositories;
using MeterReading.Persistance.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeterReading.Core
{
    public class DipendencyInjection
    {
        private readonly IMainDbContext _dbContext;
        public IBotService BotService { get; } 
        public IConfiguration Configuration { get; }

        public DipendencyInjection()
        {
            Configuration = GetConfiguration();
            var serviceProvider = GetServices().BuildServiceProvider();
            BotService = serviceProvider.GetService<IBotService>() ?? throw new Exception();
            _dbContext = serviceProvider.GetService<MainDbContext>() ?? throw new Exception();
        }

        private IServiceCollection GetServices()
        {
            var services = new ServiceCollection()
                .AddScoped<IMainDbContext, MainDbContext>()
                .AddScoped<IBaseRepository<ElectricityIndication>, ElectricityIndicationRepository>()
                .AddScoped<IBaseRepository<WaterIndication>, WaterIndicationRepository>()
                .AddScoped<IBaseRepository<User>, UsersRepository>()
                .AddScoped<IBotService, BotService>()
                .AddDbContext<MainDbContext>(option =>
                {
                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                    option.UseNpgsql(Configuration.GetConnectionString("MainDbConection"));
                });

            return services;
        }

        private IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            return builder.Build();
        }
    }
}
