using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Controllers.V1
{
    [ApiController]
    [Route("api/bill")]
    public class BillController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BillController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method to generate the bill payment and clean the bad payment history if its possible
        /// </summary>
        /// <param name="billPaymentDTO">Object with the bill to pay</param>
        /// <returns></returns>
        [HttpPost(Name = "payBill")]
        public async Task<ActionResult> Post(BillPaymentDTO billPaymentDTO)
        {
            var billDB = await _context.Bill
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == billPaymentDTO.BillId);

            if(billDB == null)
            {
                return NotFound();
            }

            if(billDB.Paid)
            {
                return BadRequest("La factura ya fue pagada");
            }

            //Here its possible to writte logic to make the payment

            //We are going to pretend that the payment was succesful

            billDB.Paid = true;
            await _context.SaveChangesAsync();

            var IsPaymentPending = await _context.Bill
                .AnyAsync(
                    b => b.UserId == billDB.UserId && !b.Paid && b.PaymentDate < DateTime.Today
                );

            if(!IsPaymentPending)
            {
                billDB.User.BadPaymentHistory = false;
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
