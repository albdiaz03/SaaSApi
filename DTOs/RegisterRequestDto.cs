namespace SaaS.Api.DTOs
{
    public class RegisterRequestDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid CompanyId { get; set; }  
    }
}