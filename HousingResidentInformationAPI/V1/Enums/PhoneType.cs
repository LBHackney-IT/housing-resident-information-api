using System.ComponentModel;

namespace HousingResidentInformationAPI.V1.Enums
{
    public enum PhoneType
    {
        [Description("Home")] H,
        [Description("Mobile")] M,
        [Description("Fax")] F,
        [Description("Work")] W,
        X
    }
}
