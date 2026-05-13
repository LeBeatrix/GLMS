using GLMS.Web.Models;

namespace GLMS.Web.Services
{
    public class ServiceRequestValidator
    {
        public bool CanCreateServiceRequest(Contract contract)
        {
            return contract.Status == ContractStatus.Active;
        }

        public string GetValidationMessage(Contract contract)
        {
            if (contract.Status == ContractStatus.Expired)
            {
                return "Service request cannot be created because the contract is expired.";
            }

            if (contract.Status == ContractStatus.OnHold)
            {
                return "Service request cannot be created because the contract is on hold.";
            }

            if (contract.Status == ContractStatus.Draft)
            {
                return "Service request cannot be created because the contract is still in draft.";
            }

            return string.Empty;
        }
    }
}