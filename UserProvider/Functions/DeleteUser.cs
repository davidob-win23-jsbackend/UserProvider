using Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace UserProvider.Functions
{
    public class DeleteUser(ILogger<DeleteUser> logger, DataContext context)
    {
        private readonly ILogger<DeleteUser> _logger = logger;
        private readonly DataContext _context = context;

        [Function("DeleteUser")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "users/{id}")] HttpRequestData req, string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                return notFoundResponse;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}
