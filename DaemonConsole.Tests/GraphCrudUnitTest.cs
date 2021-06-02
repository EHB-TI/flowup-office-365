using Microsoft.VisualStudio.TestTools.UnitTesting;
using daemon_console;
using Microsoft.Graph;
using System.Collections.Generic;
using daemon_console.Models;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;
using daemon_console.GraphCrud;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DaemonConsole.Tests
{
    [TestClass]
    public class GraphCrudUnitTest
    {
        //[TestMethod]
        //public async void shouldReturnToken()
        //{
        //    //configuration
        //    AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");
        //    string[] scopes = new string[] { "https://graph.microsoft.com/.default" };

        //    IConfidentialClientApplication app;
        //    app = ConfidentialClientApplicationBuilder.Create(config.ClientId).WithClientSecret(config.ClientSecret).WithAuthority(new Uri(config.Authority)).Build();

        //    var httpClient = new HttpClient();
        //    AuthenticationResult result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
        //    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", result.AccessToken);
        //    Assert.Equals(httpClient.DefaultRequestHeaders.Authorization, new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", result.AccessToken));

        //}
        //[TestMethod]
        //public async Task graphCrudMethodsTests()
        //{
        //    //string subject = "Test";
        //    string eventname = "Test Event";
        //    string description = "test events";
        //    //string eventContent = "Testevent";
        //    string startTime = "2021 - 05 - 27T10: 00:00.0000000";
        //    string endTime = "2021 - 05 - 27T10: 00:00.0000000";
        //    string locationName = "Online";
        //    List<Attendee> attendeesAtCreate = new List<Attendee>();
        //    string email = " Admin@flowupdesiderius.onmicrosoft.com";
        //    string name = "Event create testen";
        //    bool allowNewTimeProposal = true;

        //    //configuration
        //    AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");
        //    var httpClient = new HttpClient();
        //    IConfidentialClientApplication app;
        //    app = ConfidentialClientApplicationBuilder.Create(config.ClientId).WithClientSecret(config.ClientSecret).WithAuthority(new Uri(config.Authority)).Build();
        //    string[] scopes = new string[] { "https://graph.microsoft.com/.default" };
        //    AuthenticationResult result = await app.AcquireTokenForClient(scopes).ExecuteAsync();

        //    // use the create function
        //    string eventId = GraphCrudMethods.createEvent(result.AccessToken, eventname, description, startTime, endTime, locationName, email, name, allowNewTimeProposal).Result;
        //    Console.WriteLine(eventId);
        //    Assert.AreNotEqual(eventId, "", true, "het event is correct aangemaakt");

        //    // send a get request 
        //    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", result.AccessToken);
        //    var responsePost = await httpClient.GetAsync($"https://graph.microsoft.com/v1.0/users/Admin@flowupdesiderius.onmicrosoft.com/events/{eventId}");
        //    // test if the user is created
        //    Assert.AreEqual<bool>(responsePost.IsSuccessStatusCode, true);

        //    //GraphCrudMethods.updateEvent(result.AccessToken, eventname, description, startTime, endTime, locationName, email, name, allowNewTimeProposal).Result;
        //    // use the delete function
        //    GraphCrudMethods.deleteEvent(result.AccessToken, eventId);

        //    // send a get request 
        //    responsePost = await httpClient.GetAsync($"https://graph.microsoft.com/v1.0/users/Admin@flowupdesiderius.onmicrosoft.com/events/{eventId}");
        //    // test if the user is deleted
        //    //Assert.AreEqual(false, responsePost.IsSuccessStatusCode);
        //    //Assert.AreEqual<Object>(responsePost.Content, false);
        //}
    }
}