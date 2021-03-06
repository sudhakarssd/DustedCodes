using DustedCodes.Blog.ViewModels.Blog;
using DustedCodes.Blog.ViewModels.Home;
using DustedCodes.Core.Data;
using DustedCodes.Core.Web;
using System.Collections.Generic;
using System.Linq;
using DustedCodes.Blog.Config;

namespace DustedCodes.Blog.ViewModels
{
    public sealed class ViewModelFactory : IViewModelFactory
    {
        private readonly IAppConfig _appConfig;
        private readonly IUrlGenerator _urlGenerator;
        private readonly IUrlEncoder _urlEncoder;

        public ViewModelFactory(IAppConfig appConfig, IUrlGenerator urlGenerator, IUrlEncoder urlEncoder)
        {
            _appConfig = appConfig;
            _urlGenerator = urlGenerator;
            _urlEncoder = urlEncoder;
        }

        private ArticleWrapper WrapArticleForView(Article article, bool renderTitleAsLink)
        {
            var permalinkUrl = _urlGenerator.GeneratePermalinkUrl(article.Id);
            var encodedPermalinkUrl = _urlEncoder.EncodeUrl(permalinkUrl);

            return new ArticleWrapper
            {
                Id = article.Id,
                Author = article.Author,
                Content = article.Content,
                Title = article.Title,
                PermalinkUrl = permalinkUrl,
                EditArticleUrl = string.Format(_appConfig.EditArticleUrlFormat, article.Id),
                UserFriendlyPublishDateTime = article.PublishDateTime.ToString(_appConfig.DateTimeFormat),
                ValidHtml5TPublishDateTime = article.PublishDateTime.ToString(_appConfig.HtmlDateTimeFormat),
                TwitterShareUrl = string.Format(_appConfig.TwitterShareUrlFormat, encodedPermalinkUrl, article.Title),
                GooglePlusShareUrl = string.Format(_appConfig.GooglePlusShareUrlFormat, encodedPermalinkUrl),
                FacebookShareUrl = string.Format(_appConfig.FacebookShareUrlFormat, encodedPermalinkUrl),
                YammerShareUrl = string.Format(_appConfig.YammerShareUrlFormat, encodedPermalinkUrl),
                LinkedInShareUrl = string.Format(_appConfig.LinkedInShareUrlFormat, encodedPermalinkUrl, article.Title),
                RedditShareUrl = string.Format(_appConfig.RedditShareUrlFormat, encodedPermalinkUrl, article.Title),
                WhatsAppShareUrl = string.Format(_appConfig.WhatsAppShareUrlFormat, article.Title, encodedPermalinkUrl),
                RenderTitleAsLink = renderTitleAsLink,
                HasTags = article.Tags != null && article.Tags.Any(),
                Tags = article.Tags
            };
        }

        public AboutViewModel CreateAboutViewModel()
        {
            return new AboutViewModel();
        }

        public ArticleViewModel CreateArticleViewModel(Article article)
        {
            var wrappedArticle = WrapArticleForView(article, false);

            return new ArticleViewModel(wrappedArticle);
        }

        public IndexViewModel CreateIndexViewModel(IEnumerable<Article> articles, int totalPageCount, int currentPage)
        {
            var wrappedArticles = articles.Select(a => WrapArticleForView(a, true));

            return new IndexViewModel
            {
                Articles = wrappedArticles,
                TotalPageCount = totalPageCount,
                CurrentPage = currentPage
            };
        }

        public ArchiveViewModel CreateArchiveViewModel(IEnumerable<Article> articles, string title)
        {
            return new ArchiveViewModel(title) { Articles = articles };
        }

        public TrendingViewModel CreateTrendingViewModel(IEnumerable<Article> articles)
        {
            var wrappedArticles = articles.Select(a => WrapArticleForView(a, true)).ToList();

            return new TrendingViewModel { Articles = wrappedArticles };
        }
    }
}