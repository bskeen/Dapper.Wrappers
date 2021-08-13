// © 2021 by Benjamin Skeen
// Licensed to be used under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;

namespace Dapper.Wrappers.Generators
{
    public class MetadataGenerator: IMetadataGenerator
    {
        public QueryOperationMetadata GetDefaultOperation<T>(string name, string baseQueryString, string paramName) => GetOperation(name,
            baseQueryString, new[]
            {
                GetParameter<T>(paramName)
            });

        public QueryOperationMetadata GetOperation(string name, string baseQueryString,
            IEnumerable<QueryParameterMetadata> parameters) => new QueryOperationMetadata
        {
            Name = name,
            BaseQueryString = baseQueryString,
            Parameters = parameters
        };

        public MergeOperationMetadata GetDefaultMergeOperation<T>(string columnName, string baseQueryString) =>
            GetMergeOperation(columnName, baseQueryString, columnName, new[]
            {
                GetParameter<T>(columnName)
            });

        public MergeOperationMetadata GetMergeOperation(string name, string baseQueryString, string referencedColumn,
            IEnumerable<QueryParameterMetadata> parameters) => new MergeOperationMetadata
        {
            Name = name,
            BaseQueryString = baseQueryString,
            Parameters = parameters,
            ReferencedColumn = referencedColumn
        };

        // This list was generated from the list shown here (retrieved 8/6/2021):
        // https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings?redirectedfrom=MSDN
        private readonly Dictionary<Type, DbType> _allowedTypes = new Dictionary<Type, DbType>
        {
            {typeof(long), DbType.Int64},
            {typeof(byte[]), DbType.Binary},
            {typeof(bool), DbType.Boolean},
            {typeof(char[]), DbType.AnsiStringFixedLength},
            {typeof(DateTime), DbType.DateTime2},
            {typeof(DateTimeOffset), DbType.DateTimeOffset},
            {typeof(decimal), DbType.Decimal},
            {typeof(double), DbType.Double},
            {typeof(int), DbType.Int32},
            {typeof(string), DbType.String},
            {typeof(float), DbType.Single},
            {typeof(short), DbType.Int16},
            {typeof(TimeSpan), DbType.Time},
            {typeof(Guid), DbType.Guid}
        };

        public QueryParameterMetadata GetParameter<T>(string name, object defaultValue = null, bool hasDefault = false)
        {
            if (_allowedTypes.TryGetValue(typeof(T), out DbType paramType))
            {
                return GetParameter(name, paramType, defaultValue, hasDefault);
            }

            return GetParameter(name, DbType.Object, defaultValue, hasDefault);
        }

        public QueryParameterMetadata GetParameter(string name, DbType? paramType, object defaultValue = null,
            bool hasDefault = false)
        {
            var reallyHasDefault = hasDefault || !(defaultValue is null);

            if (reallyHasDefault)
            {
                return new QueryParameterMetadata
                {
                    Name = name,
                    DbType = paramType,
                    HasDefault = true,
                    DefaultValue = defaultValue
                };
            }

            return new QueryParameterMetadata
            {
                Name = name,
                DbType = paramType,
                HasDefault = false,
                DefaultValue = null
            };
        }
    }
}
