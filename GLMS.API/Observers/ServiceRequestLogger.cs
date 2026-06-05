using GLMS.API.Models;

namespace GLMS.API.Observers
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