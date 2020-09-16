using System;

namespace housingResidentInformationAPI.V1.Domain
{
    public class ResidentNotFoundException : Exception
    {
        public ResidentNotFoundException() { }
        public ResidentNotFoundException(string message) : base(message)
        { }
    }
}
