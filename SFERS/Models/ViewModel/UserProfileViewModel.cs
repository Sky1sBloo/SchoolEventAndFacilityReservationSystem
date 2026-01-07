namespace SFERS.Models
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }

        // Fix: Initialize all string properties
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = "Student"; // Default role

        public bool IsActive { get; set; }
    }
}