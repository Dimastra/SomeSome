using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mech
{
	// Token: 0x0200031F RID: 799
	[ByRefEvent]
	public readonly struct MechEquipmentRemovedEvent : IEquatable<MechEquipmentRemovedEvent>
	{
		// Token: 0x06000921 RID: 2337 RVA: 0x0001E921 File Offset: 0x0001CB21
		public MechEquipmentRemovedEvent(EntityUid Mech)
		{
			this.Mech = Mech;
		}

		// Token: 0x06000922 RID: 2338 RVA: 0x0001E92C File Offset: 0x0001CB2C
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MechEquipmentRemovedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000923 RID: 2339 RVA: 0x0001E978 File Offset: 0x0001CB78
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Mech = ");
			builder.Append(this.Mech.ToString());
			return true;
		}

		// Token: 0x06000924 RID: 2340 RVA: 0x0001E99F File Offset: 0x0001CB9F
		[CompilerGenerated]
		public static bool operator !=(MechEquipmentRemovedEvent left, MechEquipmentRemovedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06000925 RID: 2341 RVA: 0x0001E9AB File Offset: 0x0001CBAB
		[CompilerGenerated]
		public static bool operator ==(MechEquipmentRemovedEvent left, MechEquipmentRemovedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000926 RID: 2342 RVA: 0x0001E9B5 File Offset: 0x0001CBB5
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.Mech);
		}

		// Token: 0x06000927 RID: 2343 RVA: 0x0001E9C7 File Offset: 0x0001CBC7
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is MechEquipmentRemovedEvent && this.Equals((MechEquipmentRemovedEvent)obj);
		}

		// Token: 0x06000928 RID: 2344 RVA: 0x0001E9DF File Offset: 0x0001CBDF
		[CompilerGenerated]
		public bool Equals(MechEquipmentRemovedEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.Mech, other.Mech);
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x0001E9F7 File Offset: 0x0001CBF7
		[CompilerGenerated]
		public void Deconstruct(out EntityUid Mech)
		{
			Mech = this.Mech;
		}

		// Token: 0x04000916 RID: 2326
		public readonly EntityUid Mech;
	}
}
