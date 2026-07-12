using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Demo.Tests
{
     internal static class SecretManager
     {
          // azure key vault name
          private const string _kv = "kv-ms-updates";

          private static SecretClient Client { get; } = new SecretClient(
                         new Uri($"https://{_kv}.vault.azure.net/"),
                                     new DefaultAzureCredential(new DefaultAzureCredentialOptions
                                     {
                                          ExcludeEnvironmentCredential = true,
                                          ExcludeVisualStudioCredential = true,
                                          ExcludeVisualStudioCodeCredential = true,                                         
                                     }));

          // get secret from azure key vault
          internal static string GetSecret(string secretName) => Client.GetSecret(secretName).Value.Value;
     }
}
