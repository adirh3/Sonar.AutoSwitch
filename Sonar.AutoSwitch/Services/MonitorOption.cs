using Avalonia.Platform;
using System;

namespace Sonar.AutoSwitch.Services
{
    public class MonitorOption : IEquatable<MonitorOption>
    {
        private static int IdCount = 1;
        public int Id { get; }
        public string Name { get; }
        public Screen? Screen { get; }

        public MonitorOption(string name, Screen? screen)
        {
            Id = IdCount++;
            Name = name;
            Screen = screen;
        }

        public override string ToString() => Name;

        public bool Equals(MonitorOption? other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            return obj is MonitorOption other && Equals(other);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(MonitorOption? left, MonitorOption? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;
            return left.Equals(right);
        }

        public static bool operator !=(MonitorOption? left, MonitorOption? right) => !(left == right);
    }
}
