// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Dapper.Wrappers.Tests.DbModels
{
    public class Book
    {
        public Guid BookID { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public Guid AuthorID { get; set; }
        public Author Author { get; set; }

        public int? PageCount { get; set; }

        public IEnumerable<Genre> Genres { get; set; }
    }
}
