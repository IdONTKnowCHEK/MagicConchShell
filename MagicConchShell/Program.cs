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
builder.Services.AddSwaggerGen();

builder.Services.ConfigureSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "spongebobs",
        Version = "v1"
    });
});

var app = builder.Build();
app.UseSwagger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
