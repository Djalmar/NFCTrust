using System;
using System.Collections.Generic;
using System.Text;

namespace NFCTrust.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentificationNumber { get; set; }
        public string DriverRegistration { get; set; }
        public byte[] Picture { get; set; }
    }
}
