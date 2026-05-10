using playwrightreqnroll.Pages;

namespace playwrightreqnroll.Helpers;

public static class AuthHelpers
{
        public static async Task LoginWithEnvCredentials(HomePage home)
        {
            var username = Environment.GetEnvironmentVariable("TEST_USERNAME");
            var password = Environment.GetEnvironmentVariable("TEST_PASSWORD");
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new InvalidOperationException("USERNAME or PASSWORD environment variable is not set.");

            await home.Login(username, password);
        }
}
