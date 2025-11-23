using FluentValidation;
using RentCarServer.Application.Services;
using RentCarServer.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Auth
{
    public sealed record LoginCommand(string UserNameOrEmail,string Password):IRequest<Result<string>>;

    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x=>x.UserNameOrEmail).NotEmpty().WithMessage("Geçerli bir mail veya kullanıcı adı girin.");
            RuleFor(x=>x.Password).NotEmpty().WithMessage("Geçerli bir şifre girin.");
        }
    }

    public sealed class LoginCommandHandler(IUserRepository userRepository,IJwtProvider jwtProvider) : IRequestHandler<LoginCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.FirstOrDefaultAsync(x => x.Email.value == request.UserNameOrEmail || x.UserName.value == request.UserNameOrEmail);

            if(user is null) return Result<string>.Failure("Kullanıcı adı veya şifre geçersiz.");

            var isPasswordChecked = user.VerifyPasswordHash(request.Password);
            if (!isPasswordChecked) return Result<string>.Failure("Kullanıcı adı veya şifre geçersiz.");
            var token = jwtProvider.CreateToken(user);
            return token;
        }
    }
}
