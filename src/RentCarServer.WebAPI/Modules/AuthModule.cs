using RentCarServer.Application.Auth;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.WebAPI.Modules
{
    public static class AuthModule
    {
        public static void MapAuthEndPoint(this IEndpointRouteBuilder builder)
        {
            var app = builder.MapGroup("/auth");
            app.MapPost("/login",
                async (LoginCommand request, ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(request, cancellationToken);
                    return result.IsSuccessful ? Results.Ok(result) : Results.InternalServerError(result);
                })
                .Produces<Result<string>>()
                .RequireRateLimiting("login-fixed");

            app.MapPost("/forgot-password/{email}",
                async (string email, ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new ForgotPasswordCommand(email), cancellationToken);
                    return result.IsSuccessful ? Results.Ok(result) : Results.InternalServerError(result);
                })
                .Produces<Result<string>>()
                .RequireRateLimiting("forgot-password-fixed");

            app.MapPost("/reset-password",
                async (ResetPasswordCommand request ,string email, ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(request, cancellationToken);
                    return result.IsSuccessful ? Results.Ok(result) : Results.InternalServerError(result);
                })
                .Produces<Result<string>>()
                .RequireRateLimiting("forgot-password-fixed");
        }
    }
}
