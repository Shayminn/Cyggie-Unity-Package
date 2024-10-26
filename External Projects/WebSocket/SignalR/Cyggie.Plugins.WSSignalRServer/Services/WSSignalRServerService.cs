using Cyggie.Plugins.Logs;
using Cyggie.Plugins.Services.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cyggie.Plugins.SignalR.Services
{
    /// <summary>
    /// Service for hosting a WebSocket server using SignalR
    /// </summary>
    public class WSSignalRServerService : Service
    {
        private WebApplication App => _app ??= BuildWithUrls("http://localhost:5000");
        private WebApplication? _app = null;

        private List<string> _routes = new List<string>();

        /// <summary>
        /// Set origins url and port for the server
        /// </summary>
        /// <param name="urls">Array of urls for webhost</param>
        public WebApplication BuildWithUrls(params string[] urls)
        {
            // Based on https://learn.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-8.0
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            foreach (string url in urls)
            {
                builder.WebHost.UseUrls(url);
            }

            // Add services to the container.
            builder.Services.AddSignalR();

            _app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!_app.Environment.IsDevelopment())
            {
                _app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                _app.UseHsts();
            }

            _app.UseHttpsRedirection();
            _app.UseStaticFiles();

            _app.UseRouting();

            _app.UseAuthorization();
            return _app;
        }

        /// <summary>
        /// Register a hub type to the server <br/>
        /// The path is 
        /// </summary>
        /// <typeparam name="T">Hub type</typeparam>
        /// <param name="path">Hub route path (Type name if empty)</param>
        public void RegisterHub<T>(string path = "") where T : Hub
        {
            if (string.IsNullOrEmpty(path))
            {
                path = $"/{typeof(T).Name}";
            }
            else if (!path.StartsWith("/"))
            {
                path = path.Insert(0, "/");
            }

            if (_routes.Contains(path))
            {
                Log.Error($"Failed to add hub type {typeof(T)}, path already added. Use SignalRHubPathAttribute to specify one.", nameof(WSSignalRServerService));
                return;
            }

            App.MapHub<T>(path);
            _routes.Add(path);

            Log.Debug($"Mapped hub path: {path}", nameof(WSSignalRServerService));
        }

        /// <summary>
        /// Start the websocket server, it will then start accepting clients, and block the thread until host shutdown
        /// </summary>
        public void StartServer()
        {
            if (_routes.Count == 0)
            {
                Log.Error($"Failed to start server, no hub/route was found. Use AddHub to add one.", nameof(WSSignalRServerService));
                return;
            }

            App.Run();
        }
    }
}
