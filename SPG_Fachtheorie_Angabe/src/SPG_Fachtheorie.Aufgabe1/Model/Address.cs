namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Address
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected Address() { }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public Address(string street, string city, string zip) { 
            Street = street;
            City = city;
            Zip = zip;
        }
        public int ID { get; set; }
        public string Street {get;  }
        public string City {get;  }
        public string Zip {get; }
    }
}