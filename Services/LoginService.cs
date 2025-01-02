using LdapAuthApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace LdapAuthApi.Services
{
    public class LoginService
    {
        private readonly JwtService jwtService;
        private readonly string connectionString;

        public LoginService(IOptions<JwtSettings> jwtSettings, IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            jwtService = new JwtService(jwtSettings);
        }

        public void SaveLoginResponse(string username, string email, string displayName, string department, string title)
        {
            // Usar la cadena de conexión obtenida desde la configuración
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("SaveLoginResponse", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@displayName", displayName);
                    command.Parameters.AddWithValue("@department", department);
                    command.Parameters.AddWithValue("@title", title);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
