using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace RemotePower.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class CommandController : ControllerBase
{
    private readonly ILogger<CommandController> _logger;
    private readonly string connectionString = "Server=tcp:aranaa1.database.windows.net,1433;Initial Catalog=aranaa1;Persist Security Info=False;User ID=arana;Password=alaska@2";
    public CommandController(ILogger<CommandController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public string GetCommand()
    {
        string procedureName = "GetCommand";
        var result = new List<bool>();
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bool res = bool.Parse(reader[0].ToString());
                        result.Add(res);
                    }
                }
            }
            connection.Close();
        }

        if (result.Count == 0)
        {
            throw new ApplicationException("Command not found");
        }
        return $"[{result[0]}]";
    }
    
    [HttpGet]
    public string PostCommand(bool input)
    {
        string procedureName = "PostCommand";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using (SqlCommand command = new SqlCommand(procedureName, connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@command", input));
            command.ExecuteReader();
        }
        connection.Close();
        return "[Success]";
    }
}
