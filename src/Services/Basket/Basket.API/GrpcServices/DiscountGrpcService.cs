using System;
using System.Threading.Tasks;
using Discount.Grpc.Protos;

namespace Basket.API.GrpcServices
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoService;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountProtoService)
        {
            _discountProtoService =
                discountProtoService ?? throw new ArgumentNullException(nameof(discountProtoService));
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            var req = new GetDiscountRequest {ProductName = productName};
            var couponModel = await _discountProtoService.GetDiscountAsync(req);
            return couponModel;
        }
    }
}