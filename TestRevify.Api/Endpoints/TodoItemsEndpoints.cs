
using Microsoft.AspNetCore.Http.HttpResults;

namespace TestRevify.Api.Endpoints;

public record Todo(int Id, string Title, DateOnly? DueBy = null, bool IsComplete = false);

public static class TodoItemsEndpoints
{
    public static Todo[] sampleTodos = [
            new(1, "Walk the dog"),
            new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
            new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
            new(4, "Clean the bathroom"),
            new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
];
    public static void RegisterTodoItemsEndpoints(this WebApplication app)
    {

        var todosApi = app.MapGroup("/todos");

        todosApi.WithTags("TestEndpoints");

        todosApi.MapGet("/",() => TypedResults.Ok(sampleTodos));
        todosApi.MapGet("/{id}", GetTodoByIdAsync);


        static Results<Ok<Todo>, NotFound> GetTodoByIdAsync(int id, CancellationToken ct)
        {
            return sampleTodos.FirstOrDefault(a => a.Id == id) is Todo todo
                ? TypedResults.Ok(todo)
                : TypedResults.NotFound();
        }
    }
}
