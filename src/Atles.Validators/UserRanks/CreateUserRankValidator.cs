using Atles.Commands.UserRanks;
using Atles.Core;
using FluentValidation;

namespace Atles.Validators.UserRanks
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
