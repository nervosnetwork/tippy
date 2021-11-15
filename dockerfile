# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY src/Ckb.Address/*.csproj ./aspnetapp/
COPY src/Ckb.Cryptography/*.csproj ./aspnetapp/
COPY src/Ckb.Molecule/*.csproj ./aspnetapp/
COPY src/Ckb.Rpc/*.csproj ./aspnetapp/
COPY src/Ckb.Types/*.csproj ./aspnetapp/
COPY src/Tippy/*.csproj ./aspnetapp/
COPY src/Tippy.Core/*.csproj ./aspnetapp/
COPY src/Tippy.Ctrl/*.csproj ./aspnetapp/
COPY src/Tippy.Shared/*.csproj ./aspnetapp/
COPY src/Tippy.Util/*.csproj ./aspnetapp/
RUN dotnet restore

# copy everything else and build app
COPY aspnetapp/. ./aspnetapp/
WORKDIR /source/aspnetapp
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Tippy.dll"]
#FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
#ENV LANG C.UTF-8
#EXPOSE 5884
#WORKDIR /app
#COPY . .
#ENTRYPOINT ["dotnet", "Tippy.dll"]
