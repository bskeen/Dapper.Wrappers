using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Tests.DbModels
{
    public class Book
    {
        public Guid BookID { get; set; }

        public string Name { get; set; }

        public Guid AuthorID { get; set; }
        public Author Author { get; set; }

        public int? PageCount { get; set; }

        public IEnumerable<Genre> Genres { get; set; }
    }
}
