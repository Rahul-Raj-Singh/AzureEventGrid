using System.Text;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;

namespace EventHandler
{
    public static class EventHandler
    {
        [FunctionName("EventHandler")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Starting ");

            var events = EventGridEvent.ParseMany(BinaryData.FromStream(req.Body));

            foreach(var _event in events)
            {
                // Handle System events
                if (_event.TryGetSystemEventData(out object eventData))
                {
                    // Handshake - done by event grid to ensure webhook integrity
                    if (eventData is SubscriptionValidationEventData subscriptionValidationEventData)
                    {
                        log.LogInformation($"Got SubscriptionValidationEventData-\nCode: {subscriptionValidationEventData.ValidationCode}\n Url: {subscriptionValidationEventData.ValidationUrl}");

                        var responseData = new SubscriptionValidationResponse
                        {
                            ValidationResponse = subscriptionValidationEventData.ValidationCode
                        };

                        return new OkObjectResult(responseData);
                    }
                }
                // Handle Custom Events
                else if (_event.EventType == "Game.NewRelease")
                {
                    log.LogInformation($"Event raised of type: {_event.EventType}");

                    log.LogInformation(_event.Data.ToString());

                    return new AcceptedResult();
                }
                
            }

            return new StatusCodeResult(500);
        }
    }
}
