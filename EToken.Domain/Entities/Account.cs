using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace EToken.Domain.Entities;

public class Account 
{
    // Cif aliases the underlying Identity Id
    public Guid Cif{ get ; set;}
    public Guid Id {get; set;}
    public string Number{get; set;}
    public string Type{get; set;}
    public Decimal Balance{get; set;}
    public string Status{get; set;}


    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;


}