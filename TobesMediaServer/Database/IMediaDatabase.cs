using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobesMediaServer.Database
{
    public interface IMediaDatabase
    {
        public void AddMedia(string id, string value = "");
        public Task<string> GetValueAsync(string id);
        public Task<bool> MediaExistsAsync(string id);
        public void RemoveMedia(string id);

        public Task<List<string>> GetAllIdsAsync();
    }
}
