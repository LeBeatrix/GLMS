using GLMS.API.Models;

namespace GLMS.API.Factories
{
    public interface IServiceRequestFactory
    {
        ServiceRequest Create(
            int contractId,
            string description,
            decimal costUsd,
            string status
        );
    }
}