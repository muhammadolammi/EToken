using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace EToken.Domain.Entities;

public class User : IdentityUser<Guid>
{
    // Cif aliases the underlying Identity Id
   [NotMapped]
    public Guid Cif
    {
        get => Id;
        set => Id = value;
    }

    public string FirstName{get; set;}
    public string LastName{get; set;}

}