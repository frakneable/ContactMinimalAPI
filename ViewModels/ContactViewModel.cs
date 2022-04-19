using ContactMinimalAPI.Models;

namespace ContactMinimalAPI.ViewModels
{
    public class ContactViewModel
    {
        public ContactViewModel(Guid id, string value, ContactType type)
        {
            Id = id;
            Value = value;
            Type = type;
        }

        public Guid Id { get; set; }
        public string Value { get; set; }
        public ContactType Type { get; set; }
    }
}