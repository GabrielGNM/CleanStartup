using Microsoft.AspNetCore.Authorization;

namespace CleanStartup.Policy
{
    public class ApiKeysValidas : IAuthorizationRequirement
    {
        private const string SECRET_NAME = "ApiKey";

        public bool Contains(string key)
        {
            return GetApis()?.Contains(key.ToLower()) ?? false;
        }

        private IEnumerable<string> GetApis()
        {
            string text = VariavelAmbiente.Get("ApiKey");
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            return (from k in text.ToLower().Split(',')
                    where !string.IsNullOrEmpty(k)
                    select k).ToList();
        }
    }
}