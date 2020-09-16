FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./housingResidentInformationAPI.sln ./
COPY ./housingResidentInformationAPI/housingResidentInformationAPI.csproj ./housingResidentInformationAPI/
COPY ./housingResidentInformationAPI.Tests/housingResidentInformationAPI.Tests.csproj ./housingResidentInformationAPI.Tests/

RUN dotnet restore ./housingResidentInformationAPI/housingResidentInformationAPI.csproj
RUN dotnet restore ./housingResidentInformationAPI.Tests/housingResidentInformationAPI.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out housingResidentInformationAPI.Tests/housingResidentInformationAPI.Tests.csproj

CMD dotnet test
