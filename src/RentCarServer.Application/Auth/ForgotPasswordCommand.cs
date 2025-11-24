using FluentValidation;
using GenericRepository;
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
    public sealed record ForgotPasswordCommand(string email) : IRequest<Result<string>>;

    public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Geçerli bir mail adresi girin.")
                .EmailAddress().WithMessage("Geçerli bir mail adresi girin.");
        }
    }

    internal sealed class ForgotPasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IMailService mailService) : IRequestHandler<ForgotPasswordCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.FirstOrDefaultAsync(u => u.Email.value == request.email, cancellationToken);

            if (user is null) return Result<string>.Failure("Kullanıcı bulunamadı.");

            user.CreateForgotPasswordId();
            await unitOfWork.SaveChangesAsync(cancellationToken);
            string to = user.Email.value;
            string subject = "Şifre Sıfırla";
            string body = @"Şifre yenileme içeriği";
            await mailService.SendAsync(to, subject, body, cancellationToken);

            return "Şifre sıfırlama mailiniz gönderilmiştir. lütfen mail adresinizi kontrol ediniz.";
        }
    }
}
