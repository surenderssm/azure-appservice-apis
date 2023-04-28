// See https://aka.ms/new-console-template for more information
using Microsoft.Identity.Client;

Console.WriteLine("Welcome to the world of appservices!");

#region configurations
// TODO
/* Pre-Req : 
 * Create identity (AAD App/SPN)
 * Assign one of the relevant role to the idetity on the scoped appservice/function/logicapp [Website Contributor/Contributor/ Owner]
 *  */
const string AppServiceScmBaseUri = "https://[yoursitename].scm.azurewebsites.net";
const string TenantId = "TENANT_ID";
const string ClientId = "CLIENT_ID";
// please note cert based credentials is also available, incase one goes with secret, please ensure this comes from keyvault and follows all the security guidelines of your group.
const string ClientSecret = "CLIENT_SECRET";

#endregion configurations

// Please note resources like Client,ConfidentialClientApp in this case, should be initialized once in lifecyle of the app.
var Client = new HttpClient();
Client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

var ConfidentialClientApp = ConfidentialClientApplicationBuilder.Create(ClientId)
                    .WithClientSecret(ClientSecret)
                    .WithTenantId(TenantId)
                    .Build();

GetDeployments().Wait();
Console.ReadLine();

// Uses kudu API (.scm) to get deployments using AAD auth
// https://github.com/projectkudu/kudu/wiki/REST-API
async Task GetDeployments()
{
    try
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{AppServiceScmBaseUri}/api/deployments")
        };

        // AAD token via app/spn/identity
        var token = await GetToken();
        requestMessage.Headers.Add("Authorization", $"Bearer {token}");
       
        var response = await Client.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine($" ----RequestUri---- : `{requestMessage.RequestUri}`");
        Console.WriteLine($" ----ResponseStatusCode---- : {(int)response.StatusCode}");
        Console.WriteLine($" ----Response----");
        Console.WriteLine($"{responseString}");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
}

// GET accesstoken from aad app
async Task<string> GetToken()
{
    // audience : https://management.core.windows.net
    var scopes = new[] { "https://management.core.windows.net/.default" };
    var tokenResult = await ConfidentialClientApp.AcquireTokenForClient(scopes).ExecuteAsync();
    return tokenResult.AccessToken;
}
