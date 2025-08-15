using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Play_ease_Backend.Controllers
{
    public class GetDataFromDatasourceController : ControllerBase
    {
        private readonly IConfiguration _config;
        public GetDataFromDatasourceController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("GetDataByDataSource")]
        public async Task<IActionResult> GetDataByDataSource(int dataSourceId, string whereClause = "")
        {
            var results = new List<Dictionary<string, object>>();

            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            using (var cmd = new SqlCommand("pe_GetDataByDataSource", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DataSourceID", dataSourceId);
                cmd.Parameters.AddWithValue("@WhereClause", whereClause ?? "");

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }
                        results.Add(row);
                    }
                }
            }

            return Ok(results);
        }

    }
}
