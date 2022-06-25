using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Login
{
    public class UserController : Controller
    {
        const string connectionString = "Server=127.0.0.1,1433;database=MyApi;user id=sa;Password=MyPass@word;TrustServerCertificate=true";

        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody] UserModel body)
        {
            var insertUser = "INSERT INTO [Users] (Nome, Email, Telefone, Celular) OUTPUT Inserted.Id VALUES (@nome,@email,@telefone,@celular)";
            var insertAddress = "INSERT INTO [Address] (IdUsuario,CEP,Endereco,Numero,Bairro,Cidade,Estado,Created_at) VALUES (@idUsuario,@cep,@endereco,@numero,@bairro,@estado,@cidade,@criado)";
            var verifyEmail = "SELECT COUNT(*) FROM [Users] Where Email = @email";

            using (var connection = new SqlConnection(connectionString))
            {
                var countEmail = connection.ExecuteScalar<int>(verifyEmail, new { body.Email });

                if (countEmail > 1)
                {
                    return BadRequest("Erro ao criar usuário.");
                }
                else
                {
                    try
                    {
                        // Insert Users
                        var rows = connection.ExecuteScalar<int>(insertUser, new { body.Nome, body.Email, body.Telefone, body.Celular });

                        // Insert Address
                        connection.Execute(insertAddress, new { @idUsuario = rows, body.CEP, body.Endereco, body.Numero, body.Bairro, body.Cidade, body.Estado, @criado = DateTime.Now });

                        return Ok("Usuário criado com sucesso.");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Usuário não pode ser criado, erro {ex}");
                    }
                }
            }
        }
    }
}

