# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Install Node.js (required for Playwright CLI and Allure CLI)
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs && \
    apt-get install -y \
        libglib2.0-0 \
        libnss3 \
        libnspr4 \
        libdbus-1-3 \
        libatk1.0-0 \
        libatk-bridge2.0-0 \
        libcups2 \
        libdrm2 \
        libatspi2.0-0 \
        libx11-6 \
        libxcomposite1 \
        libxdamage1 \
        libxext6 \
        libxfixes3 \
        libxrandr2 \
        libgbm1 \
        libxcb1 \
        libxkbcommon0 \
        libpango-1.0-0 \
        libcairo2 \
        libasound2 && \
    rm -rf /var/lib/apt/lists/*

# Set the working directory
WORKDIR /app


# Prevent NuGet from using a Windows fallback package folder (fixes CI build error)
ENV NUGET_FALLBACK_PACKAGES=""
# Force NuGet to use an isolated package cache
ENV NUGET_PACKAGES=/tmp/nuget

COPY nuget.config ./
# Remove any FallbackPackageFolders lines from nuget.config (prevents Windows fallback errors)
RUN sed -i '/FallbackPackageFolders/d' /app/nuget.config

COPY Directory.Packages.props ./
COPY *.csproj ./
COPY *.sln ./



# Remove global NuGet config and fallback folders to prevent Windows fallback pollution
RUN rm -rf /root/.nuget /usr/share/dotnet/sdk/NuGetFallbackFolder || true

# Clear all NuGet caches
RUN dotnet nuget locals all --clear

# Debug: print all NuGet.Config files found in the image
RUN find / -name NuGet.Config -print || true

ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet restore --configfile nuget.config


# Copy only source and config files, not bin/obj or build outputs
COPY Config ./Config
COPY Drivers ./Drivers
COPY Features ./Features
COPY Helpers ./Helpers
COPY Hooks ./Hooks
COPY Pages ./Pages
COPY StepDefinitions ./StepDefinitions
COPY appsettings.json ./
COPY entrypoint.sh ./



# Publish the project (ensures Playwright assets are generated)
RUN dotnet publish --no-restore --output ./publish --configfile nuget.config


# Install Playwright CLI and browsers after publish (ensures Microsoft.Playwright is present)
RUN dotnet tool install --global Microsoft.Playwright.CLI && \
    /root/.dotnet/tools/playwright install

# Copy entrypoint script
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Use JSON array form for CMD
CMD ["/entrypoint.sh"]