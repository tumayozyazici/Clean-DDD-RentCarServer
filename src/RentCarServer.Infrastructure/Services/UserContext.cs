using Microsoft.AspNetCore.Http;
using RentCarServer.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarServer.Infrastructure.Services
{
    internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        public Guid GetUserId()
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext is null) throw new ArgumentNullException("HttpContext bulunamadı.");

            var claims = httpContext.User.Claims;
            string? userId = claims.FirstOrDefault(p => p.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if(userId is null) throw new ArgumentNullException("Kullanıcı bilgisi bulunamadı.");

            try
            {
                Guid id = Guid.Parse(userId);
                return id;
            }
            catch (Exception)
            {

                throw new ArgumentException("Kullanıcı id uygun guid formatında değil.");
            }
        }
    }
}
