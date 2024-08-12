//HintName: Dapper.Wrappers.Analyzers.Postgres.Test.TestClassTracked.cs
using Dapper.Wrappers.Generation;

namespace Dapper.Wrappers.Analyzers.Postgres.Test
{
    public class TestClassTracked : TestClass
    {
        private bool _isTestBoolSet;
        public override bool TestBool
        {
            get
            {
                return base.TestBool;
            }

            set
            {
                _isTestBoolSet = true;
                base.TestBool = value;
            }
        }

        public bool IsTestBoolSet
        {
            get
            {
                return _isTestBoolSet;
            }
        }

        private bool _isTestIntSet;
        public override int TestInt
        {
            get
            {
                return base.TestInt;
            }

            set
            {
                _isTestIntSet = true;
                base.TestInt = value;
            }
        }

        public bool IsTestIntSet
        {
            get
            {
                return _isTestIntSet;
            }
        }
    }
}