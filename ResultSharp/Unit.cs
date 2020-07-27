using System;

namespace ResultSharp
{
	[Serializable]
	public struct Unit :
		IEquatable<Unit>,
		IComparable<Unit>
	{
		public int CompareTo(Unit other) => 0;
		public bool Equals(Unit other) => true;
		public override bool Equals(object obj) => obj is Unit;
		public override string ToString() => "unit";
		public override int GetHashCode() => 0;
		public static bool operator ==(Unit a, Unit b) => true;
		public static bool operator !=(Unit a, Unit b) => false;
		public static Unit Default { get; } = new Unit();
	}
}
