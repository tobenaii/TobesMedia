using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobesMediaServer.Database
{
    public interface IMovieDatabase
    {
        public void AddMovie(string name, string imdbID, string fileDir);
        public Task<bool> MovieExistsAsync(string imdbID);
        public Task<string> GetMovieDirectoryAsync(string imdbID);
    }
}
