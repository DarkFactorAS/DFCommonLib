using System;
using System.Collections.Generic;
using DFCommonLib.DataAccess;
using DFCommonLib.Logger;

namespace DFCommonLib.HttpApi.OAuth2
{
    public interface IServerOAuth2Repository
    {
        IList<OAuth2ClientModel> GetOAuth2Clients();
    }

    public class ServerOAuth2Repository : IServerOAuth2Repository
    {
        private IDbConnectionFactory _connection;

        private readonly IDFLogger<ServerOAuth2Repository> _logger;

        public ServerOAuth2Repository(
            IDbConnectionFactory connection,
            IDFLogger<ServerOAuth2Repository> logger
            )
        {
            _connection = connection;
            _logger = logger;
        }

        public IList<OAuth2ClientModel> GetOAuth2Clients()
        {
            var clients = new List<OAuth2ClientModel>();
            string formattedSql = "SELECT id, client_id, client_secret, scope, TokenExpiresInSeconds, TokenIssuer FROM oauth2_clients";
            using (var cmd = _connection.CreateCommand(formattedSql))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var client = new OAuth2ClientModel
                        {
                            ClientId = reader["client_id"].ToString(),
                            ClientSecret = reader["client_secret"].ToString(),
                            Scope = reader["scope"].ToString(),
                            TokenExpiresInSeconds = Convert.ToUInt32(reader["TokenExpiresInSeconds"]),
                            TokenIssuer = reader["TokenIssuer"] == DBNull.Value ? string.Empty : reader["TokenIssuer"].ToString()
                        };
                        clients.Add(client);
                    }
                }
            }
            return clients;
        }

        public static string GetCreateTableString()
        {
            return @"CREATE TABLE `oauth2_clients` ("
            + " `id` int(11) NOT NULL AUTO_INCREMENT, "
            + " `client_id` varchar(100) NOT NULL DEFAULT '', "
            + " `client_secret` varchar(100) NOT NULL DEFAULT '', "
            + " `scope` varchar(100) NOT NULL DEFAULT '', "
            + " `TokenExpiresInSeconds` int(11) NOT NULL, "
            + " `TokenIssuer` varchar(50) NOT NULL DEFAULT '', "
            + " PRIMARY KEY (`id`)"
            + ")";
        }

    }
}