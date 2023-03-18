using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x0200004D RID: 77
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TelepathicArtifactSystem : EntitySystem
	{
		// Token: 0x060000EF RID: 239 RVA: 0x0000682C File Offset: 0x00004A2C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<TelepathicArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<TelepathicArtifactComponent, ArtifactActivatedEvent>(this.OnActivate), null, null);
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00006848 File Offset: 0x00004A48
		private void OnActivate(EntityUid uid, TelepathicArtifactComponent component, ArtifactActivatedEvent args)
		{
			foreach (EntityUid victimUid in this._lookup.GetEntitiesInRange(uid, component.Range, 46))
			{
				if (this.EntityManager.HasComponent<ActorComponent>(victimUid))
				{
					List<string> msgArr;
					if (this._random.NextFloat() <= component.DrasticMessageProb && component.DrasticMessages != null)
					{
						msgArr = component.DrasticMessages;
					}
					else
					{
						msgArr = component.Messages;
					}
					string msg = Loc.GetString(RandomExtensions.Pick<string>(this._random, msgArr));
					this._popupSystem.PopupEntity(msg, victimUid, victimUid, PopupType.Small);
				}
			}
		}

		// Token: 0x040000AD RID: 173
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040000AE RID: 174
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040000AF RID: 175
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;
	}
}
