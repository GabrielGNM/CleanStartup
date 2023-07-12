using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace CleanStartup.Policy
{
    public class ApiKeyAuthorizationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string tagApiKey = "ApiKey";
        private readonly IConfiguration _configuration;
        public ApiKeyAuthorizationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                                  ILoggerFactory logger,
                                  UrlEncoder encoder,
                                  ISystemClock clock,
                                  IConfiguration configuration) // Inject IConfiguration
    : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Method.Equals("OPTIONS", StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (!Request.Headers.TryGetValue("ApiKey", out var apiKeyHeaderValues))
            {
                Response.StatusCode = 401; // Unauthorized status code
                Response.WriteAsync("Missing ApiKey header."); // Write error message to the response
                return Task.FromResult(AuthenticateResult.Fail("Missing ApiKey header."));
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            // Validate the providedApiKey here. If invalid, return AuthenticateResult.Fail().
            if (!IsValidApiKey(providedApiKey))
            {
                Response.StatusCode = 401; // Unauthorized status code
                Response.WriteAsync("Invalid ApiKey."); // Write error message to the response
                return Task.FromResult(AuthenticateResult.Fail("Invalid ApiKey."));
            }


            // If valid, create a ClaimsPrincipal.
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, providedApiKey) }, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private bool IsValidApiKey(string apiKey)
        {
            var validApiKey = _configuration["ApiKey"];
            return apiKey == validApiKey;
        }

    }
}
