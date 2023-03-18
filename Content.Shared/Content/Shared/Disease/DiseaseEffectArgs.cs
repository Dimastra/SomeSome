using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Disease
{
	// Token: 0x02000505 RID: 1285
	[NullableContext(1)]
	[Nullable(0)]
	public readonly struct DiseaseEffectArgs : IEquatable<DiseaseEffectArgs>
	{
		// Token: 0x06000F86 RID: 3974 RVA: 0x00032638 File Offset: 0x00030838
		public DiseaseEffectArgs(EntityUid DiseasedEntity, DiseasePrototype Disease, IEntityManager EntityManager)
		{
			this.DiseasedEntity = DiseasedEntity;
			this.Disease = Disease;
			this.EntityManager = EntityManager;
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06000F87 RID: 3975 RVA: 0x0003264F File Offset: 0x0003084F
		// (set) Token: 0x06000F88 RID: 3976 RVA: 0x00032657 File Offset: 0x00030857
		public EntityUid DiseasedEntity { get; set; }

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06000F89 RID: 3977 RVA: 0x00032660 File Offset: 0x00030860
		// (set) Token: 0x06000F8A RID: 3978 RVA: 0x00032668 File Offset: 0x00030868
		public DiseasePrototype Disease { get; set; }

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06000F8B RID: 3979 RVA: 0x00032671 File Offset: 0x00030871
		// (set) Token: 0x06000F8C RID: 3980 RVA: 0x00032679 File Offset: 0x00030879
		public IEntityManager EntityManager { get; set; }

		// Token: 0x06000F8D RID: 3981 RVA: 0x00032684 File Offset: 0x00030884
		[NullableContext(0)]
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("DiseaseEffectArgs");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x000326D0 File Offset: 0x000308D0
		[NullableContext(0)]
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("DiseasedEntity = ");
			builder.Append(this.DiseasedEntity.ToString());
			builder.Append(", Disease = ");
			builder.Append(this.Disease);
			builder.Append(", EntityManager = ");
			builder.Append(this.EntityManager);
			return true;
		}

		// Token: 0x06000F8F RID: 3983 RVA: 0x00032737 File Offset: 0x00030937
		[CompilerGenerated]
		public static bool operator !=(DiseaseEffectArgs left, DiseaseEffectArgs right)
		{
			return !(left == right);
		}

		// Token: 0x06000F90 RID: 3984 RVA: 0x00032743 File Offset: 0x00030943
		[CompilerGenerated]
		public static bool operator ==(DiseaseEffectArgs left, DiseaseEffectArgs right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000F91 RID: 3985 RVA: 0x0003274D File Offset: 0x0003094D
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return (EqualityComparer<EntityUid>.Default.GetHashCode(this.<DiseasedEntity>k__BackingField) * -1521134295 + EqualityComparer<DiseasePrototype>.Default.GetHashCode(this.<Disease>k__BackingField)) * -1521134295 + EqualityComparer<IEntityManager>.Default.GetHashCode(this.<EntityManager>k__BackingField);
		}

		// Token: 0x06000F92 RID: 3986 RVA: 0x0003278D File Offset: 0x0003098D
		[NullableContext(0)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is DiseaseEffectArgs && this.Equals((DiseaseEffectArgs)obj);
		}

		// Token: 0x06000F93 RID: 3987 RVA: 0x000327A8 File Offset: 0x000309A8
		[CompilerGenerated]
		public bool Equals(DiseaseEffectArgs other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<DiseasedEntity>k__BackingField, other.<DiseasedEntity>k__BackingField) && EqualityComparer<DiseasePrototype>.Default.Equals(this.<Disease>k__BackingField, other.<Disease>k__BackingField) && EqualityComparer<IEntityManager>.Default.Equals(this.<EntityManager>k__BackingField, other.<EntityManager>k__BackingField);
		}

		// Token: 0x06000F94 RID: 3988 RVA: 0x000327FD File Offset: 0x000309FD
		[CompilerGenerated]
		public void Deconstruct(out EntityUid DiseasedEntity, out DiseasePrototype Disease, out IEntityManager EntityManager)
		{
			DiseasedEntity = this.DiseasedEntity;
			Disease = this.Disease;
			EntityManager = this.EntityManager;
		}
	}
}
