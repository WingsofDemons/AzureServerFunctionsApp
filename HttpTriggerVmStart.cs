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
            _armClient = new ArmClient(new AzureCliCredential());
        }

        [Function("HttpTriggerVmStart")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var subscriptionId = "ca5b2e2b-60d8-4085-b98c-19ed3102bc83";
            var resourceGroupName = "r.gameserver_group";
            var vmName = "r.gameserver";

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

