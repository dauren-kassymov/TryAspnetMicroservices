using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(
            IOrderRepo orderRepo,
            IMapper mapper,
            ILogger<DeleteOrderCommandHandler> logger
        )
        {
            _orderRepo = orderRepo ?? throw new ArgumentNullException(nameof(orderRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepo.GetByIdAsync(request.Id);
            if (order is null)
            {
                _logger.LogWarning("Order {OrderId} not exists in db", request.Id);
                throw new NotFoundException(nameof(Order), request.Id);
            }

            await _orderRepo.DeleteAsync(order);
            
            return Unit.Value;
        }
    }
}