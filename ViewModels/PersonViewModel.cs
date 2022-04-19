namespace ContactMinimalAPI.ViewModels
{
    public class PersonViewModel
    {
        public PersonViewModel(Guid id, string name, List<ContactViewModel> contacts)
        {
            Id = id;
            Name = name;
            Contacts = contacts;
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<ContactViewModel>? Contacts { get; set; }
    }
}
