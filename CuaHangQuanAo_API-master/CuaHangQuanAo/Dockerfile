# Sử dụng base image .NET 8 SDK để build code
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# SỬA TẠI ĐÂY: Copy file csproj vào đúng thư mục làm việc của Docker
# Cấu trúc: COPY "đường dẫn file từ máy" "đường dẫn trong docker"
COPY ["CuaHangQuanAo/CuaHangQuanAo.API.csproj", "CuaHangQuanAo/"]
COPY ["CuaHangQuanAo.Application/CuaHangQuanAo.Application.csproj", "CuaHangQuanAo.Application/"]
COPY ["CuaHangQuanAo.Domain/CuaHangQuanAo.Domain.csproj", "CuaHangQuanAo.Domain/"]
COPY ["CuaHangQuanAo.Infrastructure/CuaHangQuanAo.Infrastructure.csproj", "CuaHangQuanAo.Infrastructure/"]

# Restore
RUN dotnet restore "CuaHangQuanAo/CuaHangQuanAo.API.csproj"

# Copy toàn bộ mã nguồn (Lệnh này sẽ copy từ thư mục Root Directory trên Render)
COPY . .

# Build dự án
WORKDIR "/src/CuaHangQuanAo"
RUN dotnet build "CuaHangQuanAo.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "CuaHangQuanAo.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CuaHangQuanAo.API.dll"]