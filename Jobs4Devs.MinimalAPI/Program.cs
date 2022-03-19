using Jobs4Devs.MinimalAPI.Data;
using Jobs4Devs.MinimalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MiniValidation;
using NetDevPack.Identity;
using NetDevPack.Identity.Jwt;
using NetDevPack.Identity.Model;

var builder = WebApplication.CreateBuilder(args);

#region Services Config

builder.Services.AddIdentityEntityFrameworkContextConfiguration(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("Jobs4Devs.MinimalAPI")));

builder.Services.AddDbContext<APIDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityConfiguration();
builder.Services.AddJwtConfiguration(builder.Configuration, "AppSettings");

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DeleteVacancy",
        policy => policy.RequireClaim("DeleteVacancy"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Jobs4Devs Minimal API Sample",
        Description = "Developed by Diego Romário",
        Contact = new OpenApiContact { Name = "Diego Romário", Email = "diego.romario@outlook.com" },
        License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter your token here like this: Bearer {your token}",
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

#endregion

#region Pipeline Config

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthConfiguration();

app.UseHttpsRedirection();

MapActions(app);

app.Run();

#endregion

#region End Points
void MapActions(WebApplication app)
{
    app.MapPost("/user", [AllowAnonymous] async (
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IOptions<AppJwtSettings> appJwtSettings,
        RegisterUser registerUser) =>
    {
        if (registerUser == null)
            return Results.BadRequest("User is required");

        if (!MiniValidator.TryValidate(registerUser, out var errors))
            return Results.ValidationProblem(errors);

        var user = new IdentityUser
        {
            UserName = registerUser.Email,
            Email = registerUser.Email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, registerUser.Password);

        if (!result.Succeeded)
            return Results.BadRequest(result.Errors);

        var jwt = new JwtBuilder()
                    .WithUserManager(userManager)
                    .WithJwtSettings(appJwtSettings.Value)
                    .WithEmail(user.Email)
                    .WithJwtClaims()
                    .WithUserClaims()
                    .WithUserRoles()
                    .BuildUserResponse();

        return Results.Ok(jwt);

    }).ProducesValidationProblem()
      .Produces(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status400BadRequest)
      .WithName("AddUser")
      .WithTags("User");

    app.MapPost("/login", [AllowAnonymous] async (
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IOptions<AppJwtSettings> appJwtSettings,
        LoginUser loginUser) =>
    {
        if (loginUser == null)
            return Results.BadRequest("User is required");

        if (!MiniValidator.TryValidate(loginUser, out var errors))
            return Results.ValidationProblem(errors);

        var result = await signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

        if (result.IsLockedOut)
            return Results.BadRequest("User blocked");

        if (!result.Succeeded)
            return Results.BadRequest("Invalid User or Password");

        var jwt = new JwtBuilder()
                    .WithUserManager(userManager)
                    .WithJwtSettings(appJwtSettings.Value)
                    .WithEmail(loginUser.Email)
                    .WithJwtClaims()
                    .WithUserClaims()
                    .WithUserRoles()
                    .BuildUserResponse();

        return Results.Ok(jwt);

    }).ProducesValidationProblem()
      .Produces(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status400BadRequest)
      .WithName("Login")
      .WithTags("User");

    app.MapGet("/vacancy", [AllowAnonymous] async (
        APIDBContext context) =>
        await context.Vacancies.ToListAsync())
        .WithName("GetVacancy")
        .WithTags("Vacancy");

    app.MapGet("/vacancy/{id}", [Authorize] async (
        Guid id,
        APIDBContext context) =>
        await context.Vacancies.FindAsync(id)
              is Vacancy vacancy
                  ? Results.Ok(vacancy)
                  : Results.NotFound())
        .Produces<Vacancy>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithName("GetVacancyById")
        .WithTags("Vacancy");

    app.MapPost("/vacancy", [Authorize] async (
        APIDBContext context,
        Vacancy vacancy) =>
    {
        if (!MiniValidator.TryValidate(vacancy, out var errors))
            return Results.ValidationProblem(errors);

        context.Vacancies.Add(vacancy);
        var result = await context.SaveChangesAsync();

        return result > 0
            //? Results.Created($"/vacancy/{vacancy.Id}", vacancy)
            ? Results.CreatedAtRoute("GetVacancyById", new { id = vacancy.Id }, vacancy)
            : Results.BadRequest("There was an error saving the record");

    }).ProducesValidationProblem()
    .Produces<Vacancy>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("AddVacancy")
    .WithTags("Vacancy");

    app.MapPut("/vacancy/{id}", [Authorize] async (
        Guid id,
        APIDBContext context,
        Vacancy vacancy) =>
    {
        var vacancyDB = await context.Vacancies.AsNoTracking<Vacancy>().FirstOrDefaultAsync(v => v.Id == id);
        if (vacancyDB == null) return Results.NotFound();

        if (!MiniValidator.TryValidate(vacancy, out var errors))
            return Results.ValidationProblem(errors);

        context.Vacancies.Update(vacancy);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("There was an error updating the record");

    }).ProducesValidationProblem()
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("UpdateVacancy")
    .WithTags("Vacancy");

    app.MapDelete("/vacancy/{id}", [Authorize] async (
        Guid id,
        APIDBContext context) =>
    {
        var vacancy = await context.Vacancies.FindAsync(id);
        if (vacancy == null) return Results.NotFound();

        context.Vacancies.Remove(vacancy);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("There was an error removing the record");

    }).Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .RequireAuthorization("DeleteVacancy")
    .WithName("DeleteVacancy")
    .WithTags("Vacancy");
}
#endregion