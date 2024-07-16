using System.ComponentModel.DataAnnotations;
namespace EntityService.Application.Models;
public class EntityDto {
    [Required] 
    public Guid Id { get; set; }
    [Required] 
    public DateTime OperationDate { get; set; }
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than or equal to zero.")]
    public decimal Amount { get; set; }
}