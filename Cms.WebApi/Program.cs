using Cms.Data.Repository.Repositories;
using Cms.WebApi.Mappers;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<ICmsRepository, InMemoryCmsRepository>();
builder.Services.AddAutoMapper(typeof(CmsMapper));

builder.Services.AddApiVersioning(setupAction=>{
    setupAction.AssumeDefaultVersionWhenUnspecified=true;
    setupAction.DefaultApiVersion=new Microsoft.AspNetCore.Mvc.ApiVersion(1,0);

    //setupAction.ApiVersionReader=new QueryStringApiVersionReader("api-version");
    //setupAction.ApiVersionReader=new UrlSegmentApiVersionReader();
    setupAction.ApiVersionReader=new HeaderApiVersionReader("X-Version");
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
