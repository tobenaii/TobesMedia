using System;
using System.Collections.Generic;
using System.Text;

namespace TobesMediaCore.Data.Media
{
    public class Movie : MediaBase
    {
        public Movie(string name, string description, string posterURL) : base(name, description, posterURL)
        {
        }
    }
}
