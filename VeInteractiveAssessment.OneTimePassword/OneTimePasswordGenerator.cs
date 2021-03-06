using System;
using System.Collections.Generic;

namespace VeInteractiveAssessment.OneTimePassword
{
    public class OneTimePasswordGenerator
    {
        private readonly IDateTime _dateTime;
        private readonly Dictionary<string, OneTimePassword> _storedUserPasswords = new Dictionary<string, OneTimePassword>();

        public OneTimePasswordGenerator(IDateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public string GenerateFor(string userId)
        {
            var password = Guid.NewGuid().ToString();
            var createdTime = _dateTime.Now();
            var oneTimePassword = new OneTimePassword(password, createdTime);

            _storedUserPasswords.Add(userId, oneTimePassword);

            return password;
        }

        public bool Validate(string userId, string attemptedPassword)
        {
            OneTimePassword storedPassword;
            _storedUserPasswords.TryGetValue(userId, out storedPassword);

            if (storedPassword != null)
            {
                if (PasswordHasNotExpired(storedPassword) && PasswordIsCorrectForUser(attemptedPassword, storedPassword))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool PasswordIsCorrectForUser(string attemptedPassword, OneTimePassword storedPassword)
        {
            return attemptedPassword == storedPassword.Value;
        }

        private bool PasswordHasNotExpired(OneTimePassword storedPassword)
        {
            var timeDifference = _dateTime.Now() - storedPassword.CreatedDateDateTime;
            return timeDifference.TotalSeconds <= 30;
        }
    }
}