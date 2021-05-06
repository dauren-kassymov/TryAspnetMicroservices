using System;
using System.Threading.Tasks;
using Discount.Grpc.Protos;
using Discount.Grpc.Repos;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepo _discountRepo;
        private readonly ILogger<DiscountService> _logger;

        public DiscountService(IDiscountRepo discountRepo, ILogger<DiscountService> logger)
        {
            _discountRepo = discountRepo ?? throw new ArgumentNullException(nameof(discountRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _discountRepo.GetDiscount(request.ProductName);
            if (coupon is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Discount with ProductName={request.ProductName} is not found."));
            }

            return _mapper.map<CouponModel>(coupon);
        }
    }
}