using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Body.Components;
using Content.Server.DoAfter;
using Content.Server.Hands.Systems;
using Content.Server.Popups;
using Content.Shared.Alert;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Body.Systems
{
	// Token: 0x02000708 RID: 1800
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InternalsSystem : EntitySystem
	{
		// Token: 0x060025E6 RID: 9702 RVA: 0x000C7B6C File Offset: 0x000C5D6C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<InternalsComponent, InhaleLocationEvent>(new ComponentEventHandler<InternalsComponent, InhaleLocationEvent>(this.OnInhaleLocation), null, null);
			base.SubscribeLocalEvent<InternalsComponent, ComponentStartup>(new ComponentEventHandler<InternalsComponent, ComponentStartup>(this.OnInternalsStartup), null, null);
			base.SubscribeLocalEvent<InternalsComponent, ComponentShutdown>(new ComponentEventHandler<InternalsComponent, ComponentShutdown>(this.OnInternalsShutdown), null, null);
			base.SubscribeLocalEvent<InternalsComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<InternalsComponent, GetVerbsEvent<InteractionVerb>>(this.OnGetInteractionVerbs), null, null);
			base.SubscribeLocalEvent<InternalsComponent, DoAfterEvent<InternalsSystem.InternalsData>>(new ComponentEventHandler<InternalsComponent, DoAfterEvent<InternalsSystem.InternalsData>>(this.OnDoAfter), null, null);
		}

		// Token: 0x060025E7 RID: 9703 RVA: 0x000C7BE4 File Offset: 0x000C5DE4
		private void OnGetInteractionVerbs(EntityUid uid, InternalsComponent component, GetVerbsEvent<InteractionVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || args.Hands == null)
			{
				return;
			}
			InteractionVerb verb = new InteractionVerb
			{
				Act = delegate()
				{
					this.ToggleInternals(uid, args.User, false, component);
				},
				Message = Loc.GetString("action-description-internals-toggle"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/dot.svg.192dpi.png", "/")),
				Text = Loc.GetString("action-name-internals-toggle")
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x060025E8 RID: 9704 RVA: 0x000C7CA0 File Offset: 0x000C5EA0
		[NullableContext(2)]
		public void ToggleInternals(EntityUid uid, EntityUid user, bool force, InternalsComponent internals = null)
		{
			if (!base.Resolve<InternalsComponent>(uid, ref internals, false))
			{
				return;
			}
			if (this.AreInternalsWorking(internals))
			{
				this.DisconnectTank(internals);
				return;
			}
			EntityUid? breathToolEntity = internals.BreathToolEntity;
			if (breathToolEntity == null)
			{
				this._popupSystem.PopupEntity(Loc.GetString("internals-no-breath-tool"), uid, user, PopupType.Small);
				return;
			}
			GasTankComponent tank = this.FindBestGasTank(uid, internals);
			if (tank == null)
			{
				this._popupSystem.PopupEntity(Loc.GetString("internals-no-tank"), uid, user, PopupType.Small);
				return;
			}
			bool isUser = uid == user;
			InternalsSystem.InternalsData internalsData = default(InternalsSystem.InternalsData);
			if (!force)
			{
				float delay = (!isUser) ? internals.Delay : 1f;
				SharedDoAfterSystem doAfter = this._doAfter;
				float delay2 = delay;
				breathToolEntity = new EntityUid?(uid);
				doAfter.DoAfter<InternalsSystem.InternalsData>(new DoAfterEventArgs(user, delay2, default(CancellationToken), breathToolEntity, null)
				{
					BreakOnUserMove = true,
					BreakOnDamage = true,
					BreakOnStun = true,
					BreakOnTargetMove = true,
					MovementThreshold = 0.1f,
					RaiseOnUser = isUser,
					RaiseOnTarget = !isUser
				}, internalsData);
				return;
			}
			this._gasTank.ConnectToInternals(tank);
		}

		// Token: 0x060025E9 RID: 9705 RVA: 0x000C7DB9 File Offset: 0x000C5FB9
		private void OnDoAfter(EntityUid uid, InternalsComponent component, DoAfterEvent<InternalsSystem.InternalsData> args)
		{
			if (args.Cancelled || args.Handled)
			{
				return;
			}
			this.ToggleInternals(uid, args.Args.User, true, component);
			args.Handled = true;
		}

		// Token: 0x060025EA RID: 9706 RVA: 0x000C7DE8 File Offset: 0x000C5FE8
		private void OnInternalsStartup(EntityUid uid, InternalsComponent component, ComponentStartup args)
		{
			this._alerts.ShowAlert(uid, AlertType.Internals, new short?(this.GetSeverity(component)), null);
		}

		// Token: 0x060025EB RID: 9707 RVA: 0x000C7E18 File Offset: 0x000C6018
		private void OnInternalsShutdown(EntityUid uid, InternalsComponent component, ComponentShutdown args)
		{
			this._alerts.ClearAlert(uid, AlertType.Internals);
		}

		// Token: 0x060025EC RID: 9708 RVA: 0x000C7E28 File Offset: 0x000C6028
		private void OnInhaleLocation(EntityUid uid, InternalsComponent component, InhaleLocationEvent args)
		{
			if (this.AreInternalsWorking(component))
			{
				GasTankComponent gasTank = base.Comp<GasTankComponent>(component.GasTankEntity.Value);
				args.Gas = this._gasTank.RemoveAirVolume(gasTank, 0.5f);
				this._alerts.ShowAlert(uid, AlertType.Internals, new short?(this.GetSeverity(component)), null);
			}
		}

		// Token: 0x060025ED RID: 9709 RVA: 0x000C7E90 File Offset: 0x000C6090
		public void DisconnectBreathTool(InternalsComponent component)
		{
			EntityUid? old = component.BreathToolEntity;
			component.BreathToolEntity = null;
			BreathToolComponent breathTool;
			if (base.TryComp<BreathToolComponent>(old, ref breathTool))
			{
				this._atmos.DisconnectInternals(breathTool);
				this.DisconnectTank(component);
			}
			this._alerts.ShowAlert(component.Owner, AlertType.Internals, new short?(this.GetSeverity(component)), null);
		}

		// Token: 0x060025EE RID: 9710 RVA: 0x000C7EFC File Offset: 0x000C60FC
		public void ConnectBreathTool(InternalsComponent component, EntityUid toolEntity)
		{
			BreathToolComponent tool;
			if (base.TryComp<BreathToolComponent>(component.BreathToolEntity, ref tool))
			{
				this._atmos.DisconnectInternals(tool);
			}
			component.BreathToolEntity = new EntityUid?(toolEntity);
			this._alerts.ShowAlert(component.Owner, AlertType.Internals, new short?(this.GetSeverity(component)), null);
		}

		// Token: 0x060025EF RID: 9711 RVA: 0x000C7F5C File Offset: 0x000C615C
		[NullableContext(2)]
		public void DisconnectTank(InternalsComponent component)
		{
			if (component == null)
			{
				return;
			}
			GasTankComponent tank;
			if (base.TryComp<GasTankComponent>(component.GasTankEntity, ref tank))
			{
				this._gasTank.DisconnectFromInternals(tank);
			}
			component.GasTankEntity = null;
			this._alerts.ShowAlert(component.Owner, AlertType.Internals, new short?(this.GetSeverity(component)), null);
		}

		// Token: 0x060025F0 RID: 9712 RVA: 0x000C7FC0 File Offset: 0x000C61C0
		public bool TryConnectTank(InternalsComponent component, EntityUid tankEntity)
		{
			if (component.BreathToolEntity == null)
			{
				return false;
			}
			GasTankComponent tank;
			if (base.TryComp<GasTankComponent>(component.GasTankEntity, ref tank))
			{
				this._gasTank.DisconnectFromInternals(tank);
			}
			component.GasTankEntity = new EntityUid?(tankEntity);
			this._alerts.ShowAlert(component.Owner, AlertType.Internals, new short?(this.GetSeverity(component)), null);
			return true;
		}

		// Token: 0x060025F1 RID: 9713 RVA: 0x000C8030 File Offset: 0x000C6230
		public bool AreInternalsWorking(InternalsComponent component)
		{
			BreathToolComponent breathTool;
			GasTankComponent gasTankComponent;
			return base.TryComp<BreathToolComponent>(component.BreathToolEntity, ref breathTool) && breathTool.IsFunctional && base.TryComp<GasTankComponent>(component.GasTankEntity, ref gasTankComponent);
		}

		// Token: 0x060025F2 RID: 9714 RVA: 0x000C8068 File Offset: 0x000C6268
		private short GetSeverity(InternalsComponent component)
		{
			if (component.BreathToolEntity == null || !this.AreInternalsWorking(component))
			{
				return 2;
			}
			GasTankComponent gasTank;
			if (base.TryComp<GasTankComponent>(component.GasTankEntity, ref gasTank) && gasTank.IsLowPressure)
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x060025F3 RID: 9715 RVA: 0x000C80AC File Offset: 0x000C62AC
		[return: Nullable(2)]
		public GasTankComponent FindBestGasTank(EntityUid internalsOwner, InternalsComponent component)
		{
			InventoryComponent inventory = null;
			ContainerManagerComponent containerManager = null;
			EntityUid? backEntity;
			GasTankComponent backGasTank;
			if (this._inventory.TryGetSlotEntity(internalsOwner, "back", out backEntity, inventory, containerManager) && base.TryComp<GasTankComponent>(backEntity, ref backGasTank) && this._gasTank.CanConnectToInternals(backGasTank))
			{
				return backGasTank;
			}
			EntityUid? entity;
			GasTankComponent gasTank;
			if (this._inventory.TryGetSlotEntity(internalsOwner, "suitstorage", out entity, inventory, containerManager) && base.TryComp<GasTankComponent>(entity, ref gasTank) && this._gasTank.CanConnectToInternals(gasTank))
			{
				return gasTank;
			}
			List<GasTankComponent> tanks = new List<GasTankComponent>();
			foreach (Hand hand in this._hands.EnumerateHands(internalsOwner, null))
			{
				if (base.TryComp<GasTankComponent>(hand.HeldEntity, ref gasTank) && this._gasTank.CanConnectToInternals(gasTank))
				{
					tanks.Add(gasTank);
				}
			}
			if (tanks.Count > 0)
			{
				tanks.Sort((GasTankComponent x, GasTankComponent y) => y.Air.TotalMoles.CompareTo(x.Air.TotalMoles));
				return tanks[0];
			}
			if (base.Resolve<InventoryComponent>(internalsOwner, ref inventory, false))
			{
				InventorySystem.ContainerSlotEnumerator enumerator = new InventorySystem.ContainerSlotEnumerator(internalsOwner, inventory.TemplateId, this._protoManager, this._inventory, SlotFlags.BELT | SlotFlags.POCKET);
				ContainerSlot container;
				while (enumerator.MoveNext(out container))
				{
					if (base.TryComp<GasTankComponent>(container.ContainedEntity, ref gasTank) && this._gasTank.CanConnectToInternals(gasTank))
					{
						tanks.Add(gasTank);
					}
				}
				if (tanks.Count > 0)
				{
					tanks.Sort((GasTankComponent x, GasTankComponent y) => y.Air.TotalMoles.CompareTo(x.Air.TotalMoles));
					return tanks[0];
				}
			}
			return null;
		}

		// Token: 0x04001764 RID: 5988
		[Dependency]
		private readonly IPrototypeManager _protoManager;

		// Token: 0x04001765 RID: 5989
		[Dependency]
		private readonly AlertsSystem _alerts;

		// Token: 0x04001766 RID: 5990
		[Dependency]
		private readonly AtmosphereSystem _atmos;

		// Token: 0x04001767 RID: 5991
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x04001768 RID: 5992
		[Dependency]
		private readonly GasTankSystem _gasTank;

		// Token: 0x04001769 RID: 5993
		[Dependency]
		private readonly HandsSystem _hands;

		// Token: 0x0400176A RID: 5994
		[Dependency]
		private readonly InventorySystem _inventory;

		// Token: 0x0400176B RID: 5995
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x02000B0A RID: 2826
		[NullableContext(0)]
		private struct InternalsData : IEquatable<InternalsSystem.InternalsData>
		{
			// Token: 0x060036D1 RID: 14033 RVA: 0x001220F8 File Offset: 0x001202F8
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("InternalsData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x060036D2 RID: 14034 RVA: 0x00122144 File Offset: 0x00120344
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				return false;
			}

			// Token: 0x060036D3 RID: 14035 RVA: 0x00122147 File Offset: 0x00120347
			[CompilerGenerated]
			public static bool operator !=(InternalsSystem.InternalsData left, InternalsSystem.InternalsData right)
			{
				return !(left == right);
			}

			// Token: 0x060036D4 RID: 14036 RVA: 0x00122153 File Offset: 0x00120353
			[CompilerGenerated]
			public static bool operator ==(InternalsSystem.InternalsData left, InternalsSystem.InternalsData right)
			{
				return left.Equals(right);
			}

			// Token: 0x060036D5 RID: 14037 RVA: 0x0012215D File Offset: 0x0012035D
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return 0;
			}

			// Token: 0x060036D6 RID: 14038 RVA: 0x00122160 File Offset: 0x00120360
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is InternalsSystem.InternalsData && this.Equals((InternalsSystem.InternalsData)obj);
			}

			// Token: 0x060036D7 RID: 14039 RVA: 0x00122178 File Offset: 0x00120378
			[CompilerGenerated]
			public readonly bool Equals(InternalsSystem.InternalsData other)
			{
				return true;
			}
		}
	}
}
