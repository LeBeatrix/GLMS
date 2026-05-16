using GLMS.Web.Models;

namespace GLMS.Web.Observers
{
    public class ServiceRequestLogger : IServiceRequestObserver
    {
        public void Update(ServiceRequest serviceRequest)
        {
            Console.WriteLine(
                $"Observer Notification: Service Request {serviceRequest.Id} status changed to {serviceRequest.Status}"
            );
        }
    }
}