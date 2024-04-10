# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

USER app
# Install libs for Tesseract
#USER root
#RUN apt-get update && \
    #apt-get install -y --no-install-recommends \
    #git \
    #cmake \
    #build-essential \
    #libleptonica-dev \
    #libtesseract-dev \
    #&& rm -rf /var/lib/apt/lists/*
#
#RUN git clone --depth 1 --branch 1.80.0 https://github.com/DanBloomberg/leptonica.git /leptonica 
#
#WORKDIR /leptonica/build
#RUN cmake ..
#
#USER app
#WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["QueueApp/QueueApp.csproj", "QueueApp/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
RUN dotnet restore "./QueueApp/QueueApp.csproj"
COPY . .
WORKDIR "/src/QueueApp"
RUN dotnet build "./QueueApp.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN ls ./runtimes/linux-x64/native
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./QueueApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# copy Tesseract missing dependencies
#COPY --from=build /src/QueueApp/runtimes/linux-x64/native /app/x64
# Link libs for Tesseract
#WORKDIR /app/x64

#RUN ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 libleptonica-1.82.0.so
#RUN ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.4.0.1 libtesseract41.so
#WORKDIR /app

ENTRYPOINT ["dotnet", "QueueApp.dll"]
