using System.Security.Claims;

namespace SaaS.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? CompanyId
        {
            get
            {
                var claim = _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirst("CompanyId")?
                    .Value;

                if (Guid.TryParse(claim, out var companyId))
                    return companyId;

                return null;
            }
        }
    }
}