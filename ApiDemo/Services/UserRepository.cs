using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApiDemo.Services
{
    public class UserRepository : IUserRepository
    {
        private Container _container;

        public UserRepository(CosmosClient dbClient,string dbName,string containerName)
        {
            _container = dbClient.GetContainer(dbName, containerName);
        }

        public async Task<ApiDemo.Domain.User> GetUserByEmailAddressAsync(string emailAddress)
        {
            try
            {
                ItemResponse<ApiDemo.Domain.User> response = await this._container.ReadItemAsync<ApiDemo.Domain.User>(emailAddress, new PartitionKey(emailAddress));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task CreateUser(ApiDemo.Domain.User user)
        {
            ItemResponse<ApiDemo.Domain.User> res = await this._container.CreateItemAsync<ApiDemo.Domain.User>(user, new PartitionKey(user.EmailAddress));
        }

        public async Task UpdateUser(string id, ApiDemo.Domain.User user)
        {
            await _container.UpsertItemAsync<ApiDemo.Domain.User>(user, new PartitionKey(user.EmailAddress));
        }
    }
}
