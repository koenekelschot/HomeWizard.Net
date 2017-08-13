using System;

namespace HomeWizard.Net
{
    public class HomeWizardClientException : Exception
    {
        public HomeWizardClientException() 
            : base() { }

        public HomeWizardClientException(string message) 
            : base(message) { }

        public HomeWizardClientException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
