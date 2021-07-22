// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Dapper.Wrappers.Tests.DbModels
{
    public class Author
    {
        public Guid AuthorID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
