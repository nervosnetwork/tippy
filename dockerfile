#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.   
FROM nervos/ckb-docker-builder:bionic-rust-1.51.0 as ckb-docker-builder
WORKDIR /app
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Tippy/Tippy.csproj", "src/Tippy/"]
COPY ["src/Ckb.Molecule/Ckb.Molecule.csproj", "src/Ckb.Molecule/"]
COPY ["src/Ckb.Cryptography/Ckb.Cryptography.csproj", "src/Ckb.Cryptography/"]
COPY ["src/Ckb.Types/Ckb.Types.csproj", "src/Ckb.Types/"]
COPY ["src/Tippy.Shared/Tippy.Shared.csproj", "src/Tippy.Shared/"]
COPY ["src/Tippy.Ctrl/Tippy.Ctrl.csproj", "src/Tippy.Ctrl/"]
COPY ["src/Tippy.Core/Tippy.Core.csproj", "src/Tippy.Core/"]
COPY ["src/Ckb.Rpc/Ckb.Rpc.csproj", "src/Ckb.Rpc/"]
COPY ["src/Tippy.Util/Tippy.Util.csproj", "src/Tippy.Util/"]
COPY ["src/Ckb.Address/Ckb.Address.csproj", "src/Ckb.Address/"]
RUN dotnet restore "src/Tippy/Tippy.csproj"
COPY . .
WORKDIR "/src/src/Tippy"
RUN dotnet build "Tippy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tippy.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tippy.dll"]
