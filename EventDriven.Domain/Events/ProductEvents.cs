using System;
using System.Collections.Generic;
using System.Text;

namespace EventDriven.Domain.Events
{
    public record ProductCreated(int Id, string Name, int Quantity);
    public record ProductUpdated(int Id, string Name, int Quantity);
    public record ProductDeleted(int Id, string Name);

    public interface IDomainEvent { }
}
