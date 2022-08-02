using Microsoft.EntityFrameworkCore;
using MVC2.Interface;
using MVC2.Models;
using MVC2.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//added
builder.Services.AddDbContext<DatabaseContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseContext")));

//provides helpful error information in the development environment for EF migrations errors. //added
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// adding DI
builder.Services.AddScoped<IPagination, MockPagination>();
builder.Services.AddScoped<IStudent, StudentServices>();
builder.Services.AddScoped<IGrade, GradeServices>();
builder.Services.AddScoped<ISubject, SubjectServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else //added
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

//create db if not exists //added
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DatabaseContext>();
    //context.Database.EnsureCreated(); // for when db is rapidly created and deleted in testing phase
    DbInitializer.Initialize(context);
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
