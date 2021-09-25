// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Data;

namespace Dapper.Wrappers
{
    public interface IMetadataGenerator
    {
        QueryOperationMetadata GetDefaultOperation<T>(string name, string baseQueryString, string paramName);

        QueryOperationMetadata GetOperation(string name, string baseQueryString,
            IEnumerable<QueryParameterMetadata> parameters);

        QueryOperationMetadata GetDefaultOrderOperation(string name, string baseQueryString);

        QueryOperationMetadata GetOrderOperation(string name, string baseQueryString,
            IEnumerable<QueryParameterMetadata> parameters);

        MergeOperationMetadata GetDefaultMergeOperation<T>(string columnName, string baseQueryString,
            bool isRequired = false);

        MergeOperationMetadata GetMergeOperation(string name, string baseQueryString, string referencedColumn,
            IEnumerable<QueryParameterMetadata> parameters, bool isRequired = false);

        QueryParameterMetadata GetParameter<T>(string name, object defaultValue = null, bool hasDefault = false);

        QueryParameterMetadata GetParameter(string name, DbType? paramType, object defaultValue = null,
            bool hasDefault = false);

        QueryOperation GetQueryOperation(string name, params (string name, object value)[] parameters);
    }
}
