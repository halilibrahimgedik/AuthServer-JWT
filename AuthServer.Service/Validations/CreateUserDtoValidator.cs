﻿using AuthServer.Core.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Validations
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
                                .EmailAddress().WithMessage("Email is invalid");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");

            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required");

        }
    }
}
