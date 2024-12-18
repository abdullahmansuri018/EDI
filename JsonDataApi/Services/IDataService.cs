using JsonDataApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonDataApi.Services
{
    public interface IDataService
    {
        Task<CosmosContainer> FetchDataByContainerId(string containerId, string userId, string email);
        Task<List<CosmosContainer>> FetchDataByUserId(string userId);
        Task<bool> RemoveContainer(string containerId, string userId);
    }
}
