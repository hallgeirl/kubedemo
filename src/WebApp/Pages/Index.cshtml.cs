using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AppModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IConfiguration _config;
        private IConnection _connection;
        public int QueueCounter { get; set; }
        
        [BindProperty]
        public int AmountOfWork { get; set; } = 100000;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration config, IConnection connection)
        {
            _logger = logger;
            _config = config;
            _connection = connection;
        }

        public void OnGet()
        {
            using (var channel = _connection.CreateModel())
            {
                //Get the current queue size
                var res = channel.QueueDeclare(queue: "WorkerQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                QueueCounter = (int)res.MessageCount;
            }
        }

        
        public void OnPostAsync()
        {
            using (var channel = _connection.CreateModel())
            {
                channel.ConfirmSelect();

                channel.QueueDeclare(queue: "WorkerQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var workItem = new WorkItem() { Name = "Work_" + Guid.NewGuid().ToString(), AmountOfWork = AmountOfWork };
                string message = JsonSerializer.Serialize(workItem);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "WorkerQueue",
                                     basicProperties: null,
                                     body: body);

                channel.WaitForConfirms();

                //Get the current queue size
                var res = channel.QueueDeclare(queue: "WorkerQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                QueueCounter = (int)res.MessageCount;

                Console.WriteLine(" [x] Sent {0}", message);
            }
        }
    }
}
