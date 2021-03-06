﻿using System.Linq;
using Dapper;
using System.Text.RegularExpressions;
using System.Data;
using DiscussionForum.Domain.Interfaces;

namespace DiscussionForum.Services
{
    public class UsernameGenerator : IUsernameGenerator
    {
        private IDbConnection _connection { get; set; }

        public UsernameGenerator(IDbConnection connection)
        {
            _connection = connection;
        }

        public string GenerateUsername(string email)
        {
            var username = email.Split('@').First();
            username = Regex.Replace(username, "[^a-zA-Z0-9]", "-");

            var sql = $@"SELECT Username FROM Users WHERE Username LIKE @value";
            var usernames = _connection.Query<string>(sql, new { value = $"{username}%"}).ToList();
            int count = 1;
            var generatedUsername = username;
            while (usernames.Contains(generatedUsername))
            {
                generatedUsername = username + count;
                count++;
            }

            return generatedUsername;
        }
    }
}