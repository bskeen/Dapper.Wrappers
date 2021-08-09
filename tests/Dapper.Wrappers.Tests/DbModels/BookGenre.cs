using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Tests.DbModels
{
    public class BookGenre
    {
        public Guid BookID { get; set; }
        public Guid GenreID { get; set; }
    }
}
