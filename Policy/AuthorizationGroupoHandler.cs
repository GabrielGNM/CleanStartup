using Microsoft.AspNetCore.Authorization;

namespace CleanStartup.Policy
{
    public class AuthorizationGrupoHandler : AuthorizationHandler<ApiKeysValidas>
    {
        private const string tagApiKey = "ApiKey";

        private readonly ILogger _log;

        private IHttpContextAccessor _httpContextAccessor;

        public AuthorizationGrupoHandler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILoggerFactory logger)
        {
            _log = logger.CreateLogger("AuthorizationGrupoHandler");
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeysValidas requirement)
        {
            string apiKey = GetApiKey(_httpContextAccessor.HttpContext!.Request.Headers);
            if (ValidarApiKey(requirement, apiKey))
            {
                context.Succeed(requirement);
            }
            else
            {
                _log.LogWarning("ApiKey não encontrado nas chaves registradas");
                context.Fail();
            }

            return Task.CompletedTask;
        }

        private static bool ValidarApiKey(ApiKeysValidas requirement, string key)
        {
            if (requirement == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            return requirement.Contains(key);
        }

        private static string GetApiKey(IHeaderDictionary headers)
        {
            try
            {
                if (headers.ContainsKey("ApiKey"))
                {
                    return headers["ApiKey"][0];
                }
            }
            catch
            {
            }

            return string.Empty;
        }
    }
}
