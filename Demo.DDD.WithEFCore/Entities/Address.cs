using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Demo.DDD.WithEFCore.Entities
{
    /******************************************************************
     * Original Source: https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities
     * 
     * EF Core allows you to model entity types that can only ever appear 
     * on navigation properties of other entity types. These are called 
     * owned entity types. The entity containing an owned entity type is 
     * its owner. Owned entities are essentially a part of the owner and 
     * cannot exist without it, they are conceptually similar to aggregates. 
     * This means that the owned entity is by definition on the dependent 
     * side of the relationship with the owner.
     * 
     * Use the OwnsOne method in OnModelCreating or annotate the 
     * type with OwnedAttribute to configure the type as an owned type.
     * 
     * 
     * Owned types configured with OwnsOne or discovered through a reference 
     * navigation always have a one-to-one relationship with the owner, 
     * therefore they don't need their own key values as the foreign key values 
     * are unique. In the previous example, the StreetAddress type does not 
     * need to define a key property.
     * 
     * In order to understand how EF Core tracks these objects, it is useful 
     * to know that a primary key is created as a shadow property for the owned 
     * type. The value of the key of an instance of the owned type will be the 
     * same as the value of the key of the owner instance.
     ******************************************************************/

    [Owned]
    public class Address : ValueObject
    {
        public string Street1 { get; private set; }
        public string Street2 { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Zip { get; private set; }

        public Address() { }

        public Address(string street1, string street2, string city, string state, string zip)
        {
            Street1 = street1;
            Street2 = street2;
            City = city;
            State = state;            
            Zip = zip;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Street1;
            yield return Street2;
            yield return City;
            yield return State;
            yield return Zip;
        }

        public override string ToString()
        {
            return $"{Street1}, {Street2}, {City}, {State} {Zip}";
        }
    }
}
