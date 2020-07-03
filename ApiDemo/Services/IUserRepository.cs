using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace ApiDemo.Services
{
    public interface IUserRepository
    {
        Task<ApiDemo.Domain.User> GetUserByEmailAddressAsync(string emailAddress);
        Task CreateUser(ApiDemo.Domain.User user);
        Task UpdateUser(string id, ApiDemo.Domain.User user);
    }
}