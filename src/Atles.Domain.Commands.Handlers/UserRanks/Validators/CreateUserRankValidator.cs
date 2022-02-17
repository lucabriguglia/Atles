using Atles.Core;
using Atles.Domain.Commands.UserRanks;
using FluentValidation;

namespace Atles.Domain.Commands.Handlers.UserRanks.Validators
{
    public class CreateUserRankValidator : AbstractValidator<CreateUserRank>
    {
        private readonly IDispatcher _dispatcher;

        public CreateUserRankValidator(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
    }
}
