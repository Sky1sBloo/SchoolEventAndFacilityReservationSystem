using System;

namespace SFERS.Models.Entities;

public class Account
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public int RoleId { get; set; }
    public required Role Role { get; set; }
}
