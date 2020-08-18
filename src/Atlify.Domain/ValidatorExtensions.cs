using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace Atlify.Domain
{
    public static class ValidatorExtensions
    {
        public static async Task ValidateCommandAsync<TCommand>(this IValidator<TCommand> validator, TCommand command)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid)
                throw new ApplicationException(BuildErrorMessage(validationResult.Errors));
        }

        private static string BuildErrorMessage(IEnumerable<ValidationFailure> errors)
        {
            var errorsText = errors.Select(x => $"\r\n - {x.ErrorMessage}").ToArray();
            return $"Validation failed: {string.Join("", errorsText)}";
        }

    }
}
