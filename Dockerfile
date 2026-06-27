# 1. Setup Build Environment
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
COPY . /source
WORKDIR /source

ARG TARGETARCH

# 2. Compile and Publish using high-speed local cache mounts
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

# 3. Setup Runtime Environment
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

# Copy compiled binaries from build stage
COPY --from=build /app .

# Run as a secure, non-root system user
USER $APP_UID

ENTRYPOINT ["dotnet", "SecureAuthDemo.dll"]