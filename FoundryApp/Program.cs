// dotnet add package Azure.AI.Projects --version 2.0.0-beta.2
using Azure.AI.Projects;
using Azure.AI.Extensions.OpenAI;
using Azure.Identity;
using OpenAI.Responses;

// agent name & version
const string agentName = "ms-learn";
const string agentVersion = "6";

// retreive foundry project endpoint from key vault
string endpoint = SecretManager.GetSecret("FoundryProjectEndpointAu");

// Connect using a service principal (app registration)
string tenantId     = SecretManager.GetSecret("TenantId");
string clientId     = SecretManager.GetSecret("FoundryProjClientId");
string clientSecret = SecretManager.GetSecret("FoundryProjClientSecret");

// "Service principal authentication isn't supported for the Fabric data agent."
var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
// interactive user login (OBO-compatible)
// var credential = new InteractiveBrowserCredential();

AIProjectClient projectClient = new(endpoint: new Uri(endpoint), tokenProvider: credential);

// get the agent reference for the agent you want to interact with, you can find this information on your project page under the "Agents" tab
AgentReference agentReference = new(name: agentName, version: agentVersion);
// Get the client for interacting with agent responses
ProjectResponsesClient responseClient = projectClient.ProjectOpenAIClient.GetProjectResponsesClientForAgent(agentReference);


// Get user message from console input
Console.ForegroundColor = ConsoleColor.Green;
Console.Write($"Message the agent ({ agentName }) >>>: ");
string userMessage = Console.ReadLine() ?? string.Empty;

if (string.IsNullOrWhiteSpace(userMessage))
{
     Console.Error.WriteLine("No message entered. Exiting.");
     return;
}


// Stream the agent response
await StreamAgentResponseAsync(responseClient, userMessage);

// Get the full response without streaming
// GetResponse(responseClient, userMessage);

// stream agent responses with usage and completion information
async Task StreamAgentResponseAsync(ProjectResponsesClient client, string userMessage)
{
     Console.ForegroundColor = ConsoleColor.Gray;
     Console.WriteLine("Streaming agent response...\n");

#pragma warning disable OPENAI001
     var streamingResponse = client.CreateResponseStreamingAsync(userMessage);

     await foreach (var update in streamingResponse)
     {

          // Handle text delta chunks
          if (update is StreamingResponseOutputTextDeltaUpdate textDelta)
          {
               Console.Write(textDelta.Delta);
          }

          // Handle usage information and completion
          if (update is StreamingResponseCompletedUpdate completed)
          {
               // can we add different color for the completion status?
               Console.ForegroundColor = ConsoleColor.Yellow;

               if (completed.Response.Usage is { } usage)
               {
                    Console.WriteLine("\n\n--- Usage Information ---");
                    Console.WriteLine($"Input tokens: {usage.InputTokenCount}");
                    Console.WriteLine($"Output tokens: {usage.OutputTokenCount}");
                    Console.WriteLine($"Total tokens: {usage.TotalTokenCount}");
               }

               Console.WriteLine($"\n\n--- Completion Status ---");
               Console.WriteLine($"Finish reason: {completed.Response.Status}");
               Console.WriteLine("Done!");
               Console.ResetColor();
          }
     }
#pragma warning restore OPENAI001
}

// get the full response without streaming
void GetResponse(ProjectResponsesClient client, string userMessage)
{
     Console.WriteLine("Getting agent response...\n");
#pragma warning disable OPENAI001
     ResponseResult response = client.CreateResponse(userMessage);
#pragma warning restore OPENAI001

     Console.WriteLine(response.GetOutputText());
}