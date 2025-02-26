using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using System;
using System.Linq;
using Xunit;

namespace SPG_Fachtheorie.Aufgabe1.Test
{
    [Collection("Sequential")]
    public class Aufgabe1Test
    {
        private AppointmentContext GetEmptyDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite(@"Data Source=cash.db")
                .Options;

            var db = new AppointmentContext(options);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            return db;
        }

        // Creates an empty DB in Debug\net8.0\cash.db
        [Fact]
        public void CreateDatabaseTest()
        {
            using var db = GetEmptyDbContext();
        }

        [Fact]
        public void AddCashierSuccessTest()
        {
            using var db = GetEmptyDbContext();
            var cashier = new Cashier("Getraenke", "Ben", "Jaffery", new Address("Spengergasse 5", "Wien", "1050"));
            db.Cashiers.Add(cashier);
            db.SaveChanges();
            Assert.True(db.Employees.First().RegistrationNumber != 0);
        }

        [Fact]
        public void AddPaymentSuccessTest()
        {
            using var db = GetEmptyDbContext();
            var cashdesk = new CashDesk(1);
            var cashier = new Cashier("Getraenke", "Ben", "Jaffery", new Address("Spengergasse 5", "Wien", "1050"));
            var payment = new Payment(cashdesk, new DateTime(2025, 02, 24), PaymentType.Maestro, cashier);
            db.Cashiers.Add(cashier);
            db.Payments.Add(payment);
            db.SaveChanges();
            Assert.True(db.Payments.First().ID != 0);
        }

        [Fact]
        public void EmployeeDiscriminatorSuccessTest()
        {
            using var db = GetEmptyDbContext();
            var cashier = new Cashier("Getraenke", "Ben", "Jaffery", new Address("Spengergasse 5", "Wien", "1050"));
            var manager = new Manager("Stellvertretung", "David", "Pavlov", new Address("Spengergasse 6", "Wien", "1050"));
            db.Cashiers.Add(cashier);
            db.Managers.Add(manager);
            db.SaveChanges();
            Assert.Equal("Cashier", cashier.Type.ToString());
            Assert.Equal("Manager", manager.Type.ToString());
        }
    }
}