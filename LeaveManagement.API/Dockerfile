FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy only API project files
COPY LeaveManagement.API.csproj ./
RUN dotnet restore LeaveManagement.API.csproj

# copy everything else INSIDE this folder only
COPY . ./
RUN dotnet publish LeaveManagement.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LeaveManagement.API.dll"]
