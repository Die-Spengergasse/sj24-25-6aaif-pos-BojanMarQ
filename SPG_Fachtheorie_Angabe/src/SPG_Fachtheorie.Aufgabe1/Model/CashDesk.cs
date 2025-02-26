using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class CashDesk
    {
        protected CashDesk() { }
        public CashDesk(int number)
        {
            Number = number;
        }
        [Key]
        public int Number { get; set; }
    }
}