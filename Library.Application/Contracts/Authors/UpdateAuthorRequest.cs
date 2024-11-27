using System;
using System.ComponentModel.DataAnnotations;

namespace Library.Application.Contracts.Authors
{
    public record UpdateAuthorRequest(
        [Required] string FirstName,
        [Required] string LastName,
        [Required] DateTime DateOfBirth,
        [Required] string Country);
}