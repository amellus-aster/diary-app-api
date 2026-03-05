using DiaryApi.Models;
using Microsoft.EntityFrameworkCore;

public class DiaryDbContext : DbContext
{
    public DiaryDbContext(DbContextOptions<DiaryDbContext> options) : base(options)
    {
    }
    public DbSet<UserModel> Users {get; set;}
    public DbSet<DiaryModel> Diaries {get; set;}

}