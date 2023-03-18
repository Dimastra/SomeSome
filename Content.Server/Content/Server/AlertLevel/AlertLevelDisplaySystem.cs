using System;
using System.Runtime.CompilerServices;
using Content.Server.Station.Systems;
using Content.Shared.AlertLevel;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.AlertLevel
{
	// Token: 0x020007DB RID: 2011
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AlertLevelDisplaySystem : EntitySystem
	{
		// Token: 0x06002BB5 RID: 11189 RVA: 0x000E588B File Offset: 0x000E3A8B
		public override void Initialize()
		{
			base.SubscribeLocalEvent<AlertLevelChangedEvent>(new EntityEventHandler<AlertLevelChangedEvent>(this.OnAlertChanged), null, null);
			base.SubscribeLocalEvent<AlertLevelDisplayComponent, ComponentInit>(new ComponentEventHandler<AlertLevelDisplayComponent, ComponentInit>(this.OnDisplayInit), null, null);
		}

		// Token: 0x06002BB6 RID: 11190 RVA: 0x000E58B8 File Offset: 0x000E3AB8
		private void OnAlertChanged(AlertLevelChangedEvent args)
		{
			foreach (ValueTuple<AlertLevelDisplayComponent, AppearanceComponent> valueTuple in this.EntityManager.EntityQuery<AlertLevelDisplayComponent, AppearanceComponent>(false))
			{
				AppearanceComponent appearance = valueTuple.Item2;
				this._appearance.SetData(appearance.Owner, AlertLevelDisplay.CurrentLevel, args.AlertLevel, appearance);
			}
		}

		// Token: 0x06002BB7 RID: 11191 RVA: 0x000E5928 File Offset: 0x000E3B28
		private void OnDisplayInit(EntityUid uid, AlertLevelDisplayComponent component, ComponentInit args)
		{
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				EntityUid? stationUid = this._stationSystem.GetOwningStation(uid, null);
				AlertLevelComponent alert;
				if (stationUid != null && base.TryComp<AlertLevelComponent>(stationUid, ref alert))
				{
					this._appearance.SetData(uid, AlertLevelDisplay.CurrentLevel, alert.CurrentLevel, appearance);
				}
			}
		}

		// Token: 0x04001B25 RID: 6949
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x04001B26 RID: 6950
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
