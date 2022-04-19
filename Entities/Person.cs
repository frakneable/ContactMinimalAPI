using System.ComponentModel.DataAnnotations;

namespace ContactMinimalAPI.Models
{
    public class Person : BaseEntity
    {
        public Person(string? name)
        {
            Name = name;
        }

        public Person(Guid id, string? name)
        {
            Id = id;
            Name = name;
        }

        [Required]
        public string? Name { get; private set; }

        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();

        internal void SetContact(Contact contact)
        {
            Contacts.Add(contact);
        }
    }
}
