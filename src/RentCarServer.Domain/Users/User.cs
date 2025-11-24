using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.Users.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarServer.Domain.Users
{
    public sealed class User : Entity
    {
        public User(FirstName firstName, LastName lastName, Email email, UserName userName, Password password)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            UserName = userName;
            Password = password;
            FullName = new(firstName.value + " " + lastName.value + " (" + Email.value + ")");
            IsForgotPasswordCompleted = new(true);
        }
        private User() { }
        public FirstName FirstName { get; private set; } = default!;
        public LastName LastName { get; private set; } = default!;
        public FullName FullName { get; private set; } = default!;
        public Email Email { get; private set; } = default!;
        public UserName UserName { get; private set; } = default!;
        public Password Password { get; private set; } = default!;
        public ForgotPasswordId? ForgotPasswordId { get; private set; }
        public ForgotPasswordDate? ForgotPasswordDate { get; private set; }
        public IsForgotPasswordCompleted IsForgotPasswordCompleted { get; private set; } =default!;

        public bool VerifyPasswordHash(string password)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(Password.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(Password.PasswordHash);
        }
        public void CreateForgotPasswordId()
        {
            ForgotPasswordId = new(Guid.NewGuid());
            ForgotPasswordDate = new(DateTimeOffset.UtcNow);
            IsForgotPasswordCompleted = new(false);
        }
        public void SetPassword (Password password)
        {
            Password = password;
        }
    }
}
