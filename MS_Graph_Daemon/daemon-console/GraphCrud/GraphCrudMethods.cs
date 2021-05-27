using daemon_console.Models;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates; //Only import this if you are using certificate
using System.Threading.Tasks;

namespace daemon_console.GraphCrud
{
    public class GraphCrudMethods
    {
        AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

        //Get events of specific user with Graph api
        public static async void getEvents(HttpClient httpClient, string accessToken, Action<JObject> Display)
        {
            var apiCaller = new ProtectedApiCallHelper(httpClient);
            await apiCaller.CallWebApiAndProcessResultASync($"https://graph.microsoft.com/v1.0/users/Admin@flowupdesiderius.onmicrosoft.com/events", accessToken, Display);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);

        }

        
        //Create events of specific user with graph api
        public static async void createEvent(HttpClient httpClient, string accessToken, string subject, string eventContent, string startTime, string endTime, string locationName, List<Attendee> attendees, bool allowNewTimeProposal)
        {
            // Initialize the content of the post request 
            var @event = new Event
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = eventContent
                },
                Start = new Microsoft.Graph.DateTimeTimeZone
                {
                    DateTime = "2017-04-15T12:00:00",
                    TimeZone = "Pacific Standard Time"
                },
                End = new Microsoft.Graph.DateTimeTimeZone
                {
                    DateTime = "2017-04-15T12:00:00",
                    TimeZone = "Pacific Standard Time"
                },
                Location = new Location
                {
                    DisplayName = locationName
                },
                

                Attendees = new List<Attendee>()
                    {
        new Attendee
        {
            EmailAddress = new EmailAddress
            {
                Address = "samanthab@contoso.onmicrosoft.com",
                Name = "Samantha Booth"
            },
            Type = AttendeeType.Required
        } },
                AllowNewTimeProposals = allowNewTimeProposal,
                TransactionId = "7E163156-7762-4BEB-A1C6-729EA81755A7"

            };

            string eventAsJson = JsonConvert.SerializeObject(@event);
            var content = new StringContent(eventAsJson);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            // Send the request
            var responsePost = await httpClient.PostAsync($"https://graph.microsoft.com/v1.0/users/Admin@flowupdesiderius.onmicrosoft.com/events", content);

            if (responsePost.IsSuccessStatusCode)
                Console.WriteLine("Event has been added successfully!");
            else
                Console.WriteLine(responsePost.StatusCode);

        }

        //Delete events of a specific user with GrapApi
        public static async void deleteEvent(HttpClient httpClient, string accessToken, string eventId)
        {
            var responseDelete = await httpClient.DeleteAsync($"https://graph.microsoft.com/v1.0/users/Admin@flowupdesiderius.onmicrosoft.com/events/{eventId}");

            if (responseDelete.IsSuccessStatusCode)
                Console.WriteLine("Event has successfully been deleted!");
            else
                Console.WriteLine(responseDelete.StatusCode);
        }
    }
}
