using dol_hub;
using dol_hub.Services;
using AspNetCore.Firebase.Authentication.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IPlayerService, PlayerService>();

var app = builder.Build();

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddFirebaseAuthentication(builder.Configuration["FirebaseAuthentication:Issuer"],
    builder.Configuration["FirebaseAuthentication:Audience"]);

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("Admin", policy => policy.RequireClaim("Authority", "0"));
    option.AddPolicy("Testers", policy => policy.RequireClaim("Authority", "0", "1"));
    option.AddPolicy("Players", policy => policy.RequireClaim("Authority", "0", "1", "2"));
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapHub<GameHub>("/hub"); });

app.Run();
