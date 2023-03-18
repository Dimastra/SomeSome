using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Implants;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Content.Shared.Tag;
using Content.Shared.White.Mindshield;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.White.Mindshield
{
	// Token: 0x02000096 RID: 150
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MindShieldSystem : EntitySystem
	{
		// Token: 0x06000252 RID: 594 RVA: 0x0000CA7D File Offset: 0x0000AC7D
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SubdermalImplantInserted>(new EntityEventHandler<SubdermalImplantInserted>(this.OnImplantAdded), null, null);
			base.SubscribeLocalEvent<SubdermalImplantRemoved>(new EntityEventHandler<SubdermalImplantRemoved>(this.OnImplantRemoved), null, null);
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000CAB0 File Offset: 0x0000ACB0
		public void RemoveMindShieldImplant(EntityUid target, EntityUid mindShieldEntity, bool removeComponent = true)
		{
			this._subdermalImplantSystem.ForceRemove(target, mindShieldEntity);
			if (removeComponent)
			{
				this.EntityManager.RemoveComponent<MindShieldComponent>(target);
			}
			SubdermalImplantComponent subdermalImplantComponent;
			if (this._mindShields.TryGetValue(target, out subdermalImplantComponent))
			{
				this._mindShields.Remove(target);
			}
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000CAF8 File Offset: 0x0000ACF8
		private void OnImplantAdded(SubdermalImplantInserted ev)
		{
			if (!this._tagSystem.HasTag(ev.Component.Owner, "MindshieldImplant"))
			{
				return;
			}
			EntityUid? entity = ev.Component.ImplantedEntity;
			if (entity == null)
			{
				return;
			}
			this.EntityManager.EnsureComponent<MindShieldComponent>(entity.Value);
			this._mindShields[entity.Value] = ev.Component;
			base.RaiseLocalEvent<MindShieldImplanted>(new MindShieldImplanted(entity.Value, ev.Component.Owner));
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000CB84 File Offset: 0x0000AD84
		private void OnImplantRemoved(SubdermalImplantRemoved ev)
		{
			if (!this._tagSystem.HasTag(ev.Component.Owner, "MindshieldImplant"))
			{
				return;
			}
			EntityUid? entity = ev.Component.ImplantedEntity;
			if (entity == null)
			{
				return;
			}
			base.RemComp<MindShieldComponent>(ev.Component.ImplantedEntity.Value);
		}

		// Token: 0x040001AC RID: 428
		[Dependency]
		private readonly SubdermalImplantSystem _subdermalImplantSystem;

		// Token: 0x040001AD RID: 429
		[Dependency]
		private readonly TagSystem _tagSystem;

		// Token: 0x040001AE RID: 430
		[Dependency]
		private readonly ImplanterSystem _implanterSystem;

		// Token: 0x040001AF RID: 431
		[Dependency]
		private readonly SharedContainerSystem _sharedContainerSystem;

		// Token: 0x040001B0 RID: 432
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040001B1 RID: 433
		private Dictionary<EntityUid, SubdermalImplantComponent> _mindShields = new Dictionary<EntityUid, SubdermalImplantComponent>();

		// Token: 0x040001B2 RID: 434
		private string _mindshieldPrototype = "MindShieldImplant";
	}
}
