using GLMS.Web.Models;
using GLMS.Web.Services;

namespace GLMS.Tests
{
    public class ServiceRequestValidatorTests
    {
        [Fact]
        public void CanCreateServiceRequest_ActiveContract_ReturnsTrue()
        {
            var validator = new ServiceRequestValidator();

            var contract = new Contract
            {
                Status = ContractStatus.Active
            };

            var result = validator.CanCreateServiceRequest(contract);

            Assert.True(result);
        }

        [Fact]
        public void CanCreateServiceRequest_ExpiredContract_ReturnsFalse()
        {
            var validator = new ServiceRequestValidator();

            var contract = new Contract
            {
                Status = ContractStatus.Expired
            };

            var result = validator.CanCreateServiceRequest(contract);

            Assert.False(result);
        }

        [Fact]
        public void CanCreateServiceRequest_OnHoldContract_ReturnsFalse()
        {
            var validator = new ServiceRequestValidator();

            var contract = new Contract
            {
                Status = ContractStatus.OnHold
            };

            var result = validator.CanCreateServiceRequest(contract);

            Assert.False(result);
        }

        [Fact]
        public void CanCreateServiceRequest_DraftContract_ReturnsFalse()
        {
            var validator = new ServiceRequestValidator();

            var contract = new Contract
            {
                Status = ContractStatus.Draft
            };

            var result = validator.CanCreateServiceRequest(contract);

            Assert.False(result);
        }
    }
}