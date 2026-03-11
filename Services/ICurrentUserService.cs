namespace SaaS.Api.Services
{
    public interface ICurrentUserService
    {
        Guid? CompanyId { get; }
    }
}