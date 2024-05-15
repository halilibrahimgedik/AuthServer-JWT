using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Dto;
using SharedLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    public static class CustomGlobalExceptionHandler
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            // dotnet core'un sunduğu hazır UseExceptionHandler middleware'i üzerinden tüm hataları yakalayabiliriz
            app.UseExceptionHandler(configure =>
            {
                configure.Run(async context =>
                {
                    // 1-) response'un media type'ını belirle
                    // 2-) context nesnesinin Features'ı üzerinden IExceptionHandlerFeature nesnesini al 
                    // 3-) context'in response'ına statusCode'unu ata.

                    context.Response.ContentType = MediaTypeNames.Application.Json; // "application/json" demek yerine MediaTypeNames sınıfını kullanabiliriz

                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (exceptionFeature != null)
                    {
                        var responseStatusCode = context.Response.StatusCode;

                        responseStatusCode = exceptionFeature.Error switch
                        {
                            ClientSideException => StatusCodes.Status400BadRequest,

                            NotFoundException => StatusCodes.Status404NotFound,

                            _ => StatusCodes.Status500InternalServerError,
                        };

                        var response = ResponseDto<NoDataDto>.Fail(exceptionFeature.Error.Message, responseStatusCode, true); // bu hata kullanıcıya gösterilsin

                        await context.Response.WriteAsync(JsonSerializer.Serialize(response)); // string'e dönüştürüp response'a yazdık
                    }
                });
            });
        }
    }
}
