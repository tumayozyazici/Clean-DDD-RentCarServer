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
                    return result.IsSuccessful ? Results.Ok(result) : Results.Unauthorized();
                }).Produces<Result<string>>();
        }
    }
}
