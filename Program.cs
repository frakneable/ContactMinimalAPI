using ContactMinimalAPI.Data;
using ContactMinimalAPI.Models;
using ContactMinimalAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using MiniValidation;

var builder = WebApplication.CreateBuilder(args);

#region Configure Services

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

#endregion

#region Configure Pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("*"));

app.UseHttpsRedirection();

MapActionsPeople(app);
MapActionsContacts(app);

app.Run();

#endregion

#region Actions

void MapActionsPeople(WebApplication app)
{
    app.MapGet("/people", async (
        AppDbContext context) =>
    {
        return await context.People
        .Select(p =>
            new PersonViewModel(p.Id, p.Name, p.Contacts.Select(c =>
                new ContactViewModel(c.Id, c.Value, c.Type)).ToList()))
        .ToListAsync();
    })
        .Produces<PersonViewModel>(StatusCodes.Status200OK)
        .WithName("GetPeople")
        .WithTags("Person");

    app.MapGet("/person/{id}", async (
        Guid id, AppDbContext context) =>
    {
        var person = await context.People.Include(p => p.Contacts)
        .FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
            return Results.NotFound();

        return Results.Ok(new PersonViewModel(person.Id, person.Name, person.Contacts
            .Select(c => new ContactViewModel(c.Id, c.Value, c.Type)).ToList()));
    })
        .Produces<PersonViewModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithName("GetPersonById")
        .WithTags("Person");

    app.MapPost("/person", async (
        AppDbContext context,
        PersonViewModel person) =>
    {
        if (!MiniValidator.TryValidate(person, out var errors))
            return Results.ValidationProblem(errors);

        var newPerson = new Person(person.Name);
        context.People.Add(newPerson);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.CreatedAtRoute("GetPersonById", new { id = newPerson.Id }, newPerson)
            : Results.BadRequest("It was not possible to create the person. Some required parameter is missing.");
    })
        .ProducesValidationProblem()
        .Produces<PersonViewModel>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .WithName("PostPerson")
        .WithTags("Person");

    app.MapPut("/person/{id}", async (
        Guid id,
        AppDbContext context,
        PersonViewModel person) =>
    {
        var existentPerson = await context.People.AsNoTracking<Person>()
                                                 .FirstOrDefaultAsync(p => p.Id == id);

        if (existentPerson is null) return Results.NotFound();

        var updatedPerson = new Person(id, person.Name);

        if (!MiniValidator.TryValidate(updatedPerson, out var errors))
            return Results.ValidationProblem(errors);

        context.People.Update(updatedPerson);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("It was not possible to update the person");
    })
        .ProducesValidationProblem()
        .Produces<PersonViewModel>(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .WithName("PutPerson")
        .WithTags("Person");

    app.MapDelete("/person/{id}", async (
        Guid id,
        AppDbContext context) =>
    {
        var existentPerson = await context.People.FindAsync(id);
        if (existentPerson is null) return Results.NotFound();

        context.People.Remove(existentPerson);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("It was not possible to delete the person");
    })
        .ProducesValidationProblem()
        .Produces<PersonViewModel>(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .WithName("DeletePerson")
        .WithTags("Person");
}

void MapActionsContacts(WebApplication app)
{
    app.MapGet("/contact/{id}", async (
        Guid id, AppDbContext context) =>
    {
        var contact = await context.Contacts
        .FirstOrDefaultAsync(c => c.Id == id);

        if (contact == null)
            return Results.NotFound();

        return Results.Ok(new ContactViewModel(contact.Id, contact.Value, contact.Type));
    })
    .Produces<ContactViewModel>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetContactById")
    .WithTags("Contact");

    app.MapPost("/person/{personId}/contact", async (
        AppDbContext context,
        Guid personId,
        ContactViewModel contact) =>
    {
        var person = await context.People.FindAsync(personId);
        if (person is null) return Results.BadRequest();

        var newContact = new Contact(contact.Type, contact.Value, person);
        context.Contacts.Add(newContact);

        var result = await context.SaveChangesAsync();

        return Results.CreatedAtRoute("GetContactById", new { id = newContact.Id }, result);
    })
    .Produces<ContactViewModel>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PostContact")
    .WithTags("Contact");

    app.MapPut("/contact/{id}", async (
        AppDbContext context,
        Guid id,
        ContactViewModel contact) =>
    {
        var existentContact = await context.Contacts.AsNoTracking<Contact>()
                                                    .FirstOrDefaultAsync(c => c.Id == id);

        if (existentContact is null) return Results.NotFound();

        var updatedContact = new Contact(id, contact.Type, contact.Value);

        if (!MiniValidator.TryValidate(updatedContact, out var errors))
            return Results.ValidationProblem(errors);

        context.Contacts.Update(updatedContact);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("It was not possible to update the contact");
    })
    .Produces<ContactViewModel>(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PutContact")
    .WithTags("Contact");

    app.MapDelete("/contact/{id}", async (
        Guid id,
        AppDbContext context) =>
    {
        var contact = await context.Contacts.FindAsync(id);
        if (contact is null) return Results.NotFound();

        context.Contacts.Remove(contact);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.Ok(result)
            : Results.BadRequest("Error! A problem has ocurred when deleting the contact.");
    })
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("DeleteContact")
    .WithTags("Contact"); ;
}

#endregion
