using System;

namespace Dapper.Wrappers.Generation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TrackUpdatesAttribute : Attribute
    {
    }
}
