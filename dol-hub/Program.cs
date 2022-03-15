using dol_hub;
using dol_hub.Services;
using FirebaseAdmin;
using AspNetCore.Firebase.Authentication.Extensions;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IPlayerService, PlayerService>();


builder.Configuration.AddJsonFile("appsettings.json");

var options = new AppOptions
{
    Credential = GoogleCredential.GetApplicationDefault(),
    ProjectId = builder.Configuration["ProjectId"]
};

var fApp = FirebaseApp.Create(options);
Console.WriteLine($"My app ID is {fApp.Options.ProjectId}!");

builder.Services.AddFirebaseAuthentication(builder.Configuration["FirebaseAuthentication:Issuer"],
    builder.Configuration["FirebaseAuthentication:Audience"]);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapHub<GameHub>("/hub"); });

app.Run();
