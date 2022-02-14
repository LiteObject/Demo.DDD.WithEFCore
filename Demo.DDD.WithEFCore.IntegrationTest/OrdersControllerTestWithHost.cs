namespace Demo.DDD.WithEFCore.IntegrationTest
{
    using Demo.DDD.WithEFCore.API;
    using Demo.DDD.WithEFCore.Entities.Enums;
    using Humanizer;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.VisualStudio.TestPlatform.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    /***********************************************************************
     * When creating a test project for an app, separate the unit tests 
     * from the integration tests into different projects. This helps 
     * ensure that infrastructure testing components aren't accidentally 
     * included in the unit tests. Separation of unit and integration 
     * tests also allows control over which set of tests are run.
     * 
     * Original Article: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-5.0
     ***********************************************************************/
    public class OrdersControllerTestWithHost :
        IClassFixture<WebApplicationFactory<Startup>>
    // IClassFixture<AppInstance>
    {

        private readonly ITestOutputHelper _output;

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

        public OrdersControllerTestWithHost(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            // _instance = factory;
            this._factory = factory ?? throw new ArgumentNullException($"{nameof(factory)} cannot be null.");
            this._output = output ?? throw new ArgumentNullException($"{nameof(output)} cannot be null.");
        }

        [Fact]
        public async Task OrderById_Should_Return_200_Ok()
        {
            // ARRANGE
            // var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/v1/orders/123");
            var client = _factory.CreateClient();

            // ACT            
            var response = await client.GetAsync("/api/orders/123");

            // ASSERT
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal($"{MediaTypeNames.Application.Json}; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task OrderById_Should_Return_404_NotFound()
        {
            // ARRANGE
            // var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/v1/orders/123");
            var client = _factory.CreateClient();

            // ACT            
            var response = await client.GetAsync("/api/orders/456");

            // ASSERT            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal($"{MediaTypeNames.Text.Plain}; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task UpdateOrder_Should_Return_404_NotFound()
        {
            // ARRANGE
            var client = _factory.CreateClient();
            var orderDto = new API.DTO.Order
            {
                Id = 456,
                OrderDate = DateTime.Today,
                ShippingAddress = new API.DTO.Address(),
                LineItems = GetLineItems(),
            };

            var orderDtoJson = System.Text.Json.JsonSerializer.Serialize(orderDto);
            var httpContent = new StringContent(orderDtoJson, System.Text.Encoding.UTF8, MediaTypeNames.Application.Json);

            // ACT
            using var response = await client.PutAsync("/api/orders/456", httpContent);

            // ASSERT            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal($"{MediaTypeNames.Text.Plain}; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        // [Fact]
        public async Task PatchOrder_Should_Return_NoContent()
        {
            // ARRANGE
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("api-version", "2.0");
            var jsonPatchDoc = new JsonPatchDocument<API.DTO.Order>();
            jsonPatchDoc.Replace(d => d.Note, "new note for test");
            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(jsonPatchDoc.Operations);
            _output.WriteLine($"Json Payload: {jsonPayload}");

            // ACT
            using var response = await client.PatchAsync("/api/orders/456", new StringContent(jsonPayload, System.Text.Encoding.Unicode, MediaTypeNames.Application.Json));

            // ASSERT
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        private List<API.DTO.Order> GetTestOrders()
        {
            return new List<API.DTO.Order>
            {
                new()
                {
                    Note = "1 day older than Today",
                    OrderDate = DateTime.Today.AddDays(-1),
                    ShippingAddress = new API.DTO.Address("123 Street", "House #456", "Frisco", "Texas", "75033"),
                    LineItems = GetLineItems(),
                    Status = OrderStatus.ProcessingHalted.ToString(),
                },
                new()
                {
                    Note = "5 Days older than today",
                    OrderDate = DateTime.Today.AddDays(-5),
                    ShippingAddress = new API.DTO.Address("456 Street", "House #789", "Dallas", "Texas", "75248"),
                    LineItems = GetLineItems(),
                    Status = OrderStatus.ProcessingStarted.ToString()
                },
                new()
                {
                    Note = "7 Days older than today",
                    OrderDate = DateTime.Today.AddDays(-7),
                    ShippingAddress = new API.DTO.Address("456 Street", "House #789", "Dallas", "Texas", "75248"),
                    LineItems = GetLineItems(),
                    Status = OrderStatus.ProcessingHalted.ToString()
                },
                new()
                {
                    Note = "7 Days older than today",
                    OrderDate = DateTime.Today.AddDays(-7),
                    ShippingAddress = new API.DTO.Address("456 Street", "House #789", "Dallas", "Texas", "75248"),
                    LineItems = GetLineItems(),
                    Status = OrderStatus.Created.ToString()
                }
            };
        }

        private List<API.DTO.LineItem> GetLineItems()
        {
            return new List<API.DTO.LineItem>
            {
                new API.DTO.LineItem { Name = "Apple", UnitPrice = 1.50, Quantity = 5 },
                new API.DTO.LineItem { Name = "Orange", UnitPrice = 1.65, Quantity = 2 },
                new API.DTO.LineItem { Name = "Mango", UnitPrice = 1.75, Quantity = 5 },
            };
        }
    }
}
