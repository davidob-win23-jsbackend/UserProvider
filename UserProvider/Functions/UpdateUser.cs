using Data.Contexts;
using Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace UserProvider.Functions
{
    public class UpdateUser(ILogger<UpdateUser> logger, DataContext context)
    {
        private readonly ILogger<UpdateUser> _logger = logger;
        private readonly DataContext _context = context;

        [Function("UpdateUser")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "users/{id}")] HttpRequestData req, string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                return notFoundResponse;
            }

            var content = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedUser = JsonConvert.DeserializeObject<ApplicationUser>(content);

            if (updatedUser == null)
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Invalid user data.");
                return badRequestResponse;
            }

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(user);
            return response;
        }
    }
}
