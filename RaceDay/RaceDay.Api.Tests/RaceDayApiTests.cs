using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RaceDay.Api.Data;
using RaceDay.Contracts;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class RaceDayApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public RaceDayApiTests(WebApplicationFactory<Program> factory)
        {
            // Replaces the real SQL Server DbContext with a clean in-memory database for testing
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    ServiceDescriptor descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<RaceDayDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<RaceDayDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("RaceDayTestDb_" + Guid.NewGuid().ToString());
                    });
                });
            });
        }

        
        // Test 1: User Registration Success 
        
        [Fact]
        public async Task Register_ValidParticipant_Returns201Created()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            RegisterRequest request = new RegisterRequest
            {
                FullName = "Kagiso Rabada",
                Email = "kagiso.test@raceday.co.za",
                Password = "Password123!",
                Role = "Participant"
            };

            // Act
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/register", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        
        // Test 2: Login Failure on Incorrect Credentials
        
        [Fact]
        public async Task Login_InvalidCredentials_Returns401Unauthorized()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            LoginRequest request = new LoginRequest
            {
                Email = "nonexistent.user@raceday.co.za",
                Password = "WrongPassword999!"
            };

            // Act
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/login", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        
        // Test 3: Unauthenticated Access to Protected Route 
        
        [Fact]
        public async Task CreateEvent_UnauthenticatedUser_Returns401Unauthorized()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            CreateEventRequest request = new CreateEventRequest
            {
                EventName = "Comrades Marathon",
                Description = "Iconic ultra marathon",
                EventDate = DateTime.UtcNow.AddDays(60),
                Location = "Durban",
                DistanceKm = 89.0m,
                EventType = "Run",
                BannerUrl = null
            };

            // Act
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/events", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        
        // Test 4: Role-Based Rejection 
        
        [Fact]
        public async Task CreateEvent_WhenUserIsParticipant_Returns403Forbidden()
        {
            // Arrange
            WebApplicationFactoryClientOptions options = new WebApplicationFactoryClientOptions
            {
                HandleCookies = true
            };
            HttpClient client = _factory.CreateClient(options);

            string participantEmail = "runner_" + Guid.NewGuid().ToString() + "@raceday.co.za";

            // 1. Register as Participant
            RegisterRequest reg = new RegisterRequest
            {
                FullName = "Sipho Runner",
                Email = participantEmail,
                Password = "Password123!",
                Role = "Participant"
            };
            await client.PostAsJsonAsync("/api/auth/register", reg);

            // 2. Login to initialize Participant session
            LoginRequest login = new LoginRequest
            {
                Email = participantEmail,
                Password = "Password123!"
            };
            await client.PostAsJsonAsync("/api/auth/login", login);

            // Act
            CreateEventRequest eventRequest = new CreateEventRequest
            {
                EventName = "Trail Challenge",
                Description = "Fun trail run",
                EventDate = DateTime.UtcNow.AddDays(14),
                Location = "Polokwane",
                DistanceKm = 10.0m,
                EventType = "Run",
                BannerUrl = null
            };
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/events", eventRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        
        // Test 5: Role-Based Success - Organiser can create Events 
        
        [Fact]
        public async Task CreateEvent_WhenUserIsOrganiser_Returns201Created()
        {
            // Arrange
            WebApplicationFactoryClientOptions options = new WebApplicationFactoryClientOptions
            {
                HandleCookies = true
            };
            HttpClient client = _factory.CreateClient(options);

            string organiserEmail = "organiser_" + Guid.NewGuid().ToString() + "@raceday.co.za";

            // 1. Register as Organiser
            RegisterRequest reg = new RegisterRequest
            {
                FullName = "Ansie Organiser",
                Email = organiserEmail,
                Password = "Password123!",
                Role = "Organiser"
            };
            await client.PostAsJsonAsync("/api/auth/register", reg);

            // 2. Login to initialize Organiser session
            LoginRequest login = new LoginRequest
            {
                Email = organiserEmail,
                Password = "Password123!"
            };
            await client.PostAsJsonAsync("/api/auth/login", login);

            // Act
            CreateEventRequest eventRequest = new CreateEventRequest
            {
                EventName = "Cape Town 10k",
                Description = "Coastal road race",
                EventDate = DateTime.UtcNow.AddDays(30),
                Location = "Cape Town",
                DistanceKm = 10.0m,
                EventType = "Run",
                BannerUrl = null
            };
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/events", eventRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}