using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace Ordering.Application.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public Dictionary<string,string[]> Errors { get; }

        public ValidationException() : base("One or more validation failures have occured.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            foreach (var g in failures.GroupBy(x => x.PropertyName, x => x.ErrorMessage))
            {
                Errors.Add(g.Key, g.ToArray());
            }
        }
    }
}