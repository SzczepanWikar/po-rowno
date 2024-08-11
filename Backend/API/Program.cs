using System.Text;
using API.User.AuthView;
using Application;
using Infrastructure.CQRS;
using Infrastructure.Email;
using Infrastructure.EventStore;
using Infrastructure.Logging;
using Infrastructure.Middleware.Auth;
using Infrastructure.Middleware.ErrorHandling;
using Infrastructure.Projections;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEventStore(builder.Configuration);
builder.Services.AddCQRS();
builder.Services.AddBusinessLogic(builder.Configuration);
builder.Services.AddMailing(builder.Configuration);
builder.Services.AddHttpExceptionHandlingMiddleware();
builder.Services.AddControllersWithViews();
builder.Services.AddAuth(builder.Configuration);

builder.Logging.ConfigureLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpExceptionHandlingMiddleware();
app.UseRouting();
app.UseAuth();
app.UseAuthViews();
app.UseHttpsRedirection();
app.UseEventStore();
app.UseCQRS();
app.UseProjections();
app.UseBusinessLogic();
app.MapControllers();

app.Run();
