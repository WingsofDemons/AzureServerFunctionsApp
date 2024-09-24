using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VmGameServerStart
{
    public class HttpTriggerVmStart
    {
        private readonly ILogger<HttpTriggerVmStart> _logger;

        public HttpTriggerVmStart(ILogger<HttpTriggerVmStart> logger)
        {
            _logger = logger;
        }

        [Function("HttpTriggerVmStart")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
