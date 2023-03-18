using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.Reagent
{
	// Token: 0x020005E5 RID: 1509
	[NullableContext(1)]
	[Nullable(0)]
	public readonly struct ReagentEffectArgs : IEquatable<ReagentEffectArgs>
	{
		// Token: 0x06001213 RID: 4627 RVA: 0x0003B4FF File Offset: 0x000396FF
		public ReagentEffectArgs(EntityUid SolutionEntity, EntityUid? OrganEntity, [Nullable(2)] Solution Source, ReagentPrototype Reagent, FixedPoint2 Quantity, IEntityManager EntityManager, ReactionMethod? Method, float Scale)
		{
			this.SolutionEntity = SolutionEntity;
			this.OrganEntity = OrganEntity;
			this.Source = Source;
			this.Reagent = Reagent;
			this.Quantity = Quantity;
			this.EntityManager = EntityManager;
			this.Method = Method;
			this.Scale = Scale;
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06001214 RID: 4628 RVA: 0x0003B53E File Offset: 0x0003973E
		// (set) Token: 0x06001215 RID: 4629 RVA: 0x0003B546 File Offset: 0x00039746
		public EntityUid SolutionEntity { get; set; }

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06001216 RID: 4630 RVA: 0x0003B54F File Offset: 0x0003974F
		// (set) Token: 0x06001217 RID: 4631 RVA: 0x0003B557 File Offset: 0x00039757
		public EntityUid? OrganEntity { get; set; }

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06001218 RID: 4632 RVA: 0x0003B560 File Offset: 0x00039760
		// (set) Token: 0x06001219 RID: 4633 RVA: 0x0003B568 File Offset: 0x00039768
		[Nullable(2)]
		public Solution Source { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x0600121A RID: 4634 RVA: 0x0003B571 File Offset: 0x00039771
		// (set) Token: 0x0600121B RID: 4635 RVA: 0x0003B579 File Offset: 0x00039779
		public ReagentPrototype Reagent { get; set; }

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x0600121C RID: 4636 RVA: 0x0003B582 File Offset: 0x00039782
		// (set) Token: 0x0600121D RID: 4637 RVA: 0x0003B58A File Offset: 0x0003978A
		public FixedPoint2 Quantity { get; set; }

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x0600121E RID: 4638 RVA: 0x0003B593 File Offset: 0x00039793
		// (set) Token: 0x0600121F RID: 4639 RVA: 0x0003B59B File Offset: 0x0003979B
		public IEntityManager EntityManager { get; set; }

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06001220 RID: 4640 RVA: 0x0003B5A4 File Offset: 0x000397A4
		// (set) Token: 0x06001221 RID: 4641 RVA: 0x0003B5AC File Offset: 0x000397AC
		public ReactionMethod? Method { get; set; }

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06001222 RID: 4642 RVA: 0x0003B5B5 File Offset: 0x000397B5
		// (set) Token: 0x06001223 RID: 4643 RVA: 0x0003B5BD File Offset: 0x000397BD
		public float Scale { get; set; }

		// Token: 0x06001224 RID: 4644 RVA: 0x0003B5C8 File Offset: 0x000397C8
		[NullableContext(0)]
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ReagentEffectArgs");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06001225 RID: 4645 RVA: 0x0003B614 File Offset: 0x00039814
		[NullableContext(0)]
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("SolutionEntity = ");
			builder.Append(this.SolutionEntity.ToString());
			builder.Append(", OrganEntity = ");
			builder.Append(this.OrganEntity.ToString());
			builder.Append(", Source = ");
			builder.Append(this.Source);
			builder.Append(", Reagent = ");
			builder.Append(this.Reagent);
			builder.Append(", Quantity = ");
			builder.Append(this.Quantity.ToString());
			builder.Append(", EntityManager = ");
			builder.Append(this.EntityManager);
			builder.Append(", Method = ");
			builder.Append(this.Method.ToString());
			builder.Append(", Scale = ");
			builder.Append(this.Scale.ToString());
			return true;
		}

		// Token: 0x06001226 RID: 4646 RVA: 0x0003B731 File Offset: 0x00039931
		[CompilerGenerated]
		public static bool operator !=(ReagentEffectArgs left, ReagentEffectArgs right)
		{
			return !(left == right);
		}

		// Token: 0x06001227 RID: 4647 RVA: 0x0003B73D File Offset: 0x0003993D
		[CompilerGenerated]
		public static bool operator ==(ReagentEffectArgs left, ReagentEffectArgs right)
		{
			return left.Equals(right);
		}

		// Token: 0x06001228 RID: 4648 RVA: 0x0003B748 File Offset: 0x00039948
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((((((EqualityComparer<EntityUid>.Default.GetHashCode(this.<SolutionEntity>k__BackingField) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.<OrganEntity>k__BackingField)) * -1521134295 + EqualityComparer<Solution>.Default.GetHashCode(this.<Source>k__BackingField)) * -1521134295 + EqualityComparer<ReagentPrototype>.Default.GetHashCode(this.<Reagent>k__BackingField)) * -1521134295 + EqualityComparer<FixedPoint2>.Default.GetHashCode(this.<Quantity>k__BackingField)) * -1521134295 + EqualityComparer<IEntityManager>.Default.GetHashCode(this.<EntityManager>k__BackingField)) * -1521134295 + EqualityComparer<ReactionMethod?>.Default.GetHashCode(this.<Method>k__BackingField)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.<Scale>k__BackingField);
		}

		// Token: 0x06001229 RID: 4649 RVA: 0x0003B806 File Offset: 0x00039A06
		[NullableContext(0)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is ReagentEffectArgs && this.Equals((ReagentEffectArgs)obj);
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x0003B820 File Offset: 0x00039A20
		[CompilerGenerated]
		public bool Equals(ReagentEffectArgs other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<SolutionEntity>k__BackingField, other.<SolutionEntity>k__BackingField) && EqualityComparer<EntityUid?>.Default.Equals(this.<OrganEntity>k__BackingField, other.<OrganEntity>k__BackingField) && EqualityComparer<Solution>.Default.Equals(this.<Source>k__BackingField, other.<Source>k__BackingField) && EqualityComparer<ReagentPrototype>.Default.Equals(this.<Reagent>k__BackingField, other.<Reagent>k__BackingField) && EqualityComparer<FixedPoint2>.Default.Equals(this.<Quantity>k__BackingField, other.<Quantity>k__BackingField) && EqualityComparer<IEntityManager>.Default.Equals(this.<EntityManager>k__BackingField, other.<EntityManager>k__BackingField) && EqualityComparer<ReactionMethod?>.Default.Equals(this.<Method>k__BackingField, other.<Method>k__BackingField) && EqualityComparer<float>.Default.Equals(this.<Scale>k__BackingField, other.<Scale>k__BackingField);
		}

		// Token: 0x0600122B RID: 4651 RVA: 0x0003B8F4 File Offset: 0x00039AF4
		[CompilerGenerated]
		public void Deconstruct(out EntityUid SolutionEntity, out EntityUid? OrganEntity, [Nullable(2)] out Solution Source, out ReagentPrototype Reagent, out FixedPoint2 Quantity, out IEntityManager EntityManager, out ReactionMethod? Method, out float Scale)
		{
			SolutionEntity = this.SolutionEntity;
			OrganEntity = this.OrganEntity;
			Source = this.Source;
			Reagent = this.Reagent;
			Quantity = this.Quantity;
			EntityManager = this.EntityManager;
			Method = this.Method;
			Scale = this.Scale;
		}
	}
}
