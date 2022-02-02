using Atles.Core.Events;
using Atles.Core.Settings;
using Atles.Data;
using Atles.Domain.Events.Posts;
using Atles.Domain.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Atles.Domain.Events.Handlers.ReplyCreatedHandlers
{
    public class SubscriptionHandler : IEventHandler<ReplyCreated>
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<SubscriptionHandler> _logger;
        private readonly DatabaseSettings _databaseSettings;

        public SubscriptionHandler(IEmailSender emailSender, ILogger<SubscriptionHandler> logger, IOptions<DatabaseSettings> databaseSettings)
        {
            _emailSender = emailSender;
            _logger = logger;
            _databaseSettings = databaseSettings.Value;
        }

        public async Task Handle(ReplyCreated @event)
        {
            await Task.CompletedTask;

            Thread thread = new(delegate ()
            {
                Thread.Sleep(10000);

                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder<AtlesDbContext>();
                    optionsBuilder.UseSqlServer(_databaseSettings.AtlesConnectionString);

                    using (var dbContext = new AtlesDbContext(optionsBuilder.Options))
                    {
                        var subscriptions = dbContext.Subscriptions
                            .Include(x => x.User)
                            .Where(x => x.ItemId == @event.TopicId)
                            .ToList();

                        foreach (var subscription in subscriptions)
                        {
                            _emailSender.SendEmailAsync(subscription.User.Email, "Reply", "Reply").ConfigureAwait(false);
                        }                        
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            });

            thread.Start();
        }
    }
}
