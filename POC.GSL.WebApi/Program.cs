using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using POC.GSL.Data;
using POC.GSL.Data.Mongo;
using POC.GSL.WebApi.Diagnostics;
using POC.GSL.WebApi.Services;
using Serilog;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:4200", "https://localhost:4200","https://localhost:8080", "http://localhost:8080")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://securetoken.google.com/poc-gsl";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://securetoken.google.com/poc-gsl",
            ValidateAudience = true,
            ValidAudience = "poc-gsl",
            ValidateLifetime = true,
        };
    });

//builder.Services.AddAuthorization(options => {
//    options.AddPolicy(name: "Administrator", configurePolicy: policy => policy.RequireRole("administrator"));
//    options.AddPolicy(name: "Collaborator", configurePolicy: policy => policy.RequireRole("collaborator"));
//    options.AddPolicy(name: "Collaborator", configurePolicy: policy => policy.RequireRole("collaborator"));
//    options.AddPolicy(name: "Supplier", configurePolicy: policy => policy.RequireRole("supplier"));
//});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.Configure<MongoSettings>(settings =>
{
    settings.connectionString = "mongodb+srv://develop:develop@cluster0.rew0d.mongodb.net/myFirstDatabase?retryWrites=true&w=majority";
    settings.Database = "gsl";
});

builder.Services.Configure<ILoggerFactory>(loggerFactory => { loggerFactory.AddSerilog(); });

builder.Services.AddTransient<UnitOfWork, MongoUnitOfWork>();


builder.Services.AddTransient<OAuth>();
builder.Services.AddTransient<EmailService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<SerilogMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();

