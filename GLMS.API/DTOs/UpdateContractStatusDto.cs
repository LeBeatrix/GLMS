using GLMS.API.Models;
using System.ComponentModel.DataAnnotations;

namespace GLMS.API.DTOs
{
    public class UpdateContractStatusDto
    {
        [Required]
        public ContractStatus Status { get; set; }
    }
}