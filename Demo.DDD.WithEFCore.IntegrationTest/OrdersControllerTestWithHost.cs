namespace Demo.DDD.WithEFCore.IntegrationTest
{
    using Demo.DDD.WithEFCore.API;    
    using Microsoft.AspNetCore.Mvc.Testing;
    using System.Net;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Xunit;

    /***********************************************************************
     * When creating a test project for an app, separate the unit tests 
     * from the integration tests into different projects. This helps 
     * ensure that infrastructure testing components aren't accidentally 
     * included in the unit tests. Separation of unit and integration 
     * tests also allows control over which set of tests are run.
     * 
     * Original Article: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-5.0
     ***********************************************************************/
    public class OrdersControllerTestWithHost:
        IClassFixture<WebApplicationFactory<Startup>>
        // IClassFixture<AppInstance>
    {
        /*
         * Class Fixtures: When to use: when you want to create a single test context 
         * and share it among all the tests in the class, and have it cleaned up after 
         * all the tests in the class have finished.
         * 
         * Collection Fixtures: When to use: when you want to create a single test context 
         * and share it among tests in several test classes, and have it cleaned up after 
         * all the tests in the test classes have finished.
         */

        private readonly WebApplicationFactory<Startup> _factory;

        /* public OrdersControllerTest()
        {
            var server = new TestServer(new WebHostBuilder()
               .UseEnvironment("Development")
               .UseStartup<Startup>());

            this.client = server.CreateClient();
        } */

        public OrdersControllerTestWithHost(WebApplicationFactory<Startup> factory)
        {
            // _instance = factory;
            this._factory = factory;

            /*var server = new TestServer(new WebHostBuilder()              
              .UseEnvironment("Development")
              .UseTestServer()
              .UseStartup<Startup>());

            this.client = server.CreateClient(); */
        }

        [Fact]
        public async Task GetOrderByIdAsync()
        {
            // ARRANGE
            // var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/v1/orders/123");
            var client = _factory.CreateClient();
                        
            // ACT            
            var response = await client.GetAsync("/api/v1/orders/123");

            // ASSERT
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal($"{MediaTypeNames.Application.Json}; charset=utf-8", response.Content.Headers.ContentType.ToString());            
        }
    }
}
