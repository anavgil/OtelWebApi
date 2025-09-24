using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TestRevify.Api.Facade;
using TestRevify.Api.Workers;

namespace TestRevify.Api.Endpoints;

public record Todo(int Id,string Title,DateOnly? DueBy=null,bool IsComplete=false);

public static class TodoItemsEndpoints
{
    public static void RegisterTodoItemsEndpoints(this WebApplication app)
    {
        Todo[] sampleTodos = [
            new(1, "Walk the dog"),
            new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
            new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
            new(4, "Clean the bathroom"),
            new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
];
        var todosApi = app.MapGroup("/todos");

        todosApi.MapGet("/", () => sampleTodos);
        todosApi.MapGet("/{id}", GetTodoByIdAsync);
        
        static Results<Ok<Todo>, NotFound> GetTodoByIdAsync(int id, CancellationToken ct)
        {
            Todo[] sampleTodos = [
            new(1, "Walk the dog"),
            new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
            new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
            new(4, "Clean the bathroom"),
            new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
            ];

            return (Results<Ok<Todo>, NotFound>)
                    (sampleTodos.FirstOrDefault(a => a.Id == id) is Todo todo
                           ? Results.Ok(todo)
                           : Results.NotFound());
        }

        //static async Task<Results<Ok<int>,error> CallNativeCodeAsync(int n1,int n2,CancellationToken ct)
        //{
        //    //call to C++ function
        //    try
        //    {
        //        var t = await Task.Run(() => RevifyFacade.Logger_testSuma(n1, n2));
        //        return Results.Ok(t);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
    }
}
