using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;

namespace Content.Shared.Store
{
	// Token: 0x0200011F RID: 287
	[NullableContext(1)]
	[Nullable(0)]
	public readonly struct ListingConditionArgs : IEquatable<ListingConditionArgs>
	{
		// Token: 0x06000345 RID: 837 RVA: 0x0000E1B4 File Offset: 0x0000C3B4
		public ListingConditionArgs(EntityUid Buyer, EntityUid? StoreEntity, ListingData Listing, IEntityManager EntityManager)
		{
			this.Buyer = Buyer;
			this.StoreEntity = StoreEntity;
			this.Listing = Listing;
			this.EntityManager = EntityManager;
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000346 RID: 838 RVA: 0x0000E1D3 File Offset: 0x0000C3D3
		// (set) Token: 0x06000347 RID: 839 RVA: 0x0000E1DB File Offset: 0x0000C3DB
		public EntityUid Buyer { get; set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000348 RID: 840 RVA: 0x0000E1E4 File Offset: 0x0000C3E4
		// (set) Token: 0x06000349 RID: 841 RVA: 0x0000E1EC File Offset: 0x0000C3EC
		public EntityUid? StoreEntity { get; set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x0600034A RID: 842 RVA: 0x0000E1F5 File Offset: 0x0000C3F5
		// (set) Token: 0x0600034B RID: 843 RVA: 0x0000E1FD File Offset: 0x0000C3FD
		public ListingData Listing { get; set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x0600034C RID: 844 RVA: 0x0000E206 File Offset: 0x0000C406
		// (set) Token: 0x0600034D RID: 845 RVA: 0x0000E20E File Offset: 0x0000C40E
		public IEntityManager EntityManager { get; set; }

		// Token: 0x0600034E RID: 846 RVA: 0x0000E218 File Offset: 0x0000C418
		[NullableContext(0)]
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ListingConditionArgs");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0000E264 File Offset: 0x0000C464
		[NullableContext(0)]
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Buyer = ");
			builder.Append(this.Buyer.ToString());
			builder.Append(", StoreEntity = ");
			builder.Append(this.StoreEntity.ToString());
			builder.Append(", Listing = ");
			builder.Append(this.Listing);
			builder.Append(", EntityManager = ");
			builder.Append(this.EntityManager);
			return true;
		}

		// Token: 0x06000350 RID: 848 RVA: 0x0000E2F2 File Offset: 0x0000C4F2
		[CompilerGenerated]
		public static bool operator !=(ListingConditionArgs left, ListingConditionArgs right)
		{
			return !(left == right);
		}

		// Token: 0x06000351 RID: 849 RVA: 0x0000E2FE File Offset: 0x0000C4FE
		[CompilerGenerated]
		public static bool operator ==(ListingConditionArgs left, ListingConditionArgs right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000352 RID: 850 RVA: 0x0000E308 File Offset: 0x0000C508
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.<Buyer>k__BackingField) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.<StoreEntity>k__BackingField)) * -1521134295 + EqualityComparer<ListingData>.Default.GetHashCode(this.<Listing>k__BackingField)) * -1521134295 + EqualityComparer<IEntityManager>.Default.GetHashCode(this.<EntityManager>k__BackingField);
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0000E36A File Offset: 0x0000C56A
		[NullableContext(0)]
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is ListingConditionArgs && this.Equals((ListingConditionArgs)obj);
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0000E384 File Offset: 0x0000C584
		[CompilerGenerated]
		public bool Equals(ListingConditionArgs other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.<Buyer>k__BackingField, other.<Buyer>k__BackingField) && EqualityComparer<EntityUid?>.Default.Equals(this.<StoreEntity>k__BackingField, other.<StoreEntity>k__BackingField) && EqualityComparer<ListingData>.Default.Equals(this.<Listing>k__BackingField, other.<Listing>k__BackingField) && EqualityComparer<IEntityManager>.Default.Equals(this.<EntityManager>k__BackingField, other.<EntityManager>k__BackingField);
		}

		// Token: 0x06000355 RID: 853 RVA: 0x0000E3F1 File Offset: 0x0000C5F1
		[CompilerGenerated]
		public void Deconstruct(out EntityUid Buyer, out EntityUid? StoreEntity, out ListingData Listing, out IEntityManager EntityManager)
		{
			Buyer = this.Buyer;
			StoreEntity = this.StoreEntity;
			Listing = this.Listing;
			EntityManager = this.EntityManager;
		}
	}
}
