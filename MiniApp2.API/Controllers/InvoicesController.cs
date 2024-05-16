using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MiniApp2.API.Controllers
{
    [Authorize(AuthenticationSchemes = ("Bearer"),Roles = "manager" , Policy = "IstanbulPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetInvoices()
        {
            var userName = HttpContext.User.Identity?.Name; // bu name bize token'ın Claims'lerinden gelecek

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier); // gelen Token'ın Claims'lerinden NameIdentifier ile gelen ID 'yi aldık

            var userEmailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            // Bundan sonra veri tabanından kulanıcıyı çekip işlem yapabiliriz.

            var jwtGuidIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti);

            return Ok($"API Project Name: 'MiniApp2.API - Invoices'\n userId: {userIdClaim?.Value} - userName: {userName}\n email: {userEmailClaim?.Value} - JwtGuidId: {jwtGuidIdClaim?.Value}");
        }
    }
}
