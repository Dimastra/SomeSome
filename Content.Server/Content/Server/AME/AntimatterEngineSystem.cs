using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.AME.Components;
using Content.Server.Hands.Components;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Tools;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Server.AME
{
	// Token: 0x020007D5 RID: 2005
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AntimatterEngineSystem : EntitySystem
	{
		// Token: 0x06002B8C RID: 11148 RVA: 0x000E4A94 File Offset: 0x000E2C94
		public override void Initialize()
		{
			base.Initialize();
			ComponentEventRefHandler<AMEControllerComponent, PowerChangedEvent> componentEventRefHandler;
			if ((componentEventRefHandler = AntimatterEngineSystem.<>O.<0>__OnAMEPowerChange) == null)
			{
				componentEventRefHandler = (AntimatterEngineSystem.<>O.<0>__OnAMEPowerChange = new ComponentEventRefHandler<AMEControllerComponent, PowerChangedEvent>(AntimatterEngineSystem.OnAMEPowerChange));
			}
			base.SubscribeLocalEvent<AMEControllerComponent, PowerChangedEvent>(componentEventRefHandler, null, null);
			base.SubscribeLocalEvent<AMEControllerComponent, InteractUsingEvent>(new ComponentEventHandler<AMEControllerComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<AMEPartComponent, InteractUsingEvent>(new ComponentEventHandler<AMEPartComponent, InteractUsingEvent>(this.OnPartInteractUsing), null, null);
		}

		// Token: 0x06002B8D RID: 11149 RVA: 0x000E4AF4 File Offset: 0x000E2CF4
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this._accumulatedFrameTime += frameTime;
			if (this._accumulatedFrameTime >= 10f)
			{
				foreach (AMEControllerComponent amecontrollerComponent in this.EntityManager.EntityQuery<AMEControllerComponent>(false))
				{
					amecontrollerComponent.OnUpdate(frameTime);
				}
				this._accumulatedFrameTime -= 10f;
			}
		}

		// Token: 0x06002B8E RID: 11150 RVA: 0x000E4B7C File Offset: 0x000E2D7C
		private static void OnAMEPowerChange(EntityUid uid, AMEControllerComponent component, ref PowerChangedEvent args)
		{
			component.UpdateUserInterface();
		}

		// Token: 0x06002B8F RID: 11151 RVA: 0x000E4B84 File Offset: 0x000E2D84
		private void OnInteractUsing(EntityUid uid, AMEControllerComponent component, InteractUsingEvent args)
		{
			HandsComponent hands;
			if (!base.TryComp<HandsComponent>(args.User, ref hands))
			{
				this._popupSystem.PopupEntity(Loc.GetString("ame-controller-component-interact-using-no-hands-text"), uid, args.User, PopupType.Small);
				return;
			}
			if (!base.HasComp<AMEFuelContainerComponent>(args.Used))
			{
				this._popupSystem.PopupEntity(Loc.GetString("ame-controller-component-interact-using-fail"), uid, args.User, PopupType.Small);
				return;
			}
			if (component.HasJar)
			{
				this._popupSystem.PopupEntity(Loc.GetString("ame-controller-component-interact-using-already-has-jar"), uid, args.User, PopupType.Small);
				return;
			}
			component.JarSlot.Insert(args.Used, null, null, null, null, null);
			this._popupSystem.PopupEntity(Loc.GetString("ame-controller-component-interact-using-success"), uid, args.User, PopupType.Medium);
			component.UpdateUserInterface();
		}

		// Token: 0x06002B90 RID: 11152 RVA: 0x000E4C4C File Offset: 0x000E2E4C
		private void OnPartInteractUsing(EntityUid uid, AMEPartComponent component, InteractUsingEvent args)
		{
			if (!base.HasComp<HandsComponent>(args.User))
			{
				this._popupSystem.PopupEntity(Loc.GetString("ame-part-component-interact-using-no-hands"), uid, args.User, PopupType.Small);
				return;
			}
			if (!this._toolSystem.HasQuality(args.Used, component.QualityNeeded, null))
			{
				return;
			}
			MapGridComponent mapGrid;
			if (!this._mapManager.TryGetGrid(args.ClickLocation.GetGridUid(this.EntityManager), ref mapGrid))
			{
				return;
			}
			Vector2i snapPos = mapGrid.TileIndicesFor(args.ClickLocation);
			if (mapGrid.GetAnchoredEntities(snapPos).Any((EntityUid sc) => base.HasComp<AMEShieldComponent>(sc)))
			{
				this._popupSystem.PopupEntity(Loc.GetString("ame-part-component-shielding-already-present"), uid, args.User, PopupType.Small);
				return;
			}
			EntityUid ent = this.EntityManager.SpawnEntity("AMEShielding", mapGrid.GridTileToLocal(snapPos));
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Construction;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(20, 4);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "player", "ToPrettyString(args.User)");
			logStringHandler.AppendLiteral(" unpacked ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(ent), "ToPrettyString(ent)");
			logStringHandler.AppendLiteral(" at ");
			logStringHandler.AppendFormatted<EntityCoordinates>(base.Transform(ent).Coordinates, "Transform(ent).Coordinates");
			logStringHandler.AppendLiteral(" from ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
			adminLogger.Add(type, impact, ref logStringHandler);
			SoundSystem.Play(component.UnwrapSound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, null);
			this.EntityManager.QueueDeleteEntity(uid);
		}

		// Token: 0x04001B01 RID: 6913
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04001B02 RID: 6914
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04001B03 RID: 6915
		[Dependency]
		private readonly ToolSystem _toolSystem;

		// Token: 0x04001B04 RID: 6916
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04001B05 RID: 6917
		private float _accumulatedFrameTime;

		// Token: 0x04001B06 RID: 6918
		private const float UpdateCooldown = 10f;

		// Token: 0x02000B39 RID: 2873
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002997 RID: 10647
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<AMEControllerComponent, PowerChangedEvent> <0>__OnAMEPowerChange;
		}
	}
}
