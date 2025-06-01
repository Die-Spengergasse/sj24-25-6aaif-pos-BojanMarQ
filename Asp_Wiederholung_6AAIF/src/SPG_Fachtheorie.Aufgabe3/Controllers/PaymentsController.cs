
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe3.Commands;
using SPG_Fachtheorie.Aufgabe3.Dtos;

namespace SPG_Fachtheorie.Aufgabe3.Controllers;


[Route("api/payments/[controller]")]
[ApiController]

public class PaymentsController : ControllerBase
{

    [Route("api/[controller]")]  // --> /api/payments
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly AppointmentContext _db;

        public PaymentsController(AppointmentContext db)
        {
            _db = db;
        }
        private readonly AppDbContext _context;

        public PaymentsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /api/payments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<PaymentDto>> GetAllPayments(
            [FromQuery] int? cashDesk, [FromQuery] DateTime? dateFrom)
        {
            var payments = _db.Payments
                .Where(p =>
                    cashDesk.HasValue
                        ? p.CashDesk.Number == cashDesk.Value : true)
                .Where(p =>
                    dateFrom.HasValue
                        ? p.PaymentDateTime >= dateFrom.Value : true)
                .Select(p => new PaymentDto(
                    p.Id, p.Employee.FirstName, p.Employee.LastName,
                    p.PaymentDateTime,
                    p.CashDesk.Number, p.PaymentType.ToString(),
                    p.PaymentItems.Sum(p => p.Amount)))
                .ToList();
            return Ok(payments);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PaymentDetailDto> GetPaymentDetail(int id)
        {
            var payment = _db.Payments
                .Where(p => p.Id == id)
                .Select(p => new PaymentDetailDto(
                    p.Id, p.Employee.FirstName, p.Employee.LastName,
                    p.CashDesk.Number, p.PaymentType.ToString(),
                    p.PaymentItems
                        .Select(pi => new PaymentItemDto(
                            pi.ArticleName, pi.Amount, pi.Price))
                        .ToList()
                    ))
                .FirstOrDefault();
            if (payment is null)
                return NotFound();
            return Ok(payment);
        }

        /// <summary>
        /// POST /api/payments
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddPayment(NewPaymentCommand cmd)
        {
            var cashDesk = _db.CashDesks.FirstOrDefault(c => c.Number == cmd.CashDeskNumber);
            if (cashDesk is null) return Problem("Invalid cash desk", statusCode: 400);
            var employee = _db.Employees.FirstOrDefault(e => e.RegistrationNumber == cmd.EmployeeRegistrationNumber);
            if (employee is null) return Problem("Invalid employee", statusCode: 400);

            if (!Enum.TryParse<PaymentType>(cmd.PaymentType, out var paymentType))
                return Problem("Invalid payment type", statusCode: 400);

            var payment = new Payment(
                cashDesk, cmd.PaymentDateTime, employee, paymentType);
            _db.Payments.Add(payment);
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return Problem(e.InnerException?.Message ?? e.Message);
            }
            return CreatedAtAction(nameof(AddPayment), new { Id = payment.Id });
        }

        /// <summary>
        /// DELETE /api/payments/{id}?deleteItems=true|false
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeletePayment(int id, [FromQuery] bool deleteItems)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == id);
            if (payment is null) return NoContent();
            var paymentItems = _db.PaymentItems.Where(p => p.Payment.Id == id).ToList();
            if (paymentItems.Any() && deleteItems)
            {
                try
                {
                    _db.PaymentItems.RemoveRange(paymentItems);
                    _db.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    return Problem(e.InnerException?.Message ?? e.Message, statusCode: 400);
                }
                catch (InvalidOperationException e)
                {
                    return Problem(
                        e.InnerException?.Message ?? e.Message,
                        statusCode: StatusCodes.Status400BadRequest);
                }
            }
            try
            {
                _db.Payments.Remove(payment);
                _db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return Problem(e.InnerException?.Message ?? e.Message, statusCode: 400);
            }
            catch (InvalidOperationException e)
            {
                return Problem(
                    e.InnerException?.Message ?? e.Message,
                    statusCode: StatusCodes.Status400BadRequest);
            }
            return NoContent();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePayment(int id, UpdatePaymentCommand cmd)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == id);
            if (payment is null) return NotFound();

            var cashDesk = _db.CashDesks.FirstOrDefault(c => c.Number == cmd.CashDeskNumber);
            if (cashDesk is null) return Problem("Invalid cash desk", statusCode: 400);

            var employee = _db.Employees.FirstOrDefault(e => e.RegistrationNumber == cmd.EmployeeRegistrationNumber);
            if (employee is null) return Problem("Invalid employee", statusCode: 400);

            if (!Enum.TryParse<PaymentType>(cmd.PaymentType, out var paymentType))
                return Problem("Invalid payment type", statusCode: 400);

            if (paymentItem.LastUpdated != command.LastUpdated)
                return Problem("Payment item has changed", statusCode: 400);

            if(id != cmd.Id)
                return Problem("Payment ID mismatch", statusCode: 400);

            [HttpGet("{paymentId}")]
            public IActionResult GetPaymentById(Guid paymentId)
            {
                // Versuche, das Objekt mit der paymentId zu finden
                var payment = _paymentService.GetPaymentById(paymentId);

                if (payment == null)
                {
                    // Wenn das Objekt nicht gefunden wird, gib einen 400-Fehler zurück
                    return BadRequest(new { message = "Invalid payment ID" });
                }

                // Wenn das Objekt gefunden wird, gib es zurück
                return Ok(payment);
            }
            ublic class PaymentUpdateDto
        {
            public decimal Amount { get; set; }
            public string Description { get; set; }
        }

        [HttpPatch("{id}")]
        public IActionResult PatchConfirmed(Guid id, [FromBody] UpdateConfirmedCommand command)
        {
            // Validierung des Commands
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(command);
            if (!Validator.TryValidateObject(command, validationContext, validationResults, true))
            {
                return ValidationProblem(new ValidationProblemDetails(
                    validationResults.ToDictionary(
                        v => v.MemberNames.FirstOrDefault() ?? "Confirmed",
                        v => new[] { v.ErrorMessage ?? "Invalid value" }
                    )
                ));
            }

            // Payment suchen
            var payment = _paymentService.GetPaymentById(id);
            if (payment == null)
            {
                return Problem("Payment not found", statusCode: 404);
            }

            // Bereits bestätigt?
            if (payment.Confirmed != null)
            {
                return Problem("Payment already confirmed", statusCode: 400);
            }

            // Aktualisieren
            payment.Confirmed = command.Confirmed;
            _paymentService.UpdatePayment(payment);

            return NoContent();
        }
    }


        payment.CashDesk = cashDesk;
            payment.Employee = employee;
            payment.PaymentDateTime = cmd.PaymentDateTime;
            payment.PaymentType = paymentType;
            paymentItem.LastUpdated = DateTime.UtcNow;


            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return Problem(e.InnerException?.Message ?? e.Message, statusCode: 400);
            }
            return NoContent();
        }
    }
}