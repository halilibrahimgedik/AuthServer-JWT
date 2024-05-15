using AuthServer.Core.Configuration;
using AuthServer.Core.Entities;
using AuthServer.Core.IUnitOfWork;
using AuthServer.Core.Repositories;
using AuthServer.Core.Service;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Configuration;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Options Pattern appSettingsJson dosyasýndaki belirli bir yapýlandýrmayý Class olarak atayýp daha sonra programda kullanmak istiyebiliriz.
builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));
builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOption"));


//! DI Register
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"), options =>
    {
        options.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).FullName);
    });
});


builder.Services.AddIdentity<UserApp, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


// Jwt validating settings
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,opt =>
{
    //appsettings.json'daki TokenOption 'ý okur ve CustomTokenOption sýnýfýna mapler ve geri döndürür
    var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOption>();

    // endpoint'imize gelen token da neyi kontrol etmek, doðrulamak istiyoruz ?
    opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidIssuer = tokenOptions.Issuer, // ValidIssuer = builder.Configuration.GetSection("TokenOption")["Issuer"],
        ValidAudience = tokenOptions.Audience[0],
        IssuerSigningKey = SignService.GetSymetricSecurityKey(tokenOptions.SecurityKey),

        // gelen token'da 4 durumun varlýðýný kontrol edelim
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero, // Default olarak Jwt'ye 5 dk süre eklenir serverlar arasý zaman tutarsýzlýðýndan dolayý. Bu yüzden bunu 0'a çektik
    };
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
