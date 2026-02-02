using System;
using System.Net;
using System.Text;
using System.Text.Unicode;

namespace LoadBalancer
{
    internal class Program
    {
        private static List<string> backendServerUrls = new List<string> { "http://localhost:8081", "http://localhost:8082", "http://localhost:8083" };
        private static List<string> activeBackendServerUrls = new List<string>();
        private static Timer? _timer = null;
        static async Task Main(string[] args)
        {
            //client -> load balancer -> backend server -> load balancer -> client
            activeBackendServerUrls = backendServerUrls.ToList();
            StartHealthChecker();

            var httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:8080/");

            httpListener.Start();
            var nextBackendServer = 0;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Listening for requests...");
                var context = await httpListener.GetContextAsync();
                var request = context.Request;

                //return some info
                Console.WriteLine($"Request at [{DateTime.Now.ToString("O")}]");
                Console.WriteLine($"Request received from {request.RemoteEndPoint}");
                Console.WriteLine($"{request.HttpMethod} / HTTP/{request.ProtocolVersion.Major}.{request.ProtocolVersion.Minor}");
                Console.WriteLine($"Host: {request.Url.Host}");
                Console.WriteLine($"User-Agent: {request.UserAgent}");
                Console.WriteLine($"Accept: {(request.AcceptTypes != null ? string.Join(',', request.AcceptTypes) : "")}");

                //make a request to backend server
                Console.WriteLine();
                var backendServerIndex = nextBackendServer % activeBackendServerUrls.Count;
                var backendServerUrl = activeBackendServerUrls[backendServerIndex];
                Console.WriteLine($"Backend request to server {backendServerUrl}");
                nextBackendServer++;

                var httpClient = new HttpClient();
                var backendRequest = await httpClient.GetAsync(backendServerUrl);
                var brResponse = await backendRequest.Content.ReadAsStringAsync();

                //show the response
                if(backendRequest.IsSuccessStatusCode)
                    Console.Write($"OK response from {backendServerUrl}\n");

                //return the response to client
                var buffer = Encoding.UTF8.GetBytes(brResponse);
                var response = context.Response;
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);
                output.Close();

                Console.WriteLine();
            }

            httpListener.Stop();
        }

        static void StartHealthChecker()
        {
            Console.WriteLine("Health Checker running...");
            _timer = new Timer(async (stateInfo) => {
                Console.WriteLine($"{DateTime.Now.ToString("O")} checking health...");
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromMilliseconds(500);
                for (int i = 0; i < backendServerUrls.Count;i++)
                {
                    Console.WriteLine($"Checking - {backendServerUrls[i]} -");
                    var healthCheckUrl = backendServerUrls[i] + "/healthcheck.html";
                    HttpResponseMessage? hcRequest = null;
                    try
                    {
                        hcRequest = await client.GetAsync(healthCheckUrl);
                        if (!hcRequest.IsSuccessStatusCode)
                        {
                            activeBackendServerUrls.Remove(backendServerUrls[i]);
                        }
                        else
                        {
                            Console.WriteLine($"{DateTime.Now.ToString("O")}: OK");
                            
                            if (!activeBackendServerUrls.Contains(backendServerUrls[i]))
                                activeBackendServerUrls.Add(backendServerUrls[i]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        activeBackendServerUrls.Remove(backendServerUrls[i]);
                    }
                }

                Console.WriteLine($"Active Servers: {string.Join(",", activeBackendServerUrls)}");

            }, null, 1000, 5000);
            
        }
    }
}
