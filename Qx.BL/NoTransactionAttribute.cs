using System;

namespace Qx.BL
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class NoTransactionAttribute : Attribute
    {
    }
}