using My1kWordsEe.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


var openApiKey = builder.Configuration["Secrets:OpenAiKey"];

if (string.IsNullOrWhiteSpace(openApiKey))
{
    throw new InvalidOperationException("OpenAI API key is missing");
}

builder.Services.AddSingleton(new OpenAiService(openApiKey));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Words/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Words}/{action=Index}/{id?}");

app.Run();
