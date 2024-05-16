using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MiniApp1.API.Controllers
{
    // Sadece 'SystemAdmin' rolüne ait kullanıcılar erişebilir
    [Authorize(AuthenticationSchemes = ("Bearer"), Roles = "manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStock()
        {
            string rolemessage = " Only users with the manager role can send requests to this API.!";

            var userName = HttpContext.User.Identity?.Name; // bu name bize token'ın Claims'lerinden gelecek

            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier); // gelen Token'ın Claims'lerinden NameIdentifier ile gelen ID 'yi aldık

            var userEmailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            // Bundan sonra veri tabanından kulanıcıyı çekip işlem yapabiliriz.

            var jwtGuidIdClaim = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti);

            return Ok($"API Project Name: 'MiniApp1.API - Stocks'\n System Admin Message: {rolemessage}\n " +
                $"userId: {userIdClaim?.Value} - userName: {userName}\n " +
                $"email: {userEmailClaim?.Value} - JwtGuidId: {jwtGuidIdClaim?.Value}");
        }
    }
}
