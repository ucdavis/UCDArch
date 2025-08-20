using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NHibernate.Collection;
using NHibernate.Proxy;

namespace UCDArch.Web.ModelBinder
{
    public class SuppressNHibernateProxyValidationAttribute : Attribute, IPropertyValidationFilter
    {
        public bool ShouldValidateEntry(ValidationEntry entry, ValidationEntry parentEntry)
        {
            if (IsNHibernateWrapper(parentEntry.Model.GetType())
                || (entry.Model != null && IsNHibernateWrapper(entry.Model.GetType())))
            {
                return false;
            }
            return true;
        }

        private static bool IsNHibernateWrapper(Type type)
        {
            return typeof(INHibernateProxy).IsAssignableFrom(type)
                || typeof(AbstractPersistentCollection).IsAssignableFrom(type);
        }
    }
}
