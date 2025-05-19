using SPG_Fachtheorie.Aufgabe1.Model;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe3.Commands
{

    public record NewPaymentCommand
    (
        [Range(1, int.MaxValue, ErrorMessage = "Invalid cash desk number")]
    int CashDeskNumber,
        DateTime PaymentDateTime,
        [StringLength(255, MinimumLength = 1, ErrorMessage = "invalid payment type")]
    string PaymentType,
        [Range(1, int.MaxValue, ErrorMessage = "Invalid employee registration number")]
    int EmployeeRegistrationNumber);
    

        
}
