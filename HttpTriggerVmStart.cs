using Azure;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VmGameServerStart
{
    public class HttpTriggerVmStart
    {
        private readonly ILogger<HttpTriggerVmStart> _logger;
        private readonly ArmClient _armClient;

        public HttpTriggerVmStart(ILogger<HttpTriggerVmStart> logger)
        {
            _logger = logger;
            // Använder Managed Identity Credential för autentisering
            _armClient = new ArmClient(new ManagedIdentityCredential());
        }

        [Function("HttpTriggerVmStart")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Läs formuläret asynkront
            var form = await req.ReadFormAsync();

            // Hämta api_key och kontrollera om den finns
            string? apiKey = form["api_key"].ToString(); // Använd ToString() för att säkerställa att vi får en sträng

            // Hämta den giltiga API-nyckeln från miljövariabler
            string? validApiKey = Environment.GetEnvironmentVariable("VALID_API_KEY");


            // Kontrollera om den giltiga API-nyckeln är null
            if (validApiKey == null)
            {
                _logger.LogError("API key environment variable is not set.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError); // Eller ett lämpligt felmeddelande
            }

            // Kontrollera om apiKey är null eller tom
            if (string.IsNullOrEmpty(apiKey) || apiKey != validApiKey)
            {
                // Returnera 401 Unauthorized om API-nyckeln är felaktig
                return new UnauthorizedResult();
            }

            var subscriptionId = Environment.GetEnvironmentVariable("SUBSCRIPTION_ID");
            var resourceGroupName = Environment.GetEnvironmentVariable("RESOURCE_GROUP_NAME");
            var vmName = Environment.GetEnvironmentVariable("VM_NAME");


            // Skapar resurs-ID för den virtuella maskinen
            var vmResourceId = VirtualMachineResource.CreateResourceIdentifier(subscriptionId, resourceGroupName, vmName);
            var vm = _armClient.GetVirtualMachineResource(vmResourceId);

            try
            {
                // Startar den virtuella maskinen
                var startOperation = await vm.PowerOnAsync(WaitUntil.Completed);
                return new OkObjectResult($"Starting VM: {vmName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error starting VM: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}

