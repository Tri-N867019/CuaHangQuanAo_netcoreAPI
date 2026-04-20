# Sử dụng base image .NET 8 Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Sử dụng base image .NET 8 SDK để build code
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy toàn bộ các file .csproj của tất cả 4 project để Restore (Tải thư viện)
COPY ["CuaHangQuanAo/CuaHangQuanAo.API.csproj", "CuaHangQuanAo/"]
COPY ["CuaHangQuanAo.Application/CuaHangQuanAo.Application.csproj", "CuaHangQuanAo.Application/"]
COPY ["CuaHangQuanAo.Domain/CuaHangQuanAo.Domain.csproj", "CuaHangQuanAo.Domain/"]
COPY ["CuaHangQuanAo.Infrastructure/CuaHangQuanAo.Infrastructure.csproj", "CuaHangQuanAo.Infrastructure/"]

# Restore thư viện cho project chính (nó sẽ tự kéo các project con theo)
RUN dotnet restore "CuaHangQuanAo/CuaHangQuanAo.API.csproj"

# Copy toàn bộ mã nguồn còn lại vào Docker
COPY . .

# Chuyển thư mục làm việc vào project API và tiến hành Build
WORKDIR "/src/CuaHangQuanAo"
RUN dotnet build "CuaHangQuanAo.API.csproj" -c Release -o /app/build

# Publish (Đóng gói) dự án
FROM build AS publish
RUN dotnet publish "CuaHangQuanAo.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Chạy ứng dụng
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CuaHangQuanAo.API.dll"]