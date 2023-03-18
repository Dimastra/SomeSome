using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Systems;
using Content.Shared.Body.Components;
using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Salvage
{
	// Token: 0x02000220 RID: 544
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SalvageMobRestrictionsSystem : EntitySystem
	{
		// Token: 0x06000AC0 RID: 2752 RVA: 0x000381D8 File Offset: 0x000363D8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SalvageMobRestrictionsComponent, ComponentInit>(new ComponentEventHandler<SalvageMobRestrictionsComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<SalvageMobRestrictionsComponent, ComponentRemove>(new ComponentEventHandler<SalvageMobRestrictionsComponent, ComponentRemove>(this.OnRemove), null, null);
			base.SubscribeLocalEvent<SalvageMobRestrictionsGridComponent, ComponentRemove>(new ComponentEventHandler<SalvageMobRestrictionsGridComponent, ComponentRemove>(this.OnRemoveGrid), null, null);
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x00038228 File Offset: 0x00036428
		private void OnInit(EntityUid uid, SalvageMobRestrictionsComponent component, ComponentInit args)
		{
			EntityUid gridUid = base.Transform(uid).ParentUid;
			if (!this.EntityManager.EntityExists(gridUid))
			{
				return;
			}
			SalvageMobRestrictionsGridComponent rg;
			if (!base.TryComp<SalvageMobRestrictionsGridComponent>(gridUid, ref rg))
			{
				rg = base.AddComp<SalvageMobRestrictionsGridComponent>(gridUid);
			}
			rg.MobsToKill.Add(uid);
			component.LinkedGridEntity = gridUid;
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x00038278 File Offset: 0x00036478
		private void OnRemove(EntityUid uid, SalvageMobRestrictionsComponent component, ComponentRemove args)
		{
			SalvageMobRestrictionsGridComponent rg;
			if (base.TryComp<SalvageMobRestrictionsGridComponent>(component.LinkedGridEntity, ref rg))
			{
				rg.MobsToKill.Remove(uid);
			}
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x000382A4 File Offset: 0x000364A4
		private void OnRemoveGrid(EntityUid uid, SalvageMobRestrictionsGridComponent component, ComponentRemove args)
		{
			EntityQuery<MetaDataComponent> metaQuery = base.GetEntityQuery<MetaDataComponent>();
			EntityQuery<BodyComponent> bodyQuery = base.GetEntityQuery<BodyComponent>();
			EntityQuery<DamageableComponent> damageQuery = base.GetEntityQuery<DamageableComponent>();
			foreach (EntityUid target in component.MobsToKill)
			{
				if (!base.Deleted(target, metaQuery) && !this._mobStateSystem.IsDead(target, null))
				{
					BodyComponent body;
					DamageableComponent damageableComponent;
					if (bodyQuery.TryGetComponent(target, ref body))
					{
						this._bodySystem.GibBody(new EntityUid?(target), false, body, false);
					}
					else if (damageQuery.TryGetComponent(target, ref damageableComponent))
					{
						this._damageableSystem.SetAllDamage(damageableComponent, 200);
					}
				}
			}
		}

		// Token: 0x040006A0 RID: 1696
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x040006A1 RID: 1697
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x040006A2 RID: 1698
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;
	}
}
