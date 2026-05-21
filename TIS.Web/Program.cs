using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using QuestPDF.Infrastructure;
using Serilog;
using TIS.Data.Infrastructure;
using TIS.Data.Repositories;
using TIS.Data.Services;
using TIS.Web.Auth;

QuestPDF.Settings.License = LicenseType.Community;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) =>
        lc.ReadFrom.Configuration(ctx.Configuration));

    // ── Authentication: Windows AD (Negotiate) ──────────────────────────
    builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
        .AddNegotiate();

    builder.Services.AddScoped<IClaimsTransformation, AdRoleClaimsTransformer>();

    // ── Authorization policies ──────────────────────────────────────────
    builder.Services.AddAuthorization(opts =>
    {
        opts.FallbackPolicy = opts.DefaultPolicy; // all routes require auth by default

        opts.AddPolicy("SuperAdmin", p => p.RequireRole("SuperAdmin"));
        opts.AddPolicy("Admin", p => p.RequireRole("Administrator", "SuperAdmin"));
        opts.AddPolicy("Finance", p => p.RequireRole("Finance", "SuperAdmin"));
        opts.AddPolicy("Manager", p => p.RequireRole("LineManager", "Administrator", "SuperAdmin"));
        opts.AddPolicy("AdminOrFinance", p => p.RequireRole("Administrator", "Finance", "SuperAdmin"));
    });

    // ── Data layer ──────────────────────────────────────────────────────
    var connStr = builder.Configuration.GetConnectionString("TIS")
        ?? throw new InvalidOperationException("Connection string 'TIS' is missing from appsettings.json");

    builder.Services.AddScoped<ISpRunner>(_ => new SpRunner(connStr));
    builder.Services.AddScoped<IAdminRepository, AdminRepository>();
    builder.Services.AddScoped<ISettingRepository, SettingRepository>();
    builder.Services.AddScoped<IImportRepository>(sp => new ImportRepository(sp.GetRequiredService<ISpRunner>(), connStr));
    builder.Services.AddScoped<ImportService>(sp => new ImportService(connStr, sp.GetRequiredService<IImportRepository>()));
    builder.Services.AddScoped<IBillRepository>(sp => new BillRepository(sp.GetRequiredService<ISpRunner>(), connStr));
    builder.Services.AddScoped<IBillReportRepository>(sp => new BillReportRepository(sp.GetRequiredService<ISpRunner>()));
    builder.Services.AddScoped<IDashboardRepository>(sp => new DashboardRepository(sp.GetRequiredService<ISpRunner>()));
    builder.Services.AddScoped<IPivotRepository>(sp => new PivotRepository(sp.GetRequiredService<ISpRunner>()));
    builder.Services.AddScoped<IEmailSmsRepository>(sp => new EmailSmsRepository(sp.GetRequiredService<ISpRunner>()));
    builder.Services.AddScoped<ISapRepository>(sp => new SapRepository(sp.GetRequiredService<ISpRunner>()));
    builder.Services.AddScoped<IAuditRepository>(_ => new AuditRepository(connStr));
    builder.Services.AddScoped<IMarsaApiRepository>(sp => new MarsaApiRepository(sp.GetRequiredService<ISpRunner>()));
    builder.Services.AddScoped<EmailService>();
    builder.Services.AddScoped<SmsService>();
    builder.Services.AddHttpClient();

    // ── MVC ─────────────────────────────────────────────────────────────
    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Admin}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "TIS application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
