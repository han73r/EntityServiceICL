using EntityService.Application.Interfaces;
using EntityService.Application.Services;
using EntityService.Domain.Interfaces;
using EntityService.Infrastructure.Repositories;
using EntityService.Web.Filters;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EntityServiceAPI", Version = "v1" });
    //c.OperationFilter<CustomOperationFilter>();
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Register application services
builder.Services.AddScoped<IEntityService, EntityServiceImpl>();
builder.Services.AddScoped<IEntityRepository, InMemoryEntityRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EntityServiceAPI");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();