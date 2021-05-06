using System;
using System.Threading.Tasks;
using AutoMapper;
using Discount.Grpc.Entities;
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
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepo discountRepo, ILogger<DiscountService> logger, IMapper mapper)
        {
            _discountRepo = discountRepo ?? throw new ArgumentNullException(nameof(discountRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _discountRepo.GetDiscount(request.ProductName);
            if (coupon is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Discount with ProductName={request.ProductName} is not found."));
            }

            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            var ok = await _discountRepo.CreateDiscount(coupon);
            if (!ok)
            {
                _logger.LogInformation("Could't create Discount. ProductName={ProductName}", coupon.ProductName);
                throw new RpcException(new Status(StatusCode.AlreadyExists, //dont know what status return if not ok
                    $"Could't create discount with ProductName={request.Coupon.ProductName}"));
            }

            _logger.LogInformation("Discount is successfully created. ProductName={ProductName}", coupon.ProductName);
            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);

            await _discountRepo.UpdateDiscount(coupon);
            _logger.LogInformation("Discount is successfully updated. ProductName={ProductName}", coupon.ProductName);

            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request,
            ServerCallContext context)
        {
            var deleted = await _discountRepo.DeleteDiscount(request.ProductName);
            var resp = new DeleteDiscountResponse
            {
                Success = deleted
            };
            return resp;
        }
    }
}