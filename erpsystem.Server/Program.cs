using Microsoft.EntityFrameworkCore;
using erpsystem.Server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Dodaj us�ug� CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:55733")  // Dodaj adres, na kt�rym dzia�a frontend
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Dodaj us�ug� bazy danych
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dodaj konfiguracj� autentykacji JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ClockSkew = TimeSpan.Zero
    };
});

// Dodaj inne us�ugi
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// U�ywaj CORS z utworzon� polityk�
app.UseCors("AllowFrontend");

// Skonfiguruj pipeline HTTP
app.UseDefaultFiles();
app.MapStaticAssets();

// U�yj OpenAPI w �rodowisku deweloperskim
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();  // Zapewnienie HTTPS
app.UseAuthorization();  // Autoryzacja
app.MapControllers();  // Mapowanie kontroler�w

app.MapFallbackToFile("/index.html");  // Fallback do SPA (Single Page Application)

app.Run();
