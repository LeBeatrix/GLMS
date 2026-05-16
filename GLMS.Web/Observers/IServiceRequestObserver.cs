using GLMS.Web.Models;

namespace GLMS.Web.Observers
{
    public interface IServiceRequestObserver
    {
        void Update(ServiceRequest serviceRequest);
    }
}