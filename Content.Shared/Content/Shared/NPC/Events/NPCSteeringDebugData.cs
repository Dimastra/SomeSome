using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC.Events
{
	// Token: 0x020002D3 RID: 723
	[NetSerializable]
	[Serializable]
	public readonly struct NPCSteeringDebugData : IEquatable<NPCSteeringDebugData>
	{
		// Token: 0x060007E3 RID: 2019 RVA: 0x0001A2E7 File Offset: 0x000184E7
		[NullableContext(1)]
		public NPCSteeringDebugData(EntityUid EntityUid, Vector2 Direction, float[] Interest, float[] Danger, List<Vector2> DangerPoints)
		{
			this.EntityUid = EntityUid;
			this.Direction = Direction;
			this.Interest = Interest;
			this.Danger = Danger;
			this.DangerPoints = DangerPoints;
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x0001A310 File Offset: 0x00018510
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("NPCSteeringDebugData");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x0001A35C File Offset: 0x0001855C
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("EntityUid = ");
			builder.Append(this.EntityUid.ToString());
			builder.Append(", Direction = ");
			builder.Append(this.Direction.ToString());
			builder.Append(", Interest = ");
			builder.Append(this.Interest);
			builder.Append(", Danger = ");
			builder.Append(this.Danger);
			builder.Append(", DangerPoints = ");
			builder.Append(this.DangerPoints);
			return true;
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x0001A3FD File Offset: 0x000185FD
		[CompilerGenerated]
		public static bool operator !=(NPCSteeringDebugData left, NPCSteeringDebugData right)
		{
			return !(left == right);
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x0001A409 File Offset: 0x00018609
		[CompilerGenerated]
		public static bool operator ==(NPCSteeringDebugData left, NPCSteeringDebugData right)
		{
			return left.Equals(right);
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x0001A414 File Offset: 0x00018614
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return (((EqualityComparer<EntityUid>.Default.GetHashCode(this.EntityUid) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.Direction)) * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(this.Interest)) * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(this.Danger)) * -1521134295 + EqualityComparer<List<Vector2>>.Default.GetHashCode(this.DangerPoints);
		}

		// Token: 0x060007E9 RID: 2025 RVA: 0x0001A48D File Offset: 0x0001868D
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is NPCSteeringDebugData && this.Equals((NPCSteeringDebugData)obj);
		}

		// Token: 0x060007EA RID: 2026 RVA: 0x0001A4A8 File Offset: 0x000186A8
		[CompilerGenerated]
		public bool Equals(NPCSteeringDebugData other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.EntityUid, other.EntityUid) && EqualityComparer<Vector2>.Default.Equals(this.Direction, other.Direction) && EqualityComparer<float[]>.Default.Equals(this.Interest, other.Interest) && EqualityComparer<float[]>.Default.Equals(this.Danger, other.Danger) && EqualityComparer<List<Vector2>>.Default.Equals(this.DangerPoints, other.DangerPoints);
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x0001A52D File Offset: 0x0001872D
		[NullableContext(1)]
		[CompilerGenerated]
		public void Deconstruct(out EntityUid EntityUid, out Vector2 Direction, out float[] Interest, out float[] Danger, out List<Vector2> DangerPoints)
		{
			EntityUid = this.EntityUid;
			Direction = this.Direction;
			Interest = this.Interest;
			Danger = this.Danger;
			DangerPoints = this.DangerPoints;
		}

		// Token: 0x04000822 RID: 2082
		public readonly EntityUid EntityUid;

		// Token: 0x04000823 RID: 2083
		public readonly Vector2 Direction;

		// Token: 0x04000824 RID: 2084
		[Nullable(1)]
		public readonly float[] Interest;

		// Token: 0x04000825 RID: 2085
		[Nullable(1)]
		public readonly float[] Danger;

		// Token: 0x04000826 RID: 2086
		[Nullable(1)]
		public readonly List<Vector2> DangerPoints;
	}
}
