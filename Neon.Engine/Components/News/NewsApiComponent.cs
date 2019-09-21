using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;

namespace Neon.Engine.Components.News
{
	[NeonComponent("news_api", "v1.0.0.0", "NEWS", typeof(NewsApiConfig))]
	public class NewsApiComponent : AbstractNeonComponent<NewsApiConfig>
	{
		private NewsApiClient _newsApiClient;
		private Languages _newsLanguage;

		public NewsApiComponent(ILoggerFactory loggerFactory, IIoTService ioTService, IComponentsService componentsService) : base(loggerFactory, ioTService, componentsService)
		{

		}


		public override Task<bool> Start()
		{

			if (Config.ApiKey == "change_me")
				ThrowComponentNeedConfiguration("ApiKey");

			_newsApiClient = new NewsApiClient(Config.ApiKey);

			Enum.TryParse(Config.Language.ToUpper(), out _newsLanguage);

			return base.Start();
		}


		[ComponentPollRate(300)]
		public override async Task Poll()
		{
			var news = await _newsApiClient.GetTopHeadlinesAsync(new TopHeadlinesRequest()
			{
				PageSize = Config.PageSize,
				Sources = Config.Sources.Count > 0 ? Config.Sources : null,
				Language = _newsLanguage
			});

			if (news.Status == Statuses.Ok)
			{
				
			}

		}

	}
}
