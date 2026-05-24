FROM mcr.microsoft.com/dotnet/sdk:9.0

# Install Node.js (required for Playwright CLI and Allure CLI), Java (for Allure), and Allure CLI
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs openjdk-17-jre-headless \
        libglib2.0-0 libnss3 libnspr4 libdbus-1-3 libatk1.0-0 libatk-bridge2.0-0 \
        libcups2 libdrm2 libatspi2.0-0 libx11-6 libxcomposite1 libxdamage1 libxext6 \
        libxfixes3 libxrandr2 libgbm1 libxcb1 libxkbcommon0 libpango-1.0-0 libcairo2 \
        libasound2 libx11-xcb1 libxcursor1 libgtk-3-0 libpangocairo-1.0-0 \
        libcairo-gobject2 libgdk-pixbuf2.0-0 && \
    rm -rf /var/lib/apt/lists/* && \
    npm install -g allure-commandline --save-dev

WORKDIR /app

# NuGet environment
ENV NUGET_FALLBACK_PACKAGES=""
ENV NUGET_PACKAGES=/tmp/nuget
ENV PATH="$PATH:/root/.dotnet/tools"


# Copy only dependency files first for better Docker cache utilization
COPY nuget.config ./
COPY Directory.Packages.props ./
COPY *.csproj ./
COPY *.sln ./

# Remove any FallbackPackageFolders lines from nuget.config
RUN sed -i '/FallbackPackageFolders/d' /app/nuget.config

# Clean up global NuGet config and caches
RUN rm -rf /root/.nuget /usr/share/dotnet/sdk/NuGetFallbackFolder || true
RUN dotnet nuget locals all --clear
RUN find / -name NuGet.Config -print || true

# Restore NuGet packages (this layer will be cached unless dependency files change)
RUN dotnet restore --configfile nuget.config

# Install Playwright CLI and browsers (cache this layer unless dependencies change)
RUN dotnet tool install --global Microsoft.Playwright.CLI && \
    playwright install

# Now copy the rest of the source and config files
COPY Config ./Config
COPY Drivers ./Drivers
COPY Features ./Features
COPY Helpers ./Helpers
COPY Hooks ./Hooks
COPY Pages ./Pages
COPY StepDefinitions ./StepDefinitions
COPY appsettings.json ./
COPY entrypoint.sh ./

# Publish the project
RUN dotnet publish --no-restore --output ./publish --configfile nuget.config

# Make entrypoint script executable
RUN chmod +x /app/entrypoint.sh

# Use JSON array form for CMD
CMD ["/app/entrypoint.sh"]