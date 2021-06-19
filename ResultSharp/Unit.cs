using System;
using System.Diagnostics.Contracts;

namespace ResultSharp
{
	/// <summary>
	/// Unit is a type that encodes the abscense of a specific value
	/// </summary>
	[Serializable]
	public struct Unit :
		IEquatable<Unit>,
		IComparable<Unit>
	{
		public int CompareTo(Unit other) => 0;
		public bool Equals(Unit other) => true;
		public override bool Equals(object? obj) => obj is Unit;
		public override string ToString() => "unit";
		public override int GetHashCode() => 0;
		public static bool operator ==(Unit a, Unit b) => true;
		public static bool operator !=(Unit a, Unit b) => false;
		public static Unit Default { get; } = default;
	}
}
