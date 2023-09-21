using MagicConchShell.Models;
using MagicConchShell.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectString = builder.Configuration["ConnectionString:DefaultConnection"];

builder.Services.AddDbContext<SpongebobsContext>(options => options.UseMySQL(connectString));

builder.Services.Configure<LineBotSettings>(builder.Configuration.GetSection("LineBot"));

builder.Services.AddSingleton<LineBotService>();


builder.Services.AddControllers();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    policy.AllowAnyOrigin();
    policy.AllowAnyMethod();
    policy.AllowAnyHeader();
}));


var app = builder.Build();

app.UseCors();



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
