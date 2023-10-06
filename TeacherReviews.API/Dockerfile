FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TeacherReviews.API/TeacherReviews.API.csproj", "TeacherReviews.API/"]
COPY ["TeacherReviews.Domain/TeacherReviews.Domain.csproj", "TeacherReviews.Domain/"]
RUN dotnet restore "TeacherReviews.API/TeacherReviews.API.csproj"
COPY . .
WORKDIR "/src/TeacherReviews.API"
RUN dotnet build "TeacherReviews.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeacherReviews.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeacherReviews.API.dll"]
