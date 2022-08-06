using System.Data;
using Atles.Commands.Sites;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain;
using Atles.Events.Sites;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Sites;

public class UpdateSiteHandler : ICommandHandler<UpdateSite>
{
    private readonly AtlesDbContext _dbContext;
    private readonly ICacheManager _cacheManager;

    public UpdateSiteHandler(AtlesDbContext dbContext, ICacheManager cacheManager)
    {
        _dbContext = dbContext;
        _cacheManager = cacheManager;
    }

    public async Task<CommandResult> Handle(UpdateSite command)
    {
        var site = await _dbContext.Sites
            .FirstOrDefaultAsync(x =>
                x.Id == command.SiteId);

        if (site == null)
        {
            throw new DataException($"Site with Id {command.SiteId} not found.");
        }

        site.UpdateDetails(command.Title,
            command.Theme,
            command.Css,
            command.Language,
            command.Privacy,
            command.Terms,
            command.HeadScript);

        var @event = new SiteUpdated
        {
            Title = site.Title,
            PublicTheme = site.PublicTheme,
            PublicCss = site.PublicCss,
            Language = site.Language,
            Privacy = site.Privacy,
            Terms = site.Terms,
            HeadScript = site.HeadScript,
            TargetId = site.Id,
            TargetType = nameof(Site),
            SiteId = command.SiteId,
            UserId = command.UserId
        };

        _dbContext.Events.Add(@event.ToDbEntity());

        await _dbContext.SaveChangesAsync();

        _cacheManager.Remove(CacheKeys.CurrentSite(site.Name));

        return new Success(new IEvent[] { @event });
    }
}
