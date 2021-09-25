using System.Collections.Generic;
using System.Linq;

namespace Dapper.Wrappers.Builders
{
    public class ParsedValuesListsQueryOperations : ParsedQueryOperations
    {
        public override IEnumerable<QueryOperation> QueryOperations
        {
            get => MultiQueryOperations.FirstOrDefault();

            set => MultiQueryOperations = new[] { value };
        }

        public IEnumerable<IEnumerable<QueryOperation>> MultiQueryOperations { get; set; }
    }
}
