using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using ValidationException = Ordering.Application.Exceptions.ValidationException;

namespace Ordering.Application.Behaviours
{
    public class ValidationBehaviour<TReq, TRes>: IPipelineBehavior<TReq, TRes>
    {
        private readonly IEnumerable<IValidator<TReq>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TReq>> validators)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }

        public async Task<TRes> Handle(TReq request, CancellationToken cancellationToken, RequestHandlerDelegate<TRes> next)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TReq>(request);
                var validationResults = await Task.WhenAll(_validators
                    .Select(x => x.ValidateAsync(context, cancellationToken))
                );
                var failures = validationResults
                    .SelectMany(x => x.Errors)
                    .Where(x => x != null)
                    .ToList();
                if (failures.Any())
                {
                    throw new ValidationException(failures);
                }
            }

            return await next();
        }
    }
}