using ElMagzer.Core.Models.Identity;
using ElMagzer.Helpers;
using ElMagzer.Repository.Identity;
using Hangfire;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerServices();
builder.Services.AddDbContext<ElMagzerContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddDbContext<AppIdentityDbContext>(optionsBuilder =>
{
    optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHangfireServer();

builder.Services.AddApplicationServices();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddIdentityServices(builder.Configuration);
var app = builder.Build();

var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var dbcontext = services.GetRequiredService<ElMagzerContext>();
var Identitydbcontext = services.GetRequiredService<AppIdentityDbContext>();

var loggerFactory = services.GetRequiredService<ILoggerFactory>();
try
{

    await dbcontext.Database.MigrateAsync();
    //await ElMagzerContextSeed.SeedAsync(dbcontext);

    await Identitydbcontext.Database.MigrateAsync();

    var usermanger = services.GetRequiredService<UserManager<AppUser>>();
    await AppIdentityDbContextSeed.SeedUserAsync(usermanger);
}
catch (Exception ex)
{
    var logger = loggerFactory.CreateLogger<ElMagzerContext>();
    logger.LogError(ex, "an Error Occured during apply the Migrations");
}
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseWebSockets();
app.UseRouting();

app.UseHangfireDashboard("/dashborad");

app.UseCors("MyPolicy");

app.UseStaticFiles();

app.UseAuthorization();

app.MapHub<CowHub>("/cowHub");

app.MapControllers();

RecurringJob.AddOrUpdate<CowPiecesCleanupService>(
    "CleanupCowPieces",
    service => service.CleanupCowPiecesAsync(),
    Cron.Monthly());

app.Run();
