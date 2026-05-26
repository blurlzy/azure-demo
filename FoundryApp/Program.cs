// dotnet add package Azure.AI.Projects --version 2.0.0-beta.2
using Azure.AI.Projects;
using Azure.AI.Extensions.OpenAI;
using Azure.Identity;
using OpenAI.Responses;

// agent name & version
const string agentName = "ZL-Demo";
const string agentVersion = "6";

// retreive foundry project endpoint from key vault
string endpoint = SecretManager.GetSecret("FoundryProjectEndpointAu");

// Connect to your project using the endpoint from your project page
// The AzureCliCredential will use your logged-in Azure CLI identity, make sure to run `az login` first
AIProjectClient projectClient = new(endpoint: new Uri(endpoint), tokenProvider: new DefaultAzureCredential());

// 
AgentReference agentReference = new(name: agentName, version: agentVersion);
// Get the client for interacting with agent responses
ProjectResponsesClient responseClient = projectClient.ProjectOpenAIClient.GetProjectResponsesClientForAgent(agentReference);


// Get the full response without streaming
// GetResponse(responseClient, "could you pls write a C# function which compares 2 dates?");

// Stream the agent response
await StreamAgentResponseAsync(responseClient, "could you pls write a C# function which compares 2 dates?");


// stream agent responses with usage and completion information
async Task StreamAgentResponseAsync(ProjectResponsesClient client, string userMessage)
{
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
               Console.ForegroundColor = ConsoleColor.Green;

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
     // return response
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
     ResponseResult response = client.CreateResponse(
         "could you pls write a C# function which compares 2 dates?"
     );
#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

     Console.WriteLine(response.GetOutputText());
}