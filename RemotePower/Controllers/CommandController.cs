using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RemotePower.Controllers;

public class Command
{
    public string id { get; set; }
    public string command { get; set; }
}



[ApiController]
[Route("[controller]/[action]")]
public class CommandController : ControllerBase
{
    private readonly ILogger<CommandController> _logger;
    private CosmosClient _client;
    private Container _container;
    private Database _database;

    public CommandController(ILogger<CommandController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<string> GetCommand()
    {
        _client = new CosmosClient(
            "https://aranaa1.documents.azure.com:443/",
            "Vwi6BNVplLrUJTN10haiWTJZO9rHCoM549gS9Q11dRaCMaQAGgTQwAh0J1HWuXhcYKd35SgH6nAwACDbEVcE8w=="
        );
        _database = _client.GetDatabase("RemoteControlDB");
        _container = _database.GetContainer("commands");
        var sql = "SELECT * FROM c WHERE c.id = @id";

        var query = new QueryDefinition(
                sql
            )
            .WithParameter("@id", "1");

        using var feed = _container.GetItemQueryIterator<dynamic>(
            query
        );
        var response = await feed.ReadNextAsync();
        // Console.WriteLine($"[{response.StatusCode}]\t{1}\t{response.RequestCharge} RUs");
        if (response == null || response.StatusCode != HttpStatusCode.OK)
        {
            throw new ApplicationException("couldn't get command.");
        }

        var result = (((JObject)response.Resource.ToList()[0])["command"]).ToString();
        return $"[{result}]";
    }

    [HttpGet]
    public async Task<string> PostCommand(bool input, bool alreadyOn = false)
    {
        _client = new CosmosClient(
            "https://aranaa1.documents.azure.com:443/",
            "Vwi6BNVplLrUJTN10haiWTJZO9rHCoM549gS9Q11dRaCMaQAGgTQwAh0J1HWuXhcYKd35SgH6nAwACDbEVcE8w=="
        );
        _database = _client.GetDatabase("RemoteControlDB");
        _container = _database.GetContainer("commands");
        var command = new
        {
            id = "1",
            command = input.ToString()
        };
        var response = await _container.UpsertItemAsync(command);
        // Console.WriteLine($"[{response.StatusCode}]\t{1}\t{response.RequestCharge} RUs");
        if (response == null || response.StatusCode != HttpStatusCode.OK)
        {
            throw new ApplicationException("couldn't update command.");
        }
        return "[Success]";
    }
}