using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Server.NPC.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.NPC.Events
{
	// Token: 0x02000367 RID: 871
	[ByRefEvent]
	public readonly struct NPCSteeringEvent : IEquatable<NPCSteeringEvent>
	{
		// Token: 0x060011FD RID: 4605 RVA: 0x0005E73D File Offset: 0x0005C93D
		[NullableContext(1)]
		public NPCSteeringEvent(NPCSteeringComponent Steering, float[] Interest, float[] Danger, float AgentRadius, Angle OffsetRotation, Vector2 WorldPosition)
		{
			this.Steering = Steering;
			this.Interest = Interest;
			this.Danger = Danger;
			this.AgentRadius = AgentRadius;
			this.OffsetRotation = OffsetRotation;
			this.WorldPosition = WorldPosition;
		}

		// Token: 0x060011FE RID: 4606 RVA: 0x0005E76C File Offset: 0x0005C96C
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("NPCSteeringEvent");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060011FF RID: 4607 RVA: 0x0005E7B8 File Offset: 0x0005C9B8
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Steering = ");
			builder.Append(this.Steering);
			builder.Append(", Interest = ");
			builder.Append(this.Interest);
			builder.Append(", Danger = ");
			builder.Append(this.Danger);
			builder.Append(", AgentRadius = ");
			builder.Append(this.AgentRadius.ToString());
			builder.Append(", OffsetRotation = ");
			builder.Append(this.OffsetRotation.ToString());
			builder.Append(", WorldPosition = ");
			builder.Append(this.WorldPosition.ToString());
			return true;
		}

		// Token: 0x06001200 RID: 4608 RVA: 0x0005E87D File Offset: 0x0005CA7D
		[CompilerGenerated]
		public static bool operator !=(NPCSteeringEvent left, NPCSteeringEvent right)
		{
			return !(left == right);
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x0005E889 File Offset: 0x0005CA89
		[CompilerGenerated]
		public static bool operator ==(NPCSteeringEvent left, NPCSteeringEvent right)
		{
			return left.Equals(right);
		}

		// Token: 0x06001202 RID: 4610 RVA: 0x0005E894 File Offset: 0x0005CA94
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((((EqualityComparer<NPCSteeringComponent>.Default.GetHashCode(this.Steering) * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(this.Interest)) * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(this.Danger)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.AgentRadius)) * -1521134295 + EqualityComparer<Angle>.Default.GetHashCode(this.OffsetRotation)) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.WorldPosition);
		}

		// Token: 0x06001203 RID: 4611 RVA: 0x0005E924 File Offset: 0x0005CB24
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is NPCSteeringEvent && this.Equals((NPCSteeringEvent)obj);
		}

		// Token: 0x06001204 RID: 4612 RVA: 0x0005E93C File Offset: 0x0005CB3C
		[CompilerGenerated]
		public bool Equals(NPCSteeringEvent other)
		{
			return EqualityComparer<NPCSteeringComponent>.Default.Equals(this.Steering, other.Steering) && EqualityComparer<float[]>.Default.Equals(this.Interest, other.Interest) && EqualityComparer<float[]>.Default.Equals(this.Danger, other.Danger) && EqualityComparer<float>.Default.Equals(this.AgentRadius, other.AgentRadius) && EqualityComparer<Angle>.Default.Equals(this.OffsetRotation, other.OffsetRotation) && EqualityComparer<Vector2>.Default.Equals(this.WorldPosition, other.WorldPosition);
		}

		// Token: 0x06001205 RID: 4613 RVA: 0x0005E9D9 File Offset: 0x0005CBD9
		[NullableContext(1)]
		[CompilerGenerated]
		public void Deconstruct(out NPCSteeringComponent Steering, out float[] Interest, out float[] Danger, out float AgentRadius, out Angle OffsetRotation, out Vector2 WorldPosition)
		{
			Steering = this.Steering;
			Interest = this.Interest;
			Danger = this.Danger;
			AgentRadius = this.AgentRadius;
			OffsetRotation = this.OffsetRotation;
			WorldPosition = this.WorldPosition;
		}

		// Token: 0x04000AF6 RID: 2806
		[Nullable(1)]
		public readonly NPCSteeringComponent Steering;

		// Token: 0x04000AF7 RID: 2807
		[Nullable(1)]
		public readonly float[] Interest;

		// Token: 0x04000AF8 RID: 2808
		[Nullable(1)]
		public readonly float[] Danger;

		// Token: 0x04000AF9 RID: 2809
		public readonly float AgentRadius;

		// Token: 0x04000AFA RID: 2810
		public readonly Angle OffsetRotation;

		// Token: 0x04000AFB RID: 2811
		public readonly Vector2 WorldPosition;
	}
}
