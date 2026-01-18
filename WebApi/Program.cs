using CsvProcessor;
using WebApi.Middleware;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();

            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddAutoMapper(System.Reflection.Assembly.GetExecutingAssembly());

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {

            }

            app.UseMiddleware<ErrorHandlingMiddleware>();
            
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            app.Run();
        }
    }
}
