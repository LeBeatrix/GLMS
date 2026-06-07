using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.Models
{
    public class Contract
    {
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        public Client? Client { get; set; }

        // Added for API responses
        public string? ClientName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; }

        [Required]
        [StringLength(100)]
        public string ServiceLevel { get; set; } = string.Empty;

        public string? AgreementFilePath { get; set; }

        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

        public string DisplayName
        {
            get
            {
                return $"{(ClientName ?? Client?.Name ?? "No Client")} - {ServiceLevel}";
            }
        }
    }
}