using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mech
{
	// Token: 0x0200031E RID: 798
	[ByRefEvent]
	public readonly struct MechEquipmentInsertedEvent : IEquatable<MechEquipmentInsertedEvent>
	{
		// Token: 0x06000918 RID: 2328 RVA: 0x0001E83D File Offset: 0x0001CA3D
		public MechEquipmentInsertedEvent(EntityUid Mech)
		{
			this.Mech = Mech;
		}

		// Token: 0x06000919 RID: 2329 RVA: 0x0001E848 File Offset: 0x0001CA48
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MechEquipmentInsertedEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x0001E894 File Offset: 0x0001CA94
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Mech = ");
			builder.Append(this.Mech.ToString());
			return true;
		}

		// Token: 0x0600091B RID: 2331 RVA: 0x0001E8BB File Offset: 0x0001CABB
		[CompilerGenerated]
		public static bool operator !=(MechEquipmentInsertedEvent left, MechEquipmentInsertedEvent right)
		{
			return !(left == right);
		}

		// Token: 0x0600091C RID: 2332 RVA: 0x0001E8C7 File Offset: 0x0001CAC7
		[CompilerGenerated]
		public static bool operator ==(MechEquipmentInsertedEvent left, MechEquipmentInsertedEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x0001E8D1 File Offset: 0x0001CAD1
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<EntityUid>.Default.GetHashCode(this.Mech);
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x0001E8E3 File Offset: 0x0001CAE3
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is MechEquipmentInsertedEvent && this.Equals((MechEquipmentInsertedEvent)obj);
		}

		// Token: 0x0600091F RID: 2335 RVA: 0x0001E8FB File Offset: 0x0001CAFB
		[CompilerGenerated]
		public bool Equals(MechEquipmentInsertedEvent other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.Mech, other.Mech);
		}

		// Token: 0x06000920 RID: 2336 RVA: 0x0001E913 File Offset: 0x0001CB13
		[CompilerGenerated]
		public void Deconstruct(out EntityUid Mech)
		{
			Mech = this.Mech;
		}

		// Token: 0x04000915 RID: 2325
		public readonly EntityUid Mech;
	}
}
