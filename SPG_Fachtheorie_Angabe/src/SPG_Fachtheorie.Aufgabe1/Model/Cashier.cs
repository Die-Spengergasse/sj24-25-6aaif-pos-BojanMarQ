namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Cashier : Employee
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected Cashier() {}
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public Cashier(string jobSpezialisation, string firstName, string lastName, Address address): base ()
        {
            JobSpezialisation = jobSpezialisation;
        }
        public string JobSpezialisation { get; set; }


    }
}