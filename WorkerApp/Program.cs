using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WorkerValidatorLib.Data;
using System.Text.Json;
using WorkerValidatorLib.Global;

namespace WorkerApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
              object _lock = new object();
        var builder = Host.CreateApplicationBuilder();
            builder.Services.AddSingleton<DBContext>();
            builder.Services.AddHostedService<WorkerValidatorLib.Workers.OrderValidatorWorker>();
            Global.app = builder.Build();
            var db = Global.app.Services.GetRequiredService<DBContext>();

            Task.Run(() =>Global.app.RunAsync());
         
            Console.WriteLine("wriying data");
            lock (_lock)
            {
                for (int i = 0; i < 10; i++)
                {
                    db.Add(new OrderModel { OrderId = i, Customer = $"Customer {i}", Amount = i * 100 });
                }
            }
       
           // await Serialize(db);
            Console.WriteLine("Hello World!");
            Console.ReadLine();

            Console.WriteLine("wriying data");
            lock (_lock)
            {
                for (int i = 0; i < 10; i++)
                {
                    // await Task.Delay(1000);
                    db.Add(new OrderModel { OrderId = i, Customer = $"Customer {i}", Amount = i * 100 });
                    //  await Serialize(db);

                }
            }

            Console.ReadLine();

        }
        static async Task Serialize(DBContext db)
        {

            string fileName = "orders.json";
            using FileStream createStream = File.Create(fileName);
            var options = new JsonSerializerOptions { WriteIndented = true };
            await JsonSerializer.SerializeAsync(createStream, db, options);
            await createStream.DisposeAsync();
        }
    }
}