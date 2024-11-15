using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TriggerJob;
using TriggerJob.Models;
using Quartz;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<TriggerAlert>();     // Register your Quartz Job

// Ensure this class is properly defined and implements IJob
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();  // Use DI for job creation

    // Create a trigger that fires every 5 seconds
    var jobKey = new JobKey("TriggerAlert", "group");

    q.AddJob<TriggerAlert>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey) // Link the job with this trigger
        .WithIdentity("myTrigger", "group")
        .StartNow()
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(30)  // Every 5 seconds
                                        //.WithIntervalInMinutes(1)

            .RepeatForever())          // Repeat forever
    );
});

// Add Quartz hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    // Here is the migration executed
    dbContext.Database.Migrate();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
