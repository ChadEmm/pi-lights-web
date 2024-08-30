namespace XmasLightControlApp.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();


            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.MapGet("/lights", (HttpContext httpContext, IWebHostEnvironment env) =>
            {
                var path = Path.Combine(env.WebRootPath, "lights.txt");
                var info = File.GetLastWriteTimeUtc(path);
                if (httpContext.Request.Headers["If-Modified-Since"].Any())
                {
                    var since = DateTime.Parse(httpContext.Request.Headers["If-Modified-Since"].ToString());
                    if (since >= info)
                    {
                        return Results.StatusCode(StatusCodes.Status304NotModified);
                    }
                }

                return Results.File(path, lastModified: DateTime.UtcNow);
            });

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
