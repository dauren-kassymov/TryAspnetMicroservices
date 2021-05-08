using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public CheckoutOrderCommandHandler(
            IMapper mapper, 
            IOrderRepo orderRepo, 
            IEmailService emailService)
        {
            _mapper = mapper;
            _orderRepo = orderRepo;
            _emailService = emailService;
        }

        public Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            
        }
    }
}