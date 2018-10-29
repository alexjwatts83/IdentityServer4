using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine($"disco.Error: {disco.Error}");
                Console.ReadLine();

                return;
            }

            // request token
            //var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "511536EF-F270-4058-80CA-1C89C192F69A");
            //var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");
            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "alice", "api1");

            if (tokenResponse.IsError)
            {
                Console.WriteLine($"tokenResponse.Error: {tokenResponse.Error}");
                Console.ReadLine();

                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:50419/api/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"response.StatusCode: {response.StatusCode}");
                Console.ReadLine();

                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(JArray.Parse(content));
            Console.ReadLine();

        }
    }
}
