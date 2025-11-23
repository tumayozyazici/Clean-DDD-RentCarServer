using GenericRepository;
using RentCarServer.Domain.Users;
using RentCarServer.Domain.Users.ValueObjects;

namespace RentCarServer.WebAPI
{
    public static class ExtensionMethods
    {
        public static async Task CreateFirstUser(this IApplicationBuilder app)
        {
            using var scoped = app.ApplicationServices.CreateScope();
            var userRepository = scoped.ServiceProvider.GetRequiredService<IUserRepository>();
            var unitOfWork = scoped.ServiceProvider.GetRequiredService<IUnitOfWork>();

            if (!(await userRepository.AnyAsync(x => x.UserName.value == "admin")))
            {
                FirstName firstName = new("Taner");
                LastName lastName = new("Saydam");
                Email email = new("tanersaydam@gmail.com");
                UserName userName = new("admin");
                Password password = new("1");

                var user = new User(
                    firstName,
                    lastName,
                    email,
                    userName,
                    password);

                userRepository.Add(user);
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}
