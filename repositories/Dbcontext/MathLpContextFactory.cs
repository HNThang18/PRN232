using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using applications.Interfaces; // Nơi chứa ICurrentUserService

namespace repositories.Dbcontext
{
    // 1. Tạo một lớp "giả" (mock) ICurrentUserService
    // Vì khi migration, không có user nào đăng nhập cả.
    public class DesignTimeCurrentUserService : ICurrentUserService
    {
        public int? GetUserId() => null; // Trả về null khi migration
        public string? GetIpAddress() => null; // Trả về null khi migration
    }

    // 2. Tạo lớp Factory
    public class MathLpContextFactory : IDesignTimeDbContextFactory<MathLpContext>
    {
        public MathLpContext CreateDbContext(string[] args)
        {
            // Lấy chuỗi kết nối từ file appsettings.json
            // Code này giả định bạn chạy lệnh 'dotnet ef' từ thư mục 'repositories'
            // Nó sẽ tìm file appsettings.json trong thư mục 'controllers'
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "../controllers");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<MathLpContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            // 3. Tạo DbContext với service "giả"
            return new MathLpContext(optionsBuilder.Options, new DesignTimeCurrentUserService());
        }
    }
}