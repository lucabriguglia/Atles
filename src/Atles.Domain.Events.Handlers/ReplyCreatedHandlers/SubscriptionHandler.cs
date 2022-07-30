using System.Text.Encodings.Web;
using Atles.Core.Events;
using Atles.Core.Settings;
using Atles.Data;
using Atles.Events.Posts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Atles.Events.Handlers.ReplyCreatedHandlers
{
    public class SubscriptionHandler : IEventHandler<ReplyCreated>
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<SubscriptionHandler> _logger;
        private readonly DatabaseSettings _databaseSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubscriptionHandler(IEmailSender emailSender, ILogger<SubscriptionHandler> logger, IOptions<DatabaseSettings> databaseSettings, IHttpContextAccessor httpContextAccessor)
        {
            _emailSender = emailSender;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _databaseSettings = databaseSettings.Value;
        }

        public Task Handle(ReplyCreated @event)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseAddress = $"{request.Scheme}://{request.Host}";

            Thread thread = new(delegate ()
            {
                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder<AtlesDbContext>();
                    optionsBuilder.UseSqlServer(_databaseSettings.AtlesConnectionString);

                    using (var dbContext = new AtlesDbContext(optionsBuilder.Options))
                    {
                        var topic = dbContext.Posts.Include(x => x.Forum).FirstOrDefault(x => x.Id == @event.TopicId);

                        var subscriptions = dbContext.Subscriptions
                            .Include(x => x.User)
                            .Where(x => x.ItemId == @event.TopicId)
                            .Where(x => x.UserId != @event.UserId)
                            .ToList();

                        var callbackUrl = $"{baseAddress}/forum/{topic.Forum.Slug}/{topic.Slug}";

                        foreach (var subscription in subscriptions)
                        {
                            _emailSender.SendEmailAsync(
                                subscription.User.Email, $"[New Reply] {topic.Title}",
                                $"You can read the new reply by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.")
                                .ConfigureAwait(false);
                        }                        
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            });

            thread.Start();

            return Task.CompletedTask;
        }
    }
}
