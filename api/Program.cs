using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/status/live", () => Results.Ok(new { status = "live" }));

app.MapGet("/status/ready", async (CancellationToken cancellationToken) =>
{
    try
    {
        await using var connection = new NpgsqlConnection(DatabaseOptions.ConnectionString());
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand("SELECT 1", connection);
        await command.ExecuteScalarAsync(cancellationToken);
        return Results.Ok(new { status = "ready", database = "ready" });
    }
    catch (Exception exception) when (exception is NpgsqlException or InvalidOperationException)
    {
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
});

app.MapGet("/api/recipes", async (CancellationToken cancellationToken) =>
{
    var recipes = new List<RecipeSummary>();
    await using var connection = new NpgsqlConnection(DatabaseOptions.ConnectionString());
    await connection.OpenAsync(cancellationToken);
    await using var command = new NpgsqlCommand(
        "SELECT id, title, description FROM recipes ORDER BY title", connection);
    await using var reader = await command.ExecuteReaderAsync(cancellationToken);
    while (await reader.ReadAsync(cancellationToken))
    {
        recipes.Add(new RecipeSummary(
            reader.GetGuid(0),
            reader.GetString(1),
            reader.IsDBNull(2) ? null : reader.GetString(2)));
    }

    return Results.Ok(recipes);
});

app.MapPost("/api/recipes", async (CreateRecipe request, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new { error = "Title is required." });
    }

    var id = Guid.NewGuid();
    await using var connection = new NpgsqlConnection(DatabaseOptions.ConnectionString());
    await connection.OpenAsync(cancellationToken);
    await using var command = new NpgsqlCommand(
        "INSERT INTO recipes (id, title, description) VALUES (@id, @title, @description)", connection);
    command.Parameters.AddWithValue("id", id);
    command.Parameters.AddWithValue("title", request.Title.Trim());
    command.Parameters.AddWithValue("description", (object?)request.Description?.Trim() ?? DBNull.Value);
    await command.ExecuteNonQueryAsync(cancellationToken);

    return Results.Created($"/api/recipes/{id}", new RecipeSummary(id, request.Title.Trim(), request.Description));
});

app.Run();

public sealed record CreateRecipe(string Title, string? Description);

public sealed record RecipeSummary(Guid Id, string Title, string? Description);

internal static class DatabaseOptions
{
    public static string ConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Required("POSTGRES_HOST"),
            Port = int.Parse(Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432"),
            Database = Required("POSTGRES_DB"),
            Username = Required("POSTGRES_USER"),
            Password = Required("POSTGRES_PASSWORD"),
            SslMode = SslMode.Disable,
        };

        return builder.ConnectionString;
    }

    private static string Required(string name) =>
        Environment.GetEnvironmentVariable(name)
        ?? throw new InvalidOperationException($"Required environment variable {name} is not set.");
}

public partial class Program;

