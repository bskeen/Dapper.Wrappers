using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Tests.DbModels
{
    public class Author
    {
        public Guid AuthorID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string TestScope { get; set; }
    }
}
