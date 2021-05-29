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
using System.Text;
using System.Threading.Tasks;

namespace daemon_console.GraphCrud
{
    public class GraphCrudMethods
    {
        AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

        //Get events of specific user with Graph api
        public static async void getEvents(string accessToken, Action<JObject> Display)
        {
            var httpClient = new HttpClient();
            var apiCaller = new ProtectedApiCallHelper(httpClient);
            await apiCaller.CallWebApiAndProcessResultASync($"https://graph.microsoft.com/v1.0/users/Admin@flowupdesiderius.onmicrosoft.com/events", accessToken, Display);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);

        }


        //Create events of specific user with graph api
        public static async void createEvent(string accessToken, string subject, string eventContent, string startTime, string endTime, string locationName, List<Attendee> attendees, bool allowNewTimeProposal)
        {
            var httpClient = new HttpClient();

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
                    DateTime = startTime,
                    TimeZone = "Romance Standard Time"
                },
                End = new Microsoft.Graph.DateTimeTimeZone
                {
                    DateTime = endTime,
                    TimeZone = "Romance Standard Time"
                },
                Location = new Location
                {
                    DisplayName = locationName
                },

                AllowNewTimeProposals = allowNewTimeProposal,
                TransactionId = generateTransactionId()

            };

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);

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
        public static async void deleteEvent(string accessToken, string eventId)
        {
            var httpClient = new HttpClient();
            var responseDelete = await httpClient.DeleteAsync($"https://graph.microsoft.com/v1.0/users/Admin@flowupdesiderius.onmicrosoft.com/events/{eventId}");

            if (responseDelete.IsSuccessStatusCode)
                Console.WriteLine("Event has successfully been deleted!");
            else
                Console.WriteLine(responseDelete.StatusCode);
        }


        public static async void unsubscribeEvent(string accessToken,string userName, string eventId)
        {
            var httpClient = new HttpClient();
            var responseDelete = await httpClient.DeleteAsync($"https://graph.microsoft.com/v1.0/users/{userName}/events/{eventId}");

            if (responseDelete.IsSuccessStatusCode)
                Console.WriteLine("Event has successfully been removed for user!");
            else
                Console.WriteLine(responseDelete.StatusCode);
        }



        public static string generateTransactionId()
        {

            int length = 28;

            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }

            return str_build.ToString();
        }
    }
}
