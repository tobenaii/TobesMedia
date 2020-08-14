using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TobesMediaCore.Network;
using TobesMediaServer.MediaRequest;
using TobesMediaServer.Database;
using TobesMediaServer.NZBGet;
using TobesMediaServer.NZBManager;
using TobesMediaCore.MediaRequest;
using TobesMediaServer.MediaInfo;
using TobesMediaServer.OMDB;
using TobesMediaServer.MediaPipeline;
using TobesMediaServer.ffmpeg;
using TobesMediaServer.MediaInfo.API;
using TobesMediaServer.Indexer;
using TobesMediaServer.MediaInfo.OMDB;

namespace TobesMediaServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            services.AddSingleton<IMovieInfo, TmdbMovieInfo>();
            services.AddSingleton<IShowInfo, TmdbShowInfo>();
            services.AddSingleton<IAnimeInfo, TmdbAnimeInfo>();
            services.AddSingleton<IMediaDatabase, MySqlMediaDatabase>();
            services.AddSingleton<INzbManager, NZBgetManager>();

            services.AddTransient<IUsenetIndexer, NzbGeekIndexer>();

            services.AddTransient<IMediaService, NzbDownloadService>();
            services.AddTransient<IMediaService, FfmpegTranscodeService>();

            services.AddTransient<IMediaPipeline, MoviePipeline>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseCors("MyPolicy");
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
