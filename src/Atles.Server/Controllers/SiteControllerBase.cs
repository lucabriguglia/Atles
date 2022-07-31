using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Core;
using Atles.Core.Commands;
using Atles.Core.Queries;
using Atles.Core.Utilities;
using Atles.Models.Public;
using Atles.Queries.Public;
using Atles.Server.Extensions;
using Atles.Server.Mapping;
using FluentValidation;

namespace Atles.Server.Controllers;

[ApiController]
public abstract class SiteControllerBase : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    protected SiteControllerBase(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    private CurrentUserModel _currentUser;
    protected CurrentUserModel CurrentUser
    {
        get
        {
            if (_currentUser == null)
            {
                var queryResult = AsyncUtil.RunSync(() => _dispatcher.Get(new GetCurrentUser()));
                _currentUser = queryResult.AsT0;
            }

            return _currentUser;
        }
    }

    private CurrentSiteModel _currentSite;
    protected CurrentSiteModel CurrentSite
    {
        get
        {
            if (_currentSite == null)
            {
                var queryResult = AsyncUtil.RunSync(() => _dispatcher.Get(new GetCurrentSite()));
                _currentSite = queryResult.AsT0;
            }

            return _currentSite;
        }
    }

    private IList<CurrentForumModel> _currentForums;
    protected IList<CurrentForumModel> CurrentForums
    {
        get
        {
            if (_currentForums == null)
            {
                var queryResult = AsyncUtil.RunSync(() => _dispatcher.Get(new GetCurrentForums()));
                _currentForums = queryResult.AsT0;
            }

            return _currentForums;
        }
    }

    protected async Task<ActionResult> ProcessPost<TModel, TCommand>(
        TModel model,
        IValidator<TModel> validator,
        IMapper<TModel, TCommand> mapper)
        where TModel : class
        where TCommand : ICommand
    {
        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            return validationResult.ToActionResult();
        }

        var command = mapper.Map(model, CurrentUser.Id);
        var commandResult = await _dispatcher.Send(command);

        return commandResult.Match(
            success => Ok(),
            failure => failure.ToActionResult()
        );
    }

    protected async Task<ActionResult> ProcessGet<TResult>(IQuery<TResult> query)
    {
        var queryResult = await _dispatcher.Get(query);

        return queryResult.Match(
            result => Ok(result),
            failure => failure.ToActionResult()
        );
    }
}
