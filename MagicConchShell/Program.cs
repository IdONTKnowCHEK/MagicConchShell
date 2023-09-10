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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
