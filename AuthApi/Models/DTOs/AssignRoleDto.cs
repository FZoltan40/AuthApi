namespace AuthApi.Models.DTOs
{
    public class NewAssignRoleDto
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
