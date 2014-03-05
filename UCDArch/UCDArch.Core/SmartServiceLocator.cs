using System;
using Microsoft.Practices.ServiceLocation;

namespace UCDArch.Core
{
    public static class SmartServiceLocator<DependencyT>
    {
        public static DependencyT GetService()
        {
            DependencyT service;

            try
            {
                service = (DependencyT) ServiceLocator.Current.GetService(typeof (DependencyT));
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("ServiceLocator has not been initialized; " +
                                                 "I was trying to retrieve " + typeof (DependencyT));
            }
            catch (InvalidOperationException)
            {
                throw new NullReferenceException("ServiceLocator has not been initialized; " +
                                                 "I was trying to retrieve " + typeof(DependencyT));                
            }
            catch (ActivationException)
            {
                throw new ActivationException("The needed dependency of type " + typeof(DependencyT).Name +
                    " could not be located with the ServiceLocator. You'll need to register it with " +
                    "the Common Service Locator (CSL) via your IoC's CSL adapter.");
            }

            return service;
        }
    }
}