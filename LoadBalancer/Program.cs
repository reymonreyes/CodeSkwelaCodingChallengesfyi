using System.Net;
using System.Text;
using System.Text.Unicode;

namespace LoadBalancer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //client -> load balancer -> backend server -> load balancer -> client

            var httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:8080/");

            while (true)
            {
                httpListener.Start();
                Console.WriteLine("Listening for requests...");
                var context = await httpListener.GetContextAsync();
                var request = context.Request;

                //return some info
                Console.WriteLine($"Request received from {request.RemoteEndPoint}");
                Console.WriteLine($"{request.HttpMethod} / HTTP/{request.ProtocolVersion.Major}.{request.ProtocolVersion.Minor}");
                Console.WriteLine($"Host: {request.Url.Host}");
                Console.WriteLine($"User-Agent: {request.UserAgent}");
                Console.WriteLine($"Accept: {(request.AcceptTypes != null ? string.Join(',', request.AcceptTypes) : "")}");

                //make a request to backend server
                var httpClient = new HttpClient();
                var backendRequest = await httpClient.GetAsync("http://localhost:8081/");
                var brResponse = await backendRequest.Content.ReadAsStringAsync();

                //show the response
                Console.Write(brResponse);

                //return the response to client
                var buffer = Encoding.UTF8.GetBytes(brResponse);
                var response = context.Response;
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);
                output.Close();

                httpListener.Stop();
            }
        }
    }
}
