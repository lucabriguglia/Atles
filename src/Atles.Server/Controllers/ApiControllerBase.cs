using System.Threading.Tasks;
using Atles.Core;
using Atles.Core.Commands;
using Atles.Core.Queries;
using Atles.Queries.Public;
using Atles.Server.Extensions;
using Atles.Server.Mapping;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers;

[ApiController]
public abstract class ApiControllerBase : SiteControllerBase
{
    private readonly IDispatcher _dispatcher;

    protected ApiControllerBase(IDispatcher dispatcher) : base(dispatcher)
    {
        _dispatcher = dispatcher;
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

        var user = await CurrentUser();
        var command = mapper.Map(model, user.Id);
        var commandResult = await _dispatcher.Send(command);

        return commandResult.Match(
            success => Ok(),
            failure => failure.ToActionResult()
        );
    }

    protected async Task<ActionResult> ProcessGet<TResult>(IQuery<TResult> query)
    {
        var queryResult = await _dispatcher.Get(query);
        return Ok(queryResult);

        // TODO: Process query result
        //return queryResult.Match(
        //    result => Ok(result),
        //    failure => failure.ToActionResult()
        //);
    }
}