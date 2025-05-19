
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe3.Commands;
using SPG_Fachtheorie.Aufgabe3.Dtos;

namespace SPG_Fachtheorie.Aufgabe3.Controllers;


[Route("api/payments/[controller]")]
[ApiController]

public class PaymentsController : ControllerBase
{
    private readonly AppointmentContext _db;

    public PaymentsController(AppointmentContext db)
    {
        _db = db;
    }

    [HttpGet]
    public ActionResult<List<PaymentDto>> GetPayments([FromQuery] int? cashDesk, [FromQuery] DateTime? dateFrom)
    {
        /*
         * .Where(p => cashDesk.HasValue ? p.CashDesk.Number == cashDesk.Value : true)
         * .Where(p => dateFrom.HasValue ? p.PaymentDateTime >= dateFrom.Value : true)
         */
        return Ok(_db.Payments
            .Where(p => cashDesk.HasValue ? p.CashDesk.Number == cashDesk.Value : true)
            .Where(p => dateFrom.HasValue ? p.PaymentDateTime >= dateFrom.Value : true)
            .Select(e => new PaymentDto(
            e.Id,
            e.Employee.FirstName,
            e.Employee.LastName,
            e.CashDesk.Number,
            e.PaymentType.ToString(),
            e.PaymentItems.Select(s => s.Price).Sum()
        )).ToList());
    }

    [HttpGet("id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PaymentDto> GetPaymentById([FromQuery] int id)
    {
        var payment = _db.Payments
            .Where(e => e.Id == id)
            .Select(e => new PaymentDetailDto(
                e.Id,
                e.Employee.FirstName,
                e.Employee.LastName,
                e.CashDesk.Number,
                e.PaymentType.ToString(),
                e.PaymentItems.Select(p => new PaymentItemDto(
                    p.ArticleName,
                    p.Amount,
                    p.Price
                )).ToList()
            ))
            .AsNoTracking()
            .FirstOrDefault();
        if (payment is null)
        {
            return NotFound();
        }
        return Ok(payment);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<int> CreatePayment([FromBody] NewPaymentCommand paymentCmd)
    {
        CashDesk cashDesk = _db.CashDesks.FirstOrDefault(a => a.Number == paymentCmd.CashDeskNumber);
        Employee employee =
            _db.Employees.FirstOrDefault(e => e.RegistrationNumber == paymentCmd.EmployeeRegistrationNumber);
        var isPayment = Enum.TryParse<PaymentType>(paymentCmd.PaymentType, true, out PaymentType paymentType);
        if (cashDesk is null)
        {
            return BadRequest("Cash desk not found");
        }

        if (employee is null)
        {
            return BadRequest("Employee not found");
        }

        if (!isPayment)
        {
            return BadRequest("Invalid payment type");
        }

        var payment = new Payment(cashDesk, DateTime.Now, employee, Enum.Parse<PaymentType>(paymentCmd.PaymentType));
        try
        {
            _db.Payments.Add(payment);
            _db.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return CreatedAtAction(nameof(CreatePayment), new { id = payment.Id }, payment.Id);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    public ActionResult<int> DeletePayment(int id, [FromQuery] bool? deleteItems)
    {
        try
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == id);
            if (payment is null)
            {
                return NotFound();
            }

            if (deleteItems is true)
            {
                var paymentItems = _db.PaymentItems.Where(p => p.Payment.Id == id).ToList();
                _db.RemoveRange(paymentItems);
                _db.Remove(payment);
                _db.SaveChanges();
                return NoContent();
            }
            else
            {
                return BadRequest("Payment has payment items!");
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}