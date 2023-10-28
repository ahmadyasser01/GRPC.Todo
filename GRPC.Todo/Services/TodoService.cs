using Grpc.Core;
using GRPC.Todo.Data;
using GRPC.Todo.Models;
using Microsoft.EntityFrameworkCore;

namespace GRPC.Todo.Services
{
    public class TodoService :TodoIt.TodoItBase
    {
        private AppDbContext _dbContext;

        public TodoService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<CreateTodoResponse> CreateTodo(CreateTodoRequest request, ServerCallContext context)
        {
            if(request.Title == String.Empty || request.Description == String.Empty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,"Invalid Todo"));
            }
            var todoItem = new TodoItem
            {
                Title = request.Title,
                Description = request.Description,
            };
            await _dbContext.AddAsync(todoItem);
            await _dbContext.SaveChangesAsync();
            return await Task.FromResult(new CreateTodoResponse
            {
                Id = todoItem.Id
            });
        }


        public override async Task<ReadTodoResponse> ReadTodo(ReadTodoRequest request, ServerCallContext context)
        {
            if (request.Id <=0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Todo Id"));
            }
            var todoItem = await _dbContext.TodoItems.FirstOrDefaultAsync(td => td.Id == request.Id);

            if (todoItem != null)
            {
                return await Task.FromResult(new ReadTodoResponse
                {
                    Id = todoItem.Id,
                    Title = todoItem.Title,
                    Description  = todoItem.Description,
                    Status = todoItem.Status
                });
            }
            throw new RpcException(new Status(StatusCode.NotFound, "Invalid Todo Id"));
        }


        public override async Task<ReadAllTodoResponse> ReadAllTodo(ReadAllTodoRequest request, ServerCallContext context)
        {
            var response = new ReadAllTodoResponse();
            var todoItems = await _dbContext.TodoItems.ToListAsync();

            foreach( var todoItem in todoItems)
            {
                response.ToDo.Add(new ReadTodoResponse
                {
                    Id = todoItem.Id,
                    Title = todoItem.Title,
                    Description = todoItem.Description,
                    Status = todoItem.Status
                });
            }
            return await Task.FromResult(response);
        }



        public override async Task<UpdateTodoResponse> UpdateTodo(UpdateTodoRequest request, ServerCallContext context)
        {
           if(request.Id <=0 || request.Title == String.Empty || request.Description == String.Empty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Todo"));
            }

            var todoItem = await _dbContext.TodoItems.FirstOrDefaultAsync(td => td.Id == request.Id);

            if (todoItem == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Invalid Todo Id"));

            todoItem.Title = request.Title;
            todoItem.Description = request.Description;
            todoItem.Status = request.Status;

            await _dbContext.SaveChangesAsync();
            return await Task.FromResult(new UpdateTodoResponse { Id = todoItem.Id});
        }

        public override async Task<DeleteTodoResponse> DeleteTodo(DeleteTodoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Todo"));
            }

            var todoItem = await _dbContext.TodoItems.FirstOrDefaultAsync(td => td.Id == request.Id);

            if (todoItem == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Invalid Todo Id"));

            _dbContext.Remove(todoItem);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new DeleteTodoResponse { Id = todoItem.Id });
        }
    }
}
