using System.Text.RegularExpressions;

namespace HalaqatBackend.Utils
{
    public static class PasswordValidator
    {
        private const int MinimumLength = 8;

        public static bool IsValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (password.Length < MinimumLength)
            {
                return false;
            }

            if (!HasDigit(password))
            {
                return false;
            }

            if (!HasSymbol(password))
            {
                return false;
            }

            if (!HasUpperCase(password))
            {
                return false;
            }

            return true;
        }

        public static void ValidateOrThrow(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty");
            }

            if (password.Length < MinimumLength)
            {
                throw new ArgumentException($"Password must be at least {MinimumLength} characters long");
            }

            if (!HasDigit(password))
            {
                throw new ArgumentException("Password must contain at least one digit");
            }

            if (!HasSymbol(password))
            {
                throw new ArgumentException("Password must contain at least one special character");
            }

            if (!HasUpperCase(password))
            {
                throw new ArgumentException("Password must contain at least one uppercase letter");
            }
        }

        public static string GetValidationRules()
        {
            return $"Password must be at least {MinimumLength} characters, including uppercase, lowercase, number, and special character.";
        }

        private static bool HasDigit(string password)
        {
            return password.Any(char.IsDigit);
        }

        private static bool HasSymbol(string password)
        {
            return Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");
        }

        private static bool HasUpperCase(string password)
        {
            return password.Any(char.IsUpper);
        }
    }
}
