using PayOut_Aulac_FPT.Core.Interfaces.Repositories;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using PayOut_Aulac_FPT.Core.Services;
using PayOut_Aulac_FPT.Infrastructure.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.eShopWeb.PublicApi.Middleware;
using System.Reflection;
using System.Text.Json.Serialization;
using Minio.AspNetCore;
using PayOut_Aulac_FPT.Infrastructure.Services;
using PayOut_Aulac_FPT.Hub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IJWTManagerRepository, JWTManagerRepository>();
builder.Services.AddScoped<IMinioService, MinioService>();
builder.Services.AddScoped<IBase64Service, Base64Service>();
builder.Services.AddScoped<ISha256HexService, Sha256HexService>();
builder.Services.AddScoped<IFoxpayServiceAPI, FoxpayServiceAPI>();
builder.Services.AddScoped<IUserLoginRepository, UserLoginRepository>();
builder.Services.AddScoped<IUserLoginService, UserLoginService>();
builder.Services.AddScoped<IQRCodeRepository, QRCodeRepository>();
builder.Services.AddScoped<IQRCodeService, QRCodeService>();
builder.Services.AddScoped<ICompanyInfoRepository, CompanyInfoRepository>();
builder.Services.AddScoped<ICompanyInfoService, CompanyInfoService>();
builder.Services.AddScoped<IVchPaymentFoxpayRepository, VchPaymentFoxpayRepository>();
builder.Services.AddScoped<IVchPaymentFoxpayService, VchPaymentFoxpayService>();
builder.Services.AddScoped<IVchPaymentHISRepository, VchPaymentHISRepository>();
builder.Services.AddScoped<IVchPaymentHISService, VchPaymentHISService>();
//builder.Services.AddScoped<ICauHinhRepository, CauHinhRepository>();
//builder.Services.AddScoped<ICauHinhService, CauHinhService>();

builder.Services.AddSignalR();
const string CORS_POLICY = "CorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CORS_POLICY, builder =>
    {
        //builder.WithOrigins("").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        //builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
        //builder.SetIsOriginAllowed(origin => true);
    });
});

builder.Services.AddOptions();

builder.Services.AddTransient<IDbConnection>(db => new SqlConnection(
                    builder.Configuration.GetConnectionString("46187")));
//builder.Services.AddTransient<IDbConnection>(db => new SqlConnection(
                    //builder.Configuration.GetConnectionString("46004")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var Key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]);
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers(
    )
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSwaggerGen(swagger =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    swagger.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    // To Enable authorization using Swagger (JWT)
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(CORS_POLICY);

app.UseRouting();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
    endpoints.MapHub<SignalRService>("/qrcode");
});
app.MapControllers();

app.Run();
