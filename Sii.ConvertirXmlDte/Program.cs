using Microsoft.Extensions.Azure;
using Sii.ConvertirXmlDte.Helper;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnection"]!);
});

builder.Services.AddSingleton<TemplateLoader>();

WebApplication app = builder.Build();

app.MapPost("/api/dte/genera-html", async (
    TemplateLoader loader,
    HttpContext http,
    CancellationToken token
) =>
{
    using StreamReader reader = new(http.Request.Body);
    string xml = await reader.ReadToEndAsync(token);

    string xsltContent = await loader.GetXsltTemplateAsync();
    XsltHelper helper = new(xsltContent);

    string html = helper.GenerateHtml(xml, token: token);
    return Results.Content(html, "text/html");
});

app.UseHttpsRedirection();
await app.RunAsync();
