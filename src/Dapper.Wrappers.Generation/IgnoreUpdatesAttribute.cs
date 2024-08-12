using System;

namespace Dapper.Wrappers.Generation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IgnoreUpdatesAttribute : Attribute
    {
    }
}
