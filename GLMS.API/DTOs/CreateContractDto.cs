using GLMS.API.Models;
using System.ComponentModel.DataAnnotations;

namespace GLMS.API.DTOs
{
    public class CreateContractDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; }

        [Required]
        [StringLength(100)]
        public string ServiceLevel { get; set; } = string.Empty;

        public string? AgreementFilePath { get; set; }
    }
}