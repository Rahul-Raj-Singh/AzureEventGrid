using System;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.EventGrid;

namespace EventGrid
{
    public class EventGridManager
    {
        private readonly EventGridPublisherClient _client;

        public EventGridManager(string topicEndpoint, string accessKey)
        {
            _client = new EventGridPublisherClient( new Uri(topicEndpoint), new AzureKeyCredential(accessKey));;
        }

        public async Task PublishEventAsync(string payload)
        {
            var _event = new EventGridEvent
            (
                subject: "Notification for new Game",
                eventType: "Game.NewRelease",
                dataVersion: "1.0",
                data: new BinaryData(payload)
            );

            await _client.SendEventAsync(_event);
        }
    }
}