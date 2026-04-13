using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace SharedKernel.Utilities.Helpers;

public class DbHelper(
    IHttpContextAccessor httpContextAccessor,
    ILogger<DbHelper> logger,
    Microsoft.Extensions.Configuration.IConfiguration configuration
)
{
    private readonly ConcurrentDictionary<string, string> _connectionCache = new();

    public SqlConnection GetSaasDB()
    {
        var connectionString = configuration.GetConnectionString("SaasDB") 
            ?? throw new InvalidOperationException("SaasDB connection string not configured");
        
        return new SqlConnection(connectionString);
    }      
}

