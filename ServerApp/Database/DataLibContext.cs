using Microsoft.EntityFrameworkCore;
using DataLib;
using System.Reflection;

namespace ServerApp.Database;

/// <summary>
/// <see cref="DataLib"/> 모델 라이브러리를 기반으로 하는 EF Core 데이터베이스 컨텍스트 클래스<para/>
/// EF Core database context class based on <see cref="DataLib"/> model library
/// </summary>
public class DataLibContext : DbContext {
    
    public DataLibContext(DbContextOptions<DataLibContext> options) : base(options) {
        
    }
    
    /// <summary>
    /// <see cref="UserData"/> 테이블<para/>
    /// Table of <see cref="UserData"/>
    /// </summary>
    public DbSet<UserData>? Users { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder) {
        
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
#if DEBUG
            .AddJsonFile("appsettings.Development.json")
#else
            .AddJsonFile("appsettings.json")
#endif
            .Build();
        
        builder.UseSqlite($"Data Source={configuration["SQLitePath"]};", options => {
            
            options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
        });
        
        base.OnConfiguring(builder);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserData>().ToTable(nameof(UserData));
    }
}