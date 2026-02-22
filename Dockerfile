# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution-level config files
COPY .editorconfig .
COPY stylecop.json .

# Copy the project file and restore dependencies
COPY BookSearchApi/BookSearchApi.csproj BookSearchApi/
RUN dotnet restore BookSearchApi/BookSearchApi.csproj

# Copy the rest of the source code and build
COPY BookSearchApi/ BookSearchApi/
RUN dotnet publish BookSearchApi/BookSearchApi.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=5000

EXPOSE 5000
EXPOSE 5001

ENTRYPOINT ["dotnet", "BookSearchApi.dll"]