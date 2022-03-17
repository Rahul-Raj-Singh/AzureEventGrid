using System;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EventGrid
{
    class Startup
    {
        static async Task Main(string[] args)
        {
            var config = BuildConfig();

            var topicEndpoint = config["EventTopicEndpoint"];
            var accessKey = config["EventTopicAccessKey"];

            var eventGridManager = new EventGridManager(topicEndpoint, accessKey);

            await eventGridManager.PublishEventAsync(JsonConvert.SerializeObject(
                new Game
                {
                    Name = "God of War Ragnarok",
                    Price = 4999m,
                    ReleaseDate = new DateTime(2022, 12, 20) // 😜
                }
            ));

            Console.WriteLine("Events are sent");
            Console.ReadKey();
        }

        static IConfiguration BuildConfig()
        {
            var configuration = new ConfigurationBuilder()
                                    .AddJsonFile("./appsettings.json", optional: false)
                                    .Build();

            return configuration;
        }
    }

}
