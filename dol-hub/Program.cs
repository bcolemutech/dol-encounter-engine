using dol_hub;
using dol_hub.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IPlayerService, PlayerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapHub<GameHub>("/game"); });

app.Run();
