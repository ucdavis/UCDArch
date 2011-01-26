using System;

namespace UCDArch.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class HandleTransactionsManuallyAttribute : TransactionalActionBaseAttribute
    {
    }
}