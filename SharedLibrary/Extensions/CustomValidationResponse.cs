using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    public static class CustomValidationResponse
    {
        public static void AddCustomValidationResponse(this IServiceCollection services) //builder.service.AddCustomValidationResponse() diyerek ulaşacağız
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                // InvalidModelStateResponseFactory bir func delegate alıyor, delegate de bir ActionContext nesnesi alıp IActionResult dönüyor.
                // InvalidModelStateResponseFactory metot'u ile ile ModelState invalid olduğunda bir response oluşturmamızı sağlar. Bunu yapılandıralım
                options.InvalidModelStateResponseFactory = context =>
                {
                    // ilk Hataları alalım, Hatalar nereden Gelicek bize ?
                    var errors = context.ModelState.Values
                                .Where(x => x.Errors.Count > 0)   // bakarsan, buradan IEnumerable<ModelStateEntry> listesi dönecek
                                .SelectMany(x=>x.Errors)          // Bu ModelStateEntry'nin Errors'larını alalım, Buradan da ModelErrorCollection dönüyor.
                                .Select(x => x.ErrorMessage)      // Yani ModelError listesi dönüyor.Bu sınıfında ErrorMessage property'sini alalım 
                                .ToList();

                    // Aldığımız hataları özel Custom Response nesnemize ekleyip Client'a gönderelim

                    ErrorDto errorDto = new ErrorDto(errors,true);

                    var response = ResponseDto<NoDataDto>.Fail(errorDto,StatusCodes.Status400BadRequest);

                    return new BadRequestObjectResult(response); // Geriye IActionResult'ımızı döndük Func delegate'inin istediği gibi
                };
            });
        }
    }
}
