using System;

namespace housingResidentInformationAPI.V1.Domain
{
    public class InvalidQueryParameterException : Exception
    {
        public InvalidQueryParameterException(string message)
            : base(message)
        {
        }
    }
}
