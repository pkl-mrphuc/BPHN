#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BPHN.MailWorker/BPHN.MailWorker.csproj", "BPHN.MailWorker/"]
COPY ["BPHN.ImpRabbitMQLayer/BPHN.ImpRabbitMQLayer.csproj", "BPHN.ImpRabbitMQLayer/"]
COPY ["BPHN.BusinessLayer/BPHN.BusinessLayer.csproj", "BPHN.BusinessLayer/"]
COPY ["BPHN.DataLayer/BPHN.DataLayer.csproj", "BPHN.DataLayer/"]
COPY ["BPHN.ModelLayer/BPHN.ModelLayer.csproj", "BPHN.ModelLayer/"]
COPY ["BPHN.IRabbitMQLayer/BPHN.IRabbitMQLayer.csproj", "BPHN.IRabbitMQLayer/"]
RUN dotnet restore "BPHN.MailWorker/BPHN.MailWorker.csproj"
COPY . .
WORKDIR "/src/BPHN.MailWorker"
RUN dotnet build "BPHN.MailWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BPHN.MailWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BPHN.MailWorker.dll"]