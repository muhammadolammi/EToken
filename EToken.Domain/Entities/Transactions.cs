using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace EToken.Domain.Entities;

public class Transaction 
{

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Reference { get; set; } = string.Empty;
    public Guid SourceAccountId { get; set; }
    public Guid DestinationAccountId { get; set; }
    public decimal Amount { get; set; } 
    public string Narration { get; set; } = string.Empty;
    public string Status { get; set; } = "successful"; // "successful", "failed", "pending"
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
    


