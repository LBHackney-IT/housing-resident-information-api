using System;

namespace HousingResidentInformationAPI.V1.Domain
{
    public class ResidentNotFoundException : Exception
    {
        public ResidentNotFoundException() { }
        public ResidentNotFoundException(string message) : base(message)
        { }
    }
}
