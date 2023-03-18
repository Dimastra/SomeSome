using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Server.Station.Systems;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.AlertLevel
{
	// Token: 0x020007DE RID: 2014
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AlertLevelSystem : EntitySystem
	{
		// Token: 0x06002BC5 RID: 11205 RVA: 0x000E5A4A File Offset: 0x000E3C4A
		public override void Initialize()
		{
			base.SubscribeLocalEvent<StationInitializedEvent>(new EntityEventHandler<StationInitializedEvent>(this.OnStationInitialize), null, null);
			this._prototypeManager.PrototypesReloaded += this.OnPrototypeReload;
		}

		// Token: 0x06002BC6 RID: 11206 RVA: 0x000E5A77 File Offset: 0x000E3C77
		public override void Shutdown()
		{
			base.Shutdown();
			this._prototypeManager.PrototypesReloaded -= this.OnPrototypeReload;
		}

		// Token: 0x06002BC7 RID: 11207 RVA: 0x000E5A98 File Offset: 0x000E3C98
		public override void Update(float time)
		{
			foreach (EntityUid station in this._stationSystem.Stations)
			{
				AlertLevelComponent alert;
				if (base.TryComp<AlertLevelComponent>(station, ref alert))
				{
					if (alert.CurrentDelay <= 0f)
					{
						if (alert.ActiveDelay)
						{
							base.RaiseLocalEvent<AlertLevelDelayFinishedEvent>(new AlertLevelDelayFinishedEvent());
							alert.ActiveDelay = false;
						}
					}
					else
					{
						alert.CurrentDelay -= time;
					}
				}
			}
		}

		// Token: 0x06002BC8 RID: 11208 RVA: 0x000E5B28 File Offset: 0x000E3D28
		private void OnStationInitialize(StationInitializedEvent args)
		{
			AlertLevelComponent alertLevelComponent = base.AddComp<AlertLevelComponent>(args.Station);
			AlertLevelPrototype alerts;
			if (!this._prototypeManager.TryIndex<AlertLevelPrototype>("stationAlerts", ref alerts))
			{
				return;
			}
			alertLevelComponent.AlertLevels = alerts;
			string defaultLevel = alertLevelComponent.AlertLevels.DefaultLevel;
			if (string.IsNullOrEmpty(defaultLevel))
			{
				defaultLevel = alertLevelComponent.AlertLevels.Levels.Keys.First<string>();
			}
			this.SetLevel(args.Station, defaultLevel, false, false, true, false, null, null);
		}

		// Token: 0x06002BC9 RID: 11209 RVA: 0x000E5B9C File Offset: 0x000E3D9C
		private void OnPrototypeReload(PrototypesReloadedEventArgs args)
		{
			PrototypesReloadedEventArgs.PrototypeChangeSet alertPrototypes;
			IPrototype alertObject;
			if (args.ByType.TryGetValue(typeof(AlertLevelPrototype), out alertPrototypes) && alertPrototypes.Modified.TryGetValue("stationAlerts", out alertObject))
			{
				AlertLevelPrototype alerts = alertObject as AlertLevelPrototype;
				if (alerts != null)
				{
					foreach (AlertLevelComponent comp in base.EntityQuery<AlertLevelComponent>(false))
					{
						comp.AlertLevels = alerts;
						if (!comp.AlertLevels.Levels.ContainsKey(comp.CurrentLevel))
						{
							string defaultLevel = comp.AlertLevels.DefaultLevel;
							if (string.IsNullOrEmpty(defaultLevel))
							{
								defaultLevel = comp.AlertLevels.Levels.Keys.First<string>();
							}
							this.SetLevel(comp.Owner, defaultLevel, true, true, true, false, null, null);
						}
					}
					base.RaiseLocalEvent<AlertLevelPrototypeReloadedEvent>(new AlertLevelPrototypeReloadedEvent());
					return;
				}
			}
		}

		// Token: 0x06002BCA RID: 11210 RVA: 0x000E5C90 File Offset: 0x000E3E90
		[NullableContext(2)]
		public float GetAlertLevelDelay(EntityUid station, AlertLevelComponent alert = null)
		{
			if (!base.Resolve<AlertLevelComponent>(station, ref alert, true))
			{
				return float.NaN;
			}
			return alert.CurrentDelay;
		}

		// Token: 0x06002BCB RID: 11211 RVA: 0x000E5CAC File Offset: 0x000E3EAC
		[NullableContext(2)]
		public void SetLevel(EntityUid station, [Nullable(1)] string level, bool playSound, bool announce, bool force = false, bool locked = false, MetaDataComponent dataComponent = null, AlertLevelComponent component = null)
		{
			AlertLevelDetail detail;
			if (!base.Resolve<AlertLevelComponent, MetaDataComponent>(station, ref component, ref dataComponent, true) || component.AlertLevels == null || !component.AlertLevels.Levels.TryGetValue(level, out detail) || component.CurrentLevel == level)
			{
				return;
			}
			if (!force)
			{
				if (!detail.Selectable || component.CurrentDelay > 0f || component.IsLevelLocked)
				{
					return;
				}
				component.CurrentDelay = 30f;
				component.ActiveDelay = true;
			}
			component.CurrentLevel = level;
			component.IsLevelLocked = locked;
			string stationName = dataComponent.EntityName;
			string name = level.ToLower();
			string locName;
			if (Loc.TryGetString("alert-level-" + level, ref locName))
			{
				name = locName.ToLower();
			}
			string announcement = detail.Announcement;
			string locAnnouncement;
			if (Loc.TryGetString(detail.Announcement, ref locAnnouncement))
			{
				announcement = locAnnouncement;
			}
			string announcementFull = Loc.GetString("alert-level-announcement", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("name", name),
				new ValueTuple<string, object>("announcement", announcement)
			});
			bool playDefault = false;
			if (playSound)
			{
				if (detail.Sound != null)
				{
					Filter filter = this._stationSystem.GetInOwningStation(station, 32f);
					SoundSystem.Play(detail.Sound.GetSound(null, null), filter, new AudioParams?(detail.Sound.Params));
				}
				else
				{
					playDefault = true;
				}
			}
			if (announce)
			{
				ChatSystem chatSystem = this._chatSystem;
				string message = announcementFull;
				bool playDefaultSound = playDefault;
				Color? colorOverride = new Color?(detail.Color);
				chatSystem.DispatchStationAnnouncement(station, message, stationName, playDefaultSound, null, colorOverride);
			}
			base.RaiseLocalEvent<AlertLevelChangedEvent>(new AlertLevelChangedEvent(station, level));
		}

		// Token: 0x04001B32 RID: 6962
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001B33 RID: 6963
		[Dependency]
		private readonly ChatSystem _chatSystem;

		// Token: 0x04001B34 RID: 6964
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x04001B35 RID: 6965
		public const string DefaultAlertLevelSet = "stationAlerts";
	}
}
