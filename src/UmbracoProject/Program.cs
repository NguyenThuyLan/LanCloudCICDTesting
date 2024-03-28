using Joonasw.AspNetCore.SecurityHeaders;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .Build();

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromSeconds(63072000);
    options.ExcludedHosts.Add("example.com");
    options.ExcludedHosts.Add("www.example.com");
});

WebApplication app = builder.Build();

await app.BootUmbracoAsync();

app.UseHttpsRedirection();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    await next();
});

app.UseCsp(csp =>
{
    csp.ByDefaultAllow
        .FromSelf()
        .From("packages.umbraco.org")
        .From("our.umbraco.org");
    csp.AllowScripts
        .FromSelf()
        .From("ajax.googleapis.com")
        .From("unpkg.com")
        .From("ajax.aspnetcdn.com")
        .From("cdnjs.cloudflare.com")
        .From("cdn.jsdelivr.net");
    csp.AllowStyles
        .FromSelf()
        .AllowUnsafeInline()
        .From("fonts.googleapis.com")
        .From("cdn.jsdelivr.net")
        .From("cdnjs.cloudflare.com")
        .From("cdn.linearicons.com");
    csp.AllowImages
        .FromSelf()
        .From("*.googleapis.com")
        .From("via.placeholder.com")
        .From("umbraco.com");
    csp.AllowFonts
        .FromSelf()
        .From("cdnjs.cloudflare.com")
        .From("fonts.gstatic.com")
        .From("cdn.linearicons.com");

    csp.AllowFraming.FromSelf();
    csp.OnSendingHeader = context =>
    {
        context.ShouldNotSend = context.HttpContext.Request.Path.StartsWithSegments("/umbraco");
        return Task.CompletedTask;
    };
});

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseInstallerEndpoints();
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
