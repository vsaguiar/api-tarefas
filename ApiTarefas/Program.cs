using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registando o contexto como um serviço no container de injeção de dependência
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("TarefasDB"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


#region Definições de mapeamento

app.MapGet("/", () => "Olá, mundo! ");

app.MapGet("/tarefas", async (AppDbContext db) => { return await db.Tarefas.ToListAsync(); });

app.MapPost("/tarefas", async (Tarefa tarefa, AppDbContext db) =>
{
    db.Tarefas.Add(tarefa);
    await db.SaveChangesAsync();
    return Results.Created($"/tarefas/{tarefa.Id}", tarefa);
});

app.MapGet("/tarefas/{id}", async (int id, AppDbContext db) =>
    await db.Tarefas.FindAsync(id) is Tarefa tarefa ? Results.Ok(tarefa) : Results.NotFound());

app.MapGet("/tarefas/concluida", async (AppDbContext db) => { return await db.Tarefas.Where(tarefa => tarefa.IsConcluida).ToListAsync(); });

#endregion

app.Run();


// Modelo de domínio
class Tarefa
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public bool IsConcluida { get; set; }
}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
}