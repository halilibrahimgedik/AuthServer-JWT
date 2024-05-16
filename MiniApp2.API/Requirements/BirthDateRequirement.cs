using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MiniApp2.API.Requirements
{
    public class BirthDateRequirement : IAuthorizationRequirement
    {
        // ** bir policy tabanlı yetkilendirme yapabilmek için sınıfımız "IAuthorizationRequirement" sınıfından miras almalı.

        // ben bugün 18 yaşından küçük'ler doğrulanmasın derim yarın ise 21 yaşından küçükler bunuda dinamik tasarlamak amacıyla bu propertty'i ekliyoruz.
        public int RequiredAge { get; set; }

        public BirthDateRequirement(int requiredAge)
        {
            RequiredAge = requiredAge;
        }
    }

    // Business kodumu yazıcağımız sınıfımı tanımlayalım
    // bu sınıf bir IAuthorizationRequirement'ı implemente eden bir sınıf istiyor dinamik olarak o'da yukarıdaki BirthDateRequirement olacak 
    public class BirthDateRequirementHandler : AuthorizationHandler<BirthDateRequirement> 
    {
        // AuthorizationHandler'dan gelen abstract method'u implemente ediyoruz
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BirthDateRequirement requirement)
        {
            var birthDateClaim = context.User.Claims.Where(x => x.Type == ClaimTypes.DateOfBirth).FirstOrDefault(); // token'ın payload'ından claim'i aldık

            if (birthDateClaim == null)
            {
                context.Fail(); // birthDateClaim adlı Claim nesnesi yoksa, bu metot sayesinde  ilgili endpoint'e istek yapamaz gelen request
                return Task.CompletedTask;
            }

            var today = DateTime.Now;

            var age = today.Year - Convert.ToDateTime(birthDateClaim.Value).Year;

            if (age >= requirement.RequiredAge)
            {
                context.Succeed(requirement); // istek başarıyla yürütülür
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
