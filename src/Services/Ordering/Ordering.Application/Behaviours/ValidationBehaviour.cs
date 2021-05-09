using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Ordering.Application.Behaviours
{
    public class ValidationBehaviour<TReq, TRes>: IPipelineBehavior<TReq, TRes>
    {
        public Task<TRes> Handle(TReq request, CancellationToken cancellationToken, RequestHandlerDelegate<TRes> next)
        {
            throw new System.NotImplementedException();
        }
    }
}