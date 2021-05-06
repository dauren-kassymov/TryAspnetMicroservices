using System;
using System.Threading.Tasks;
using Discount.API.Entities;
using Discount.API.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Discount.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepo _discountRepo;

        public DiscountController(IDiscountRepo discountRepo)
        {
            _discountRepo = discountRepo ?? throw new ArgumentNullException(nameof(discountRepo));
        }

        [HttpGet("{productName}", Name = "GetDiscount")]
        [ProducesResponseType(typeof(Coupon), StatusCodes.Status200OK)]
        public async Task<ActionResult<Coupon>> GetDiscount(string productName)
        {
            var coupon = await _discountRepo.GetDiscount(productName);
            return Ok(coupon);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Coupon), StatusCodes.Status200OK)]
        public async Task<ActionResult<Coupon>> CreateDiscount(Coupon coupon)
        {
            await _discountRepo.CreateDiscount(coupon);
            return CreatedAtRoute("GetDiscount", new {productName = coupon.ProductName}, coupon);
        }

        [HttpPut]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDiscount(Coupon coupon)
        {
            await _discountRepo.UpdateDiscount(coupon);
            return Ok();
        }

        [HttpDelete("{productName}", Name = "DeleteDiscount")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<ActionResult<Coupon>> DeleteDiscount(string productName)
        {
            await _discountRepo.DeleteDiscount(productName);
            return Ok();
        }
    }
}