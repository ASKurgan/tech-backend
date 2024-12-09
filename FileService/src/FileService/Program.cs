using Amazon.S3;
using FileService;
using FileService.Endpoints;
using FileService.Extensions;
using FileService.Jobs;
using FileService.MongoDataAccess;
using MongoDB.Driver;
using Hangfire;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddMinio(builder.Configuration);

builder.Services.AddEndpoints();

builder.Services.AddCors();

builder.Services.AddSingleton<IMongoClient>(new MongoClient(builder.Configuration.GetConnectionString("MongoConnection")));

builder.Services.AddScoped<FileMongoDbContext>();
builder.Services.AddScoped<IFilesRepository, FilesRepository>();

var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoConnection"));

builder.Services.AddHangfire(builder.Configuration);

builder.Services.AddHangfireServer(serverOptions => { serverOptions.ServerName = "Hangfire.Mongo server"; });

builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    var config = new AmazonS3Config
    {
        ServiceURL = "http://localhost:9000",
        ForcePathStyle = true,
        UseHttp = true
    };

    return new AmazonS3Client("minioadmin", "minioadmin", config);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.CreateHangfireDatabaseIfNotExists();

app.UseHangfireDashboard();

app.UseCors(c => c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.MapEndpoints();

app.Run();

public partial class Program;