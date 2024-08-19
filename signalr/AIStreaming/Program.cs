using AIStreaming.Hubs;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using OpenAI;
using System.ClientModel;

namespace AIStreaming
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<OpenAIOptions>(builder.Configuration.GetSection("OpenAI"));
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            builder.Services.AddSingleton<GroupAccessor>()
                .AddSingleton<GroupHistoryStore>()
                .AddSingleton<OpenAIClient>(provider =>
                {
                    var options = provider.GetRequiredService<IOptions<OpenAIOptions>>().Value;
                    return new AzureOpenAIClient(new Uri(options.Endpoint), new ApiKeyCredential(options.Key));
                });

            var app = builder.Build();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.MapHub<GroupChatHub>("/groupChat");
            app.Run();
        }
    }
}
