using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dto
{
    public class ErrorDto
    {
        public List<string> Errors { get; private set; }

        public bool IsShow { get; private set; } // hata kullanıcıya gösterilecek mi ?

        public ErrorDto()
        {
            Errors = new List<string>();
        }

        public ErrorDto(string error, bool isShow)
        {
            Errors = new List<string>();
            Errors.Add(error);
            IsShow = isShow;
        }

        public ErrorDto(List<string> errors,bool isShow)
        {
            Errors=errors;
            IsShow = isShow;
        }

    }
}
