﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Wrappers.Formatters
{
    public class SqlServerQueryFormatter : BaseSqlQueryFormatter
    {
        public override string FormatVariable(string variableName)
        {
            return $"@{variableName}";
        }

        public override string FormatIdentifier(string identifierName)
        {
            return $"[{identifierName}]";
        }

        public override string FormatPagination(string skipVariable, string takeVariable)
        {
            return $"OFFSET {skipVariable} ROWS FETCH NEXT {takeVariable} ROWS ONLY";
        }
    }
}