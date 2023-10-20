

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WorkerValidatorLib.Data;
using WorkerValidatorLib.Global;

namespace WorkerValidatorLib.Workers
{
    public sealed class OrderValidatorWorker : BackgroundService
    {
        private readonly ILogger<OrderValidatorWorker> _logger;

        //for signal completion
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        public OrderValidatorWorker(
            IHostApplicationLifetime hostApplicationLifetime, ILogger<OrderValidatorWorker> logger) =>
        (_hostApplicationLifetime, _logger) = (hostApplicationLifetime, logger);
        //  _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
                _logger.LogInformation("DB Worker running at: {time}", DateTimeOffset.Now);
            object myLock = new object();
            while (!stoppingToken.IsCancellationRequested)
            {
                DBContext db=new DBContext();
                 db = Global.Global.app.Services.GetRequiredService<DBContext>();
                _logger.LogInformation($"Orders count {db.Orders.Count}");

                try
                {
                    //avoid race condition
                   // lock (myLock)
                   // {
                   //Use tolist to avoid race condition
                        foreach (var order in db.Orders.ToList())
                        {
                            if (order.Checked == 0)
                            {
                                order.Checked = 1;
                                Console.WriteLine($"Order {order.OrderId} is Checked");
                                await Task.Delay(2000, stoppingToken);
                            }

                        }
                   // }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DB err {ex.Message}");
                }
             //   await Serialize(db);

                await Task.Delay(10_000, stoppingToken);
                // When completed, the entire app host will stop.
                //   _hostApplicationLifetime.StopApplication();
            }
        }
        public DBContext Deserialize()
        {
            try
            {
                var j = File.ReadAllText("orders.json");
                var app = JsonSerializer.Deserialize<DBContext>(j);
                return app;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB err {ex.Message}");
                return new DBContext();
            }
        
        }
        public async Task Serialize(DBContext db)
        {

            string fileName = "orders.json";
            using FileStream createStream = File.Create(fileName);
            var options = new JsonSerializerOptions { WriteIndented = true };
            await JsonSerializer.SerializeAsync(createStream, db, options);
            await createStream.DisposeAsync();
        }
    }
}
