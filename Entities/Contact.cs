using System.ComponentModel.DataAnnotations;

namespace ContactMinimalAPI.Models
{
    public class Contact : BaseEntity
    {
        public Contact(ContactType type, string value)
        {
            Type = type;
            Value = value;
        }
        public Contact(ContactType type, string value, Person person)
        {
            Type = type;
            Value = value;
            Person = person;
        }
        public Contact(Guid id, ContactType type, string value)
        {
            Id = id;
            Type = type;
            Value = value;
        }

        [Required]
        public ContactType Type { get; private set; }

        [Required]
        public string? Value { get; private set; }

        public Person? Person { get; private set; }
    }
}