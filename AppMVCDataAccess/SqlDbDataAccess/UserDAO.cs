﻿using DataAccessLayer.Model;
using System.Security.Principal;
using System.Data.SqlClient;
using System.Data;
using DataAccessLayer.Interfaces;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace DataAccessLayer.SqlDbDataAccess
{
    public class UserDAO : IUserDataAccess
    {
        private string connectionstring;
        private IOrderDataAccess orderDataAccess;

        public UserDAO(string connectionstring)
        {
            this.connectionstring = connectionstring;
            orderDataAccess = DataAccessFactory.CreateRepository<IOrderDataAccess>(connectionstring);
        }

        public async Task<string> CreateUserAsync(User user)
        {
            using SqlConnection connection = new SqlConnection(connectionstring);

            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead);

            SqlCommand command = connection.CreateCommand();
            command.Transaction = transaction;

            try
            {
                command.CommandText = "INSERT INTO dbo.[User] (email, name, surename, phone, address, username, password, isAdmin) VALUES (@email, @name, @surename, @phone, @address, @username, @password, @isAdmin);)";
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@surename", user.Surename);
                command.Parameters.AddWithValue("@phone", user.PhoneNumber);
                command.Parameters.AddWithValue("@address", user.Address);
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@isAdmin", user.IsAdmin);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
            return user.Email;
        }

        public async Task<bool> DeleteUserAsync(string email)
        {
            using SqlConnection connection = new SqlConnection(connectionstring);
            connection.Open();

            SqlTransaction transaction = connection.BeginTransaction();
            SqlCommand command = connection.CreateCommand();
            command.Transaction = transaction;

            try
            {
                command.CommandText = "DELETE FROM [User] WHERE email = @email";
                command.Parameters.AddWithValue("@email", email);
                command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("An error occured while deleting a User: " + ex);
                return false;
            }
            finally
            {
                connection.Close();
            }
            return true;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            List<User> users = new List<User>();
            using SqlConnection connection = new SqlConnection(connectionstring);
            try
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand("Select * from [User]");
                selectCommand.Connection = connection;
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    // dorobit
                    users.Add(new User(reader.GetString("email"), reader.GetString("name"), reader.GetString("surname"), reader.GetString("phone"), reader.GetString("address"), reader.GetString("username"), reader.GetString("password"), reader.GetBoolean("isAdmin")));
                }
            }
            catch (SqlException sqlex)
            {
                Console.WriteLine("An sql error occured while trying to retrieve all the users from the database: " + sqlex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unspecified error occured while trying to retrieve all the users from the database: " + ex);
            }
            finally
            {
                connection.Close();
            }
            return users;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using SqlConnection connection = new SqlConnection(connectionstring);
            try
            {
                connection.Open();
                string query = "SELECT * FROM [User] WHERE email = @email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", email);
                SqlDataReader reader = command.ExecuteReader();

                reader.Read();
                IEnumerable<Order> customersOrders = await orderDataAccess.GetOrdersByUserAsync(email);
                return new User(reader.GetString("email"), reader.GetString("name"), reader.GetString("surname"), reader.GetString("phone"), reader.GetString("address"), reader.GetString("username"), reader.GetString("password"), reader.GetBoolean("isAdmin"), customersOrders);

            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            using SqlConnection connection = new SqlConnection(connectionstring);
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * from [User] where email = @email and password = @password", connection);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@name", password);
                SqlDataReader reader = command.ExecuteReader();
                User user = new User(reader.GetString("email"), reader.GetString("name"), reader.GetString("surename"), reader.GetString("phone"), reader.GetString("address"), reader.GetString("username"), reader.GetString("password"), reader.GetBoolean("isAdmin"));
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error logging in for author with email {email}: '{ex.Message}'.", ex);
            }
            
        }

        public async Task<bool> UpdateUserAsync(User user)
        {

            using SqlConnection connection = new SqlConnection(connectionstring);
            try
            {
                connection.Open();                            //email, name, surename, phone, address, username, password, isAdmin
                SqlCommand command = new SqlCommand("UPDATE [User] SET email = @email, name = @name, surename = @surename, phone = @phone, adddress = @address, username = @username, password = @password, isAdmin = @isAdmin WHERE email = @email", connection);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@surename", user.Surename);
                command.Parameters.AddWithValue("@phone", user.PhoneNumber);
                command.Parameters.AddWithValue("@address", user.Address);
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@isAdmin", user.IsAdmin);
                int affected = command.ExecuteNonQuery();
                if (affected == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while updating User " + ex);
            }
            finally
            {
                connection.Close();
            }
            return false;
        }
    }
}