FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./HousingResidentInformationAPI.sln ./
COPY ./HousingResidentInformationAPI/HousingResidentInformationAPI.csproj ./HousingResidentInformationAPI/
COPY ./HousingResidentInformationAPI.Tests/HousingResidentInformationAPI.Tests.csproj ./HousingResidentInformationAPI.Tests/

RUN dotnet restore ./HousingResidentInformationAPI/HousingResidentInformationAPI.csproj
RUN dotnet restore ./HousingResidentInformationAPI.Tests/HousingResidentInformationAPI.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out HousingResidentInformationAPI.Tests/HousingResidentInformationAPI.Tests.csproj

CMD dotnet test
