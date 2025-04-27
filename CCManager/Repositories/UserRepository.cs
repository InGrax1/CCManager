using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using CCManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace CCManager.Repositories
{
    // VALIDACION DE USUARIO
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public void Add(UserModel userModel)
        {
            using (var connection = GetConnection())
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                INSERT INTO [User] (Username, [Password], Name, LastName, Email) 
                VALUES (@username, @password, @name, @lastname, @email)";

                    // Encriptar la contraseña
                    using (var sha256 = SHA256.Create())
                    {
                        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(userModel.Password));
                        command.Parameters.Add("@password", SqlDbType.VarBinary).Value = hash;
                    }

                    // Agregar el resto de parámetros
                    command.Parameters.Add("@username", SqlDbType.NVarChar).Value = userModel.Username;
                    command.Parameters.Add("@name", SqlDbType.NVarChar).Value = userModel.Name;
                    command.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = userModel.LastName;
                    command.Parameters.Add("@email", SqlDbType.NVarChar).Value = userModel.Email;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool AuthenticateUser(NetworkCredential credential)
        {
            using (var connection = GetConnection())
            {
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "SELECT [Password] FROM [User] WHERE Username = @username";
                    command.Parameters.Add("@username", SqlDbType.NVarChar).Value = credential.UserName;

                    var hashedPasswordFromDb = command.ExecuteScalar() as byte[];
                    if (hashedPasswordFromDb == null)
                        return false;

                    // Convertimos la contraseña ingresada a hash SHA-256
                    using (var sha256 = SHA256.Create())
                    {
                        byte[] enteredPasswordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(credential.Password));
                        return hashedPasswordFromDb.SequenceEqual(enteredPasswordHash);
                    }
                }
            }
        }

        public void Edit(UserModel userModel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserModel> GetByAll()
        {
            throw new NotImplementedException();
        }

        public UserModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public UserModel GetByUsername(string username)
        {
            UserModel user = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "select *from [User] where username=@username";
                command.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserModel()
                        {
                            Id = reader[0].ToString(),
                            Username = reader[1].ToString(),
                            Password = string.Empty,
                            Name = reader[3].ToString(),
                            LastName = reader[4].ToString(),
                            Email = reader[5].ToString(),
                        };
                    }
                }
            }
            return user;
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }
    }
}
