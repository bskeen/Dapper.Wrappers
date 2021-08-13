// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Dapper.Wrappers.Tests.DbModels
{
    public class BookGenre
    {
        public Guid BookID { get; set; }
        public Guid GenreID { get; set; }
    }
}
