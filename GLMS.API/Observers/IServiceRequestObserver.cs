using GLMS.API.Models;

namespace GLMS.API.Observers
{
    public interface IServiceRequestObserver
    {
        void Update(ServiceRequest serviceRequest);
    }
}