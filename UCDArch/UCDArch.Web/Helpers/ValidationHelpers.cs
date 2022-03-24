using Microsoft.AspNetCore.Mvc.ModelBinding;
using UCDArch.Core.DomainModel;
using UCDArch.Web.Validator;

namespace UCDArch.Web.Helpers
{
    public static class ValidationHelpers
    {
        public static void TransferValidationMessagesTo<IdT>(
            this DomainObjectWithTypedId<IdT> domainObjectWithTypedID, ModelStateDictionary modelStateDictionary)
        {
            MvcValidationAdapter.TransferValidationMessagesTo(modelStateDictionary,
                                                              domainObjectWithTypedID.ValidationResults());
        }

        public static void TransferValidationMessagesTo<IdT>(
            this DomainObjectWithTypedId<IdT> domainObjectWithTypedID, string keyBase,
            ModelStateDictionary modelStateDictionary)
        {
            MvcValidationAdapter.TransferValidationMessagesTo(keyBase, modelStateDictionary,
                                                              domainObjectWithTypedID.ValidationResults());
        }
    }
}