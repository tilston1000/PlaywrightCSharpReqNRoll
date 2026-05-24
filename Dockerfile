# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Install Node.js (required for Playwright CLI and Allure CLI)
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs

# Set the working directory
WORKDIR /app

# Copy only csproj and solution files for restore (improves cache)
COPY *.csproj ./
COPY *.sln ./



# Install Allure CLI globally
RUN npm install -g allure-commandline

# Add .NET tools to PATH
ENV PATH="$PATH:/root/.dotnet/tools"


RUN dotnet restore

# Copy the rest of the source code
COPY . .


RUN dotnet build --no-restore

# Install Playwright CLI and browsers after build (ensures Microsoft.Playwright is present)
RUN dotnet tool install --global Microsoft.Playwright.CLI && \
    /root/.dotnet/tools/playwright install

# Copy entrypoint script
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Use JSON array form for CMD
CMD ["/entrypoint.sh"]