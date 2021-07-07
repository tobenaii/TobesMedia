using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobesMediaServer.Database
{
    public interface IMediaDatabase
    {
        public void AddMedia(string id, int season, int episode, string value = "");
        public Task<string> GetValueAsync(string id, int seasopn, int episode);
        public Task<bool> MediaExistsAsync(string id, int season, int episode);
        public void RemoveMedia(string id, int season, int episode);

        public Task<List<string>> GetAllIdsAsync();
        public Task<string> GetFilePathAsync(string id, int season, int episode);
    }
}
