using AuthServer.Core.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Validations
{
    public class AssignRoleToUserDtoValidator : AbstractValidator<AssignRoleToUserDto>
    {
        public AssignRoleToUserDtoValidator()
        {
            RuleFor(x=>x.UserId).NotEmpty().WithMessage("User informations are invalid.");

            RuleFor(x => x.RoleName).NotEmpty().WithMessage("RoleName is invalid.")
                                    .MinimumLength(3).WithMessage("RoleName requires minimum 3 characters");
        }
    }
}
