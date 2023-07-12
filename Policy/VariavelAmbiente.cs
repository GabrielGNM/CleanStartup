namespace CleanStartup.Policy
{
    internal class VariavelAmbiente
    {
        internal static string Get(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine) ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Process) ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User) ?? Environment.GetEnvironmentVariable(variableName);
        }
    }
}
