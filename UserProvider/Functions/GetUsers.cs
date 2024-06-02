using Data.Contexts;
using Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace UserProvider.Functions
{
    public class UserFunctions
    {
        private readonly ILogger<UserFunctions> _logger;
        private readonly DataContext _context;

        public UserFunctions(ILogger<UserFunctions> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetUsers")]
        public async Task<HttpResponseData> GetUsers([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            var users = await _context.Users.ToListAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(users);
            return response;
        }
    }
}
