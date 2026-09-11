# =============================================================================
#  Dockerfile — CulinaryBlog.API (multi-stage: SDK để build → aspnet runtime để chạy, CONS-009)
#
#  QUAN TRỌNG: build context PHẢI là THƯ MỤC GỐC repo (không phải src/CulinaryBlog.API/), vì
#  project này tham chiếu tới 3 project khác trong src/. Xem cách gọi đúng trong docker-compose.yml:
#      build: { context: ., dockerfile: src/CulinaryBlog.API/Dockerfile }
# =============================================================================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy trước các file .csproj để tận dụng Docker layer cache — chỉ "restore" lại khi
# package thay đổi, không phải mỗi khi sửa code C#.
COPY CulinaryBlog.sln .
COPY src/CulinaryBlog.Domain/CulinaryBlog.Domain.csproj src/CulinaryBlog.Domain/
COPY src/CulinaryBlog.Application/CulinaryBlog.Application.csproj src/CulinaryBlog.Application/
COPY src/CulinaryBlog.Infrastructure/CulinaryBlog.Infrastructure.csproj src/CulinaryBlog.Infrastructure/
COPY src/CulinaryBlog.API/CulinaryBlog.API.csproj src/CulinaryBlog.API/
RUN dotnet restore src/CulinaryBlog.API/CulinaryBlog.API.csproj

COPY src/ src/
RUN dotnet publish src/CulinaryBlog.API/CulinaryBlog.API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CulinaryBlog.API.dll"]
