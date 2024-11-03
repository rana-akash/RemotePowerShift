using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace RemotePower.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class CommandController : ControllerBase
{
    private readonly ILogger<CommandController> _logger;

    private readonly string connectionString =System.Environment.GetEnvironmentVariable("SQLCONNSTR_ConString");

    public CommandController(ILogger<CommandController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public string GetCommand()
    {
        var procedureName = "GetCommand";
        var result = new List<bool>();
        using var connection = new SqlConnection(connectionString);
        try
        {
            connection.Open();
            using (var command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var res = bool.Parse(reader[0].ToString());
                        result.Add(res);
                    }
                }
            }
            connection.Close();
        }
        catch (Exception ex)
        {
            if (connection.State == ConnectionState.Open) connection.Close();
        }

        if (result.Count == 0) throw new ApplicationException("Command not found");
        return $"[{result[0]}]";
    }

    [HttpGet]
    public string PostStatus(string input)
    {
        var procedureName = "PostStatus";
        using var connection = new SqlConnection(connectionString);
        try
        {
            connection.Open();
            using (var command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@status", input));
                command.ExecuteReader();
            }

            connection.Close();
        }
        catch (Exception ex)
        {
            if (connection.State == ConnectionState.Open) connection.Close();
        }
        return "[Success]";
    }

    [HttpGet]
    public string PostCommand(bool input, bool alreadyOn = false)
    {
        var procedureName = "PostCommand";
        using var connection = new SqlConnection(connectionString);
        try
        {
            connection.Open();
            using (var command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@command", input));
                command.Parameters.Add(new SqlParameter("@alreadyOn", alreadyOn));
                command.ExecuteReader();
            }
            connection.Close();
        }
        catch (Exception ex)
        {
            if (connection.State == ConnectionState.Open) connection.Close();
        }

        return "[Success]";
    }
}