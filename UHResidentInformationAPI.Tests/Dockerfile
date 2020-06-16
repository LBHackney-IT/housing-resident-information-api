FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./UHResidentInformationAPI.sln ./
COPY ./UHResidentInformationAPI/UHResidentInformationAPI.csproj ./UHResidentInformationAPI/
COPY ./UHResidentInformationAPI.Tests/UHResidentInformationAPI.Tests.csproj ./UHResidentInformationAPI.Tests/

RUN dotnet restore ./UHResidentInformationAPI/UHResidentInformationAPI.csproj
RUN dotnet restore ./UHResidentInformationAPI.Tests/UHResidentInformationAPI.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out UHResidentInformationAPI.Tests/UHResidentInformationAPI.Tests.csproj

CMD dotnet test
