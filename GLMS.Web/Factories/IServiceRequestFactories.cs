using GLMS.Web.Models;

namespace GLMS.Web.Factories
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