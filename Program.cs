// See https://aka.ms/new-console-template for more information
using Microsoft.Identity.Client;

Console.WriteLine("Welcome to the world of appservices!");

// TODO : replace it with your appp [ yoursite == yourapp]
const string AppServiceScmBaseUri =  "https://yoursite.scm.azurewebsites.net";
// Once in a lifetime of the app
var Client = new HttpClient();
Client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

GetDeployments().Wait();
Console.ReadLine();

async Task GetDeployments()
{
    try
    {
        var requestMessage = new HttpRequestMessage();
        requestMessage.Method = HttpMethod.Get;
        requestMessage.RequestUri = new Uri($"{AppServiceScmBaseUri}/api/deployments");
        var token = await GetToken();
        requestMessage.Headers.Add("Authorization", $"Bearer {token}");
        var response = await Client.SendAsync(requestMessage);
        var responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseString);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
}

async Task<string> GetToken()
{
    // TODO : replace it with your values
    var clientId = "client_id";
    var clientSecret = "client_secret";
    // TODO : this is for corp tenant, get the relevant value for your tenant
    var authority = "https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47";
   
    // TODO : This should be created once in a lifetime of the app
    var app = ConfidentialClientApplicationBuilder.Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(new Uri(authority))
                    .Build();

    var scopes = new[] { "https://management.core.windows.net/.default" };

    var tokenResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();
    return tokenResult.AccessToken;
}
