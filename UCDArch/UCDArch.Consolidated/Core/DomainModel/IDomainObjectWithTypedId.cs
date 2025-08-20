using System.Collections.Generic;
using System.Reflection;

namespace UCDArch.Core.DomainModel
{
    /// <summary>
    /// This serves as a base interface for <see cref="DomainObjectWithTypedId{IdT}"/> and 
    /// <see cref="DomainObject"/>. Also provides a simple means to develop your own base entity.
    /// </summary>
    public interface IDomainObjectWithTypedId<IdT>
    {
        IdT Id { get; }
        bool IsTransient();
        IEnumerable<PropertyInfo> GetSignatureProperties();
    }
}