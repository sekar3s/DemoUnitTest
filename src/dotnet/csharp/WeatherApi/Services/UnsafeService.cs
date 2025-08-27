using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;
using System.Text;

namespace WeatherApi.Services
{
    public class SafeService
    {
        public string ReadFile(string userInput)
        {
            // Validate and sanitize the file path
            string basePath = "/safe/directory"; // Define a safe base directory
            string fullPath = Path.GetFullPath(Path.Combine(basePath, userInput));

            if (!fullPath.StartsWith(basePath))
            {
                throw new UnauthorizedAccessException("Access to the specified file is not allowed.");
            }

            // Read the file safely
            if (File.Exists(fullPath))
            {
                return File.ReadAllText(fullPath, Encoding.UTF8);
            }

            throw new FileNotFoundException("The specified file does not exist.");
        }

        public int GetProduct(string productName)
        {
            // Use parameterized queries to prevent SQL injection
            using (SqlConnection connection = new SqlConnection("fakeconnectionstring"))
            {
                connection.Open();

                using (SqlCommand sqlCommand = new SqlCommand("SELECT ProductId FROM Products WHERE ProductName = @ProductName", connection))
                {
                    sqlCommand.Parameters.Add(new SqlParameter("@ProductName", SqlDbType.NVarChar) { Value = productName });

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(0);
                        }
                        else
                        {
                            throw new Exception("Product not found.");
                        }
                    }
                }
            }
        }
    }
}