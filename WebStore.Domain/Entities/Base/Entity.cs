using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebStore.Domain.Entities.Base.Interfaces;

namespace WebStore.Domain.Entities.Base;

public abstract class Entity : IEntity, IEquatable<Entity>
{
    public int Id { get; set; }

    // сравниваем тоже самое, только добавляется срвнение по Id
    public bool Equals(Entity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        // если типы совпадают, то вызываем метод из IEquatable
        return Equals((Entity)obj);
    }

    public override int GetHashCode() => Id;

    public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);
    public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);
}
