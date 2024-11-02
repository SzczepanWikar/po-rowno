using API.User.AuthView;
using Infrastructure.CQRS;
using Infrastructure.Email;
using Infrastructure.EventStore;
using Infrastructure.Logging;
using Infrastructure.Middleware.Auth;
using Infrastructure.Middleware.ErrorHandling;
using Infrastructure.Middleware.UserFetching;
using Infrastructure.Projections;
using ReadModel;
using WriteModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEventStore(builder.Configuration);
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(p =>
        p.AllowAnyHeader().AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()
    );
});
builder.Services.AddCQRS();
builder.Services.AddBusinessLogic(builder.Configuration);
builder.Services.AddMailing(builder.Configuration);
builder.Services.AddHttpExceptionHandlingMiddleware();
builder.Services.AddControllersWithViews();
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddDatabaseProjections(builder.Configuration);
builder.Services.AddUserFetchng();

builder.Logging.ConfigureLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpExceptionHandlingMiddleware();
app.UseRouting();
app.UseAuth();
app.UseUserFetching();
app.UseAuthViews();
app.UseHttpsRedirection();
app.UseEventStore();
app.UseCQRS();
app.UseProjections();
app.UseBusinessLogic();
app.MapControllers();

app.Run();
