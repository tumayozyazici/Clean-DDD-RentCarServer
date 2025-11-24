using FluentValidation;
using GenericRepository;
using RentCarServer.Domain.Users;
using RentCarServer.Domain.Users.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Auth
{
    public sealed record ResetPasswordCommand(Guid forgotPasswordId, string newPassword): IRequest<Result<string>>;
    public sealed class ResetPassowrdCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPassowrdCommandValidator()
        {
            RuleFor(x=>x.newPassword).
                NotEmpty().WithMessage("Şifre boş geçilemez.");
        }
    }

    internal sealed class ResetPasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<ResetPasswordCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository
                .FirstOrDefaultAsync(u =>
                u.ForgotPasswordId!=null
                && u.ForgotPasswordId.value == request.forgotPasswordId
                && u.IsForgotPasswordCompleted.value == false,
                cancellationToken);

            if(user is null) return Result<string>.Failure("Şifre sıfırlama değeriniz geçersiz.");

            var fpDate = user.ForgotPasswordDate!.value;
            var now =  new DateTimeOffset().AddDays(1);
            if(fpDate<now) return Result<string>.Failure("Şifre sıfırlama değeriniz geçersiz.");

            Password password = new(request.newPassword);
            user.SetPassword(password);
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return "Şifreniz başarıyla güncellenmiştir.";
        }
    }
}
