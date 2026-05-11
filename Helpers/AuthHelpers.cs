using playwrightreqnroll.Pages;

namespace playwrightreqnroll.Helpers;

public static class AuthHelpers
{
        public static async Task LoginWithEnvCredentials(HomePage home)
        {
            // Debug: Print all environment variables
            Console.WriteLine("--- ENVIRONMENT VARIABLES ---");
            foreach (System.Collections.DictionaryEntry env in Environment.GetEnvironmentVariables())
            {
                Console.WriteLine($"{env.Key}={env.Value}");
            }
            Console.WriteLine("----------------------------");

            var username = Environment.GetEnvironmentVariable("TEST_USERNAME");
            var password = Environment.GetEnvironmentVariable("TEST_PASSWORD");
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new InvalidOperationException("USERNAME or PASSWORD environment variable is not set.");

            await home.Login(username, password);
        }
}
