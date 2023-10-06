using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherReviews.API.Contracts.Repositories;
using TeacherReviews.API.Middlewares;
using TeacherReviews.API.Repositories;
using TeacherReviews.API.Services;
using TeacherReviews.Data;
using TeacherReviews.Data.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options
            .UseLazyLoadingProxies()
            .UseNpgsql(builder.Configuration.GetConnectionString("DefaultDbConnection"));
    }
);

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
        new BadRequestObjectResult(
            new EntityValidationException(context.ModelState).Serialize()
        );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler(options =>
{
    options.ExceptionHandler = async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()!.Error;
        await ExceptionHandler.Handle(context, exception);
    };
});

builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddScoped<CityService>();
builder.Services.AddScoped<UniversityService>();
builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<UnitOfWork>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseExceptionHandler();

app.UseMiddleware<RequestTransactionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();