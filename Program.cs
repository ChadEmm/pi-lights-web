using System.Diagnostics.Eventing.Reader;

namespace XmasLightControlApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.MapGet("/lights", (HttpContext httpContext, IWebHostEnvironment env) =>
            {
                var path = env.WebRootPath ?? env.ContentRootPath;
                var filename = Path.Join(path, "lights.txt");
                var info = File.GetLastWriteTimeUtc(filename);
                if (httpContext.Request.Headers["If-Modified-Since"].Any())
                {
                    var since = DateTime.Parse(httpContext.Request.Headers["If-Modified-Since"].ToString());
                    if (since >= info)
                    {
                        return Results.StatusCode(StatusCodes.Status304NotModified);
                    }
                }

                return Results.File(filename, lastModified: info);

            });

            app.Run();
        }
    }
}
