using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Tests.DbModels
{
    public class Genre
    {
        public Guid GenreID { get; set; }
        public string Name { get; set; }
        public Guid TestScope { get; set; }
    }
}
