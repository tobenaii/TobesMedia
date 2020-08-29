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
using TobesMediaServer.MediaInfo.API;
using TobesMediaServer.Indexer;
using TobesMediaServer.MediaInfo.OMDB;
using TobesMediaServer.Transcoding;

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
        public async void ConfigureServices(IServiceCollection services)
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

            services.AddSingleton<IMediaPipelineDatabase, MySqlMediaDatabase>(x => new MySqlMediaDatabase("Pipeline"));
            services.AddSingleton<ILocalMediaDatabase, MySqlMediaDatabase>(x => new MySqlMediaDatabase("Local"));
            services.AddSingleton<IDownloadDatabase, MySqlMediaDatabase>(x => new MySqlMediaDatabase("Downloads"));
            services.AddSingleton<INzbManager, NZBgetManager>();
            services.AddSingleton<IPipelineData, PipelineData>();

            services.AddTransient<IUsenetIndexer, NzbGeekIndexer>();

            services.AddSingleton<IServiceLogger, ServiceLogger>();
            services.AddTransient<IMediaService, NzbDownloadService>();
            services.AddTransient<IMediaService, FfmpegTranscodeService>();

            services.AddTransient<IMediaPipeline, MoviePipeline>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMediaPipelineDatabase pipelineDB, IMovieInfo movieInfo)
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

            List<string> ids = await pipelineDB.GetAllIdsAsync();
            List<Task> pipes = new List<Task>();
            foreach (string id in ids)
            {
                var movie = await movieInfo.GetMediaByIDAsync(id);
                IMediaPipeline pipe = app.ApplicationServices.GetRequiredService<IMediaPipeline>();
                pipes.Add(pipe.ProcessMediaAsync(movie, true));
            }
            await Task.WhenAll(pipes);
        }
    }
}
