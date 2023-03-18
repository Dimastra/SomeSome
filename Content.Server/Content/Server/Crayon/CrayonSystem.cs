using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Decals;
using Content.Server.Nutrition.EntitySystems;
using Content.Server.Popups;
using Content.Shared.Administration.Logs;
using Content.Shared.Audio;
using Content.Shared.Crayon;
using Content.Shared.Database;
using Content.Shared.Decals;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Crayon
{
	// Token: 0x020005DC RID: 1500
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CrayonSystem : SharedCrayonSystem
	{
		// Token: 0x0600200C RID: 8204 RVA: 0x000A7610 File Offset: 0x000A5810
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CrayonComponent, ComponentInit>(new ComponentEventHandler<CrayonComponent, ComponentInit>(this.OnCrayonInit), null, null);
			base.SubscribeLocalEvent<CrayonComponent, CrayonSelectMessage>(new ComponentEventHandler<CrayonComponent, CrayonSelectMessage>(this.OnCrayonBoundUI), null, null);
			base.SubscribeLocalEvent<CrayonComponent, CrayonColorMessage>(new ComponentEventHandler<CrayonComponent, CrayonColorMessage>(this.OnCrayonBoundUIColor), null, null);
			base.SubscribeLocalEvent<CrayonComponent, UseInHandEvent>(new ComponentEventHandler<CrayonComponent, UseInHandEvent>(this.OnCrayonUse), new Type[]
			{
				typeof(FoodSystem)
			}, null);
			base.SubscribeLocalEvent<CrayonComponent, AfterInteractEvent>(new ComponentEventHandler<CrayonComponent, AfterInteractEvent>(this.OnCrayonAfterInteract), null, new Type[]
			{
				typeof(FoodSystem)
			});
			base.SubscribeLocalEvent<CrayonComponent, DroppedEvent>(new ComponentEventHandler<CrayonComponent, DroppedEvent>(this.OnCrayonDropped), null, null);
			ComponentEventRefHandler<CrayonComponent, ComponentGetState> componentEventRefHandler;
			if ((componentEventRefHandler = CrayonSystem.<>O.<0>__OnCrayonGetState) == null)
			{
				componentEventRefHandler = (CrayonSystem.<>O.<0>__OnCrayonGetState = new ComponentEventRefHandler<CrayonComponent, ComponentGetState>(CrayonSystem.OnCrayonGetState));
			}
			base.SubscribeLocalEvent<CrayonComponent, ComponentGetState>(componentEventRefHandler, null, null);
		}

		// Token: 0x0600200D RID: 8205 RVA: 0x000A76E2 File Offset: 0x000A58E2
		private static void OnCrayonGetState(EntityUid uid, CrayonComponent component, ref ComponentGetState args)
		{
			args.State = new CrayonComponentState(component.Color, component.SelectedState, component.Charges, component.Capacity);
		}

		// Token: 0x0600200E RID: 8206 RVA: 0x000A7708 File Offset: 0x000A5908
		private void OnCrayonAfterInteract(EntityUid uid, CrayonComponent component, AfterInteractEvent args)
		{
			if (args.Handled || !args.CanReach)
			{
				return;
			}
			if (component.Charges <= 0)
			{
				if (component.DeleteEmpty)
				{
					this.UseUpCrayon(uid, args.User);
				}
				else
				{
					this._popup.PopupEntity(Loc.GetString("crayon-interact-not-enough-left-text"), uid, args.User, PopupType.Small);
				}
				args.Handled = true;
				return;
			}
			if (!args.ClickLocation.IsValid(this.EntityManager))
			{
				this._popup.PopupEntity(Loc.GetString("crayon-interact-invalid-location"), uid, args.User, PopupType.Small);
				args.Handled = true;
				return;
			}
			uint num;
			if (!this._decals.TryAddDecal(component.SelectedState, args.ClickLocation.Offset(new Vector2(-0.5f, -0.5f)), out num, new Color?(component.Color), null, 0, true))
			{
				return;
			}
			if (component.UseSound != null)
			{
				SoundSystem.Play(component.UseSound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, new AudioParams?(AudioHelpers.WithVariation(0.125f)));
			}
			int charges = component.Charges;
			component.Charges = charges - 1;
			base.Dirty(component, null);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.CrayonDraw;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(9, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(args.User), "user", "EntityManager.ToPrettyString(args.User)");
			logStringHandler.AppendLiteral(" drew a ");
			logStringHandler.AppendFormatted<Color>(component.Color, "color", "component.Color");
			logStringHandler.AppendLiteral(" ");
			logStringHandler.AppendFormatted(component.SelectedState);
			adminLogger.Add(type, impact, ref logStringHandler);
			args.Handled = true;
			if (component.DeleteEmpty && component.Charges <= 0)
			{
				this.UseUpCrayon(uid, args.User);
			}
		}

		// Token: 0x0600200F RID: 8207 RVA: 0x000A78E0 File Offset: 0x000A5AE0
		private void OnCrayonUse(EntityUid uid, CrayonComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			BoundUserInterface userInterface = component.UserInterface;
			if (userInterface != null)
			{
				userInterface.Toggle(actor.PlayerSession);
			}
			BoundUserInterface userInterface2 = component.UserInterface;
			if (userInterface2 != null && userInterface2.SessionHasOpen(actor.PlayerSession))
			{
				component.UserInterface.SetState(new CrayonBoundUserInterfaceState(component.SelectedState, component.SelectableColor, component.Color), null, true);
			}
			args.Handled = true;
		}

		// Token: 0x06002010 RID: 8208 RVA: 0x000A7964 File Offset: 0x000A5B64
		private void OnCrayonBoundUI(EntityUid uid, CrayonComponent component, CrayonSelectMessage args)
		{
			DecalPrototype prototype;
			if (!this._prototypeManager.TryIndex<DecalPrototype>(args.State, ref prototype) || !prototype.Tags.Contains("crayon"))
			{
				return;
			}
			component.SelectedState = args.State;
			base.Dirty(component, null);
		}

		// Token: 0x06002011 RID: 8209 RVA: 0x000A79AD File Offset: 0x000A5BAD
		private void OnCrayonBoundUIColor(EntityUid uid, CrayonComponent component, CrayonColorMessage args)
		{
			if (component.SelectableColor && args.Color != component.Color)
			{
				component.Color = args.Color;
				base.Dirty(component, null);
			}
		}

		// Token: 0x06002012 RID: 8210 RVA: 0x000A79E0 File Offset: 0x000A5BE0
		private void OnCrayonInit(EntityUid uid, CrayonComponent component, ComponentInit args)
		{
			component.Charges = component.Capacity;
			DecalPrototype decal = this._prototypeManager.EnumeratePrototypes<DecalPrototype>().FirstOrDefault((DecalPrototype x) => x.Tags.Contains("crayon"));
			component.SelectedState = (((decal != null) ? decal.ID : null) ?? string.Empty);
			base.Dirty(component, null);
		}

		// Token: 0x06002013 RID: 8211 RVA: 0x000A7A4C File Offset: 0x000A5C4C
		private void OnCrayonDropped(EntityUid uid, CrayonComponent component, DroppedEvent args)
		{
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(args.User, ref actor))
			{
				BoundUserInterface userInterface = component.UserInterface;
				if (userInterface == null)
				{
					return;
				}
				userInterface.Close(actor.PlayerSession);
			}
		}

		// Token: 0x06002014 RID: 8212 RVA: 0x000A7A80 File Offset: 0x000A5C80
		private void UseUpCrayon(EntityUid uid, EntityUid user)
		{
			this._popup.PopupEntity(Loc.GetString("crayon-interact-used-up-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("owner", uid)
			}), user, user, PopupType.Small);
			this.EntityManager.QueueDeleteEntity(uid);
		}

		// Token: 0x040013E1 RID: 5089
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040013E2 RID: 5090
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040013E3 RID: 5091
		[Dependency]
		private readonly DecalSystem _decals;

		// Token: 0x040013E4 RID: 5092
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x02000AB9 RID: 2745
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400279F RID: 10143
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<CrayonComponent, ComponentGetState> <0>__OnCrayonGetState;
		}
	}
}
