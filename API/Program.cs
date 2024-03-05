using BussinessObject.Services.BillServices;
using BussinessObject.Services.CustomerServices;
using BussinessObject.Services.HouseServices;
using BussinessObject.Services.RoomServices;
using BussinessObject.Services.UserServices;
using BussinessObject.Services.VerifyServices;
using DataAccess.Entities;
using DataAccess.Repositories.BillRepository;
using DataAccess.Repositories.ContractRepository;
using DataAccess.Repositories.CustomerRepository;
using DataAccess.Repositories.HouseRepository;
using DataAccess.Repositories.OTPRepo;
using DataAccess.Repositories.RoomRepository;
using DataAccess.Repositories.UserRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Set up JWT Environment
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "MyShop.API",
        Description = "E-Store Shop"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. " +
                            "\n\nEnter your token in the text input below. " +
                              "\n\nExample: '12345abcde'",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Connect Database 
//builder.Services.AddDbContext<HouseManagementContext>(ServiceLifetime.Transient);
//builder.Services.AddDbContext<HouseManagementContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("HouseManagement")));
builder.Services.AddDbContext<HouseManagementContext>(option => option.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));


// Subcribe service
builder.Services.AddScoped<IUserSevices, UserSevices>();
builder.Services.AddScoped<IVerifyServices, VerifyServices>();
builder.Services.AddScoped<IHouseServices, HouseServices>();
builder.Services.AddScoped<IRoomServices, RoomServices>();
builder.Services.AddScoped<IBillServices, BillServices>();
builder.Services.AddScoped<ICustomerServices, CustomerServices>();


//Subcribe repository
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IOTPRepository, OTPRepository>();
builder.Services.AddTransient<IHouseRepository, HouseRepository>();
builder.Services.AddTransient<IRoomRepository, RoomRepository>();
builder.Services.AddTransient<IContractRepository, ContractRepository>();
builder.Services.AddTransient<IBillRepository, BillRepository>();
builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();



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
