using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ServerWebbPageHttpTrigger
{
    public class ServerWebbPageHttpTrigger
    {
        private readonly ILogger<ServerWebbPageHttpTrigger> _logger;

        public ServerWebbPageHttpTrigger(ILogger<ServerWebbPageHttpTrigger> logger)
        {
            _logger = logger;
        }

        [Function("ServerWebbPageHttpTrigger")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Skapar ett nytt HttpResponseData-objekt
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);

            string html = @"
            <!DOCTYPE html>
            <html lang='sv'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Starta Server</title>
            </head>
            <body>
                <h1>Starta Servern</h1>
                <form id='apiForm' action='https://gameserverstart.azurewebsites.net/api/HttpTriggerVmStart' method='post'>
                    <label for='api_key'>API Nyckel:</label>
                    <input type='text' id='api_key' name='api_key' required>
                    <button type='submit'>Starta Server</button>
                </form>
            </body>
            </html>";

            response.Headers.Add("Content-Type", "text/html; charset=utf-8");
            await response.WriteStringAsync(html);
            return response;
        }
    }
}

