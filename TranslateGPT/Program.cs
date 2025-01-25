var builder = WebApplication.CreateBuilder(args);

// Register MongoDB settings
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

// Register MongoDB service
builder.Services.AddSingleton<LanguageService>();

// Register services
builder.Services.AddHttpClient<OpenAIClient>(); // Register OpenAIClient for HTTP calls
builder.Services.AddSingleton<ApiClientFactory>(); // Register ApiClientFactory as a singleton
builder.Services.AddSingleton<IApiClient, OpenAIClient>();

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add HttpClient
builder.Services.AddHttpClient();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
