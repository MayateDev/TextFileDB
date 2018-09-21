using System;
using System.Collections.Generic;
using TextDbLibrary.Interfaces;

namespace DataLibrary.Entities
{
    public class Person : IPrimaryString
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string CellphoneNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public List<Prize> Prize { get; set; }
    }
}
