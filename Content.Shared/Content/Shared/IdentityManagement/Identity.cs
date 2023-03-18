using System;
using System.Runtime.CompilerServices;
using Content.Shared.Ghost;
using Content.Shared.IdentityManagement.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.IdentityManagement
{
	// Token: 0x020003FC RID: 1020
	[NullableContext(1)]
	[Nullable(0)]
	public static class Identity
	{
		// Token: 0x06000BE8 RID: 3048 RVA: 0x0002748C File Offset: 0x0002568C
		public static string Name(EntityUid uid, IEntityManager ent, EntityUid? viewer = null)
		{
			string uidName = ent.GetComponent<MetaDataComponent>(uid).EntityName;
			IdentityComponent identity;
			if (!ent.TryGetComponent<IdentityComponent>(uid, ref identity))
			{
				return uidName;
			}
			EntityUid? ident = identity.IdentityEntitySlot.ContainedEntity;
			if (ident == null)
			{
				return uidName;
			}
			string identName = ent.GetComponent<MetaDataComponent>(ident.Value).EntityName;
			if (viewer == null || !Identity.CanSeeThroughIdentity(uid, viewer.Value, ent))
			{
				return identName;
			}
			if (uidName == identName)
			{
				return uidName;
			}
			return uidName + " (" + identName + ")";
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x00027514 File Offset: 0x00025714
		public static EntityUid Entity(EntityUid uid, IEntityManager ent)
		{
			IdentityComponent identity;
			if (!ent.TryGetComponent<IdentityComponent>(uid, ref identity))
			{
				return uid;
			}
			EntityUid? containedEntity = identity.IdentityEntitySlot.ContainedEntity;
			if (containedEntity == null)
			{
				return uid;
			}
			return containedEntity.GetValueOrDefault();
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x0002754C File Offset: 0x0002574C
		public static bool CanSeeThroughIdentity(EntityUid uid, EntityUid viewer, IEntityManager ent)
		{
			return ent.HasComponent<SharedGhostComponent>(viewer);
		}
	}
}
