using HousingResidentInformationAPI.V1.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace HousingResidentInformationAPI.Tests.V1.UseCase
{
    public class ValidatePostcodeTests
    {
        [TestCase("E8 1JJ")]
        [TestCase("E8T 1JJ")]
        [TestCase("EY58 1JJ")]
        [TestCase("EH1A 1JJ")]
        [TestCase("EH8 1JJ")]
        [TestCase("E8 1JJ")]
        [TestCase("E13 1JJ")]
        [TestCase("e87 1Jj")]
        [TestCase(null)]
        public void ValidPostcodesReturnTrue(string postcode)
        {
            var validator = new ValidatePostcode();
            validator.Execute(postcode).Should().BeTrue();
        }

        [TestCase("1")]
        [TestCase("111111")]
        [TestCase("111 111")]
        [TestCase("EEEEEE")]
        [TestCase("EEE EEE")]
        [TestCase("E")]
        [TestCase("BA56 6Y")]
        [TestCase("B 7JI")]
        [TestCase("BA56 YTH")]
        [TestCase("BA 6YU")]
        [TestCase("BAY  6IY")]
        [TestCase("BA56 YH")]
        [TestCase("6A567 6YH")]
        [TestCase("B656 6YU")]
        [TestCase("BHHHH656 6YU")]
        [TestCase("BH6 6YUuuu")]
        [TestCase("Q33q 8TH")]
        public void InvalidPostcodesReturnsFalse(string postcode)
        {
            var validator = new ValidatePostcode();
            validator.Execute(postcode).Should().BeFalse();
        }
    }
}
