using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JsonApiDotNetCore.Serialization.Objects;
using JsonApiDotNetCoreExample;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace JsonApiDotNetCoreExampleTests.Acceptance
{
    [Collection("WebHostCollection")]
    public sealed class NoHttpPostTests
    {
        [Fact]
        public async Task Allows_GET_Requests()
        {
            // Arrange
            const string route = "nohttppost";
            const string method = "GET";

            // Act
            var response = await MakeRequestAsync(route, method);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Rejects_POST_Requests()
        {
            // Arrange
            const string route = "nohttppost";
            const string method = "POST";

            // Act
            var response = await MakeRequestAsync(route, method);

            // Assert
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);

            var errorDocument = JsonConvert.DeserializeObject<ErrorDocument>(body);
            Assert.Single(errorDocument.Errors);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, errorDocument.Errors[0].StatusCode);
            Assert.Equal("The request method is not allowed.", errorDocument.Errors[0].Title);
            Assert.Equal("Resource does not support POST requests.", errorDocument.Errors[0].Detail);
        }

        [Fact]
        public async Task Allows_PATCH_Requests()
        {
            // Arrange
            const string route = "nohttppost";
            const string method = "PATCH";

            // Act
            var response = await MakeRequestAsync(route, method);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Allows_DELETE_Requests()
        {
            // Arrange
            const string route = "nohttppost";
            const string method = "DELETE";

            // Act
            var response = await MakeRequestAsync(route, method);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<HttpResponseMessage> MakeRequestAsync(string route, string method)
        {
            var builder = WebHost.CreateDefaultBuilder()
                .UseStartup<TestStartup>();
            var httpMethod = new HttpMethod(method);
            var server = new TestServer(builder);
            var client = server.CreateClient();
            var request = new HttpRequestMessage(httpMethod, route);
            var response = await client.SendAsync(request);
            return response;
        }
    }
}
