using GLMS.Web.Models;

namespace GLMS.Web.Factories
{
    public class ServiceRequestFactory : IServiceRequestFactory
    {
        public ServiceRequest Create(
            int contractId,
            string description,
            decimal costUsd,
            string status)
        {
            return new ServiceRequest
            {
                ContractId = contractId,
                Description = description,
                CostUSD = costUsd,
                Status = status,
                CostZAR = 0
            };
        }
    }
}