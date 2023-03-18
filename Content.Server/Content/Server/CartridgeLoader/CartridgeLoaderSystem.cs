using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Systems;
using Content.Shared.CartridgeLoader;
using Content.Shared.Interaction;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Server.CartridgeLoader
{
	// Token: 0x020006D9 RID: 1753
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CartridgeLoaderSystem : SharedCartridgeLoaderSystem
	{
		// Token: 0x06002494 RID: 9364 RVA: 0x000BE784 File Offset: 0x000BC984
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CartridgeLoaderComponent, MapInitEvent>(new ComponentEventHandler<CartridgeLoaderComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<CartridgeLoaderComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<CartridgeLoaderComponent, DeviceNetworkPacketEvent>(this.OnPacketReceived), null, null);
			base.SubscribeLocalEvent<CartridgeLoaderComponent, AfterInteractEvent>(new ComponentEventHandler<CartridgeLoaderComponent, AfterInteractEvent>(this.OnUsed), null, null);
			base.SubscribeLocalEvent<CartridgeLoaderComponent, CartridgeLoaderUiMessage>(new ComponentEventHandler<CartridgeLoaderComponent, CartridgeLoaderUiMessage>(this.OnLoaderUiMessage), null, null);
			base.SubscribeLocalEvent<CartridgeLoaderComponent, CartridgeUiMessage>(new ComponentEventHandler<CartridgeLoaderComponent, CartridgeUiMessage>(this.OnUiMessage), null, null);
		}

		// Token: 0x06002495 RID: 9365 RVA: 0x000BE7FC File Offset: 0x000BC9FC
		[NullableContext(2)]
		public void UpdateUiState(EntityUid loaderUid, [Nullable(1)] CartridgeLoaderUiState state, IPlayerSession session = null, CartridgeLoaderComponent loader = null)
		{
			if (!base.Resolve<CartridgeLoaderComponent>(loaderUid, ref loader, true))
			{
				return;
			}
			state.ActiveUI = loader.ActiveProgram;
			state.Programs = this.GetAvailablePrograms(loaderUid, loader);
			BoundUserInterface ui = this._userInterfaceSystem.GetUiOrNull(loader.Owner, loader.UiKey, null);
			if (ui != null)
			{
				this._userInterfaceSystem.SetUiState(ui, state, session, true);
			}
		}

		// Token: 0x06002496 RID: 9366 RVA: 0x000BE860 File Offset: 0x000BCA60
		[NullableContext(2)]
		public void UpdateCartridgeUiState(EntityUid loaderUid, [Nullable(1)] BoundUserInterfaceState state, IPlayerSession session = null, CartridgeLoaderComponent loader = null)
		{
			if (!base.Resolve<CartridgeLoaderComponent>(loaderUid, ref loader, true))
			{
				return;
			}
			BoundUserInterface ui = this._userInterfaceSystem.GetUiOrNull(loader.Owner, loader.UiKey, null);
			if (ui != null)
			{
				this._userInterfaceSystem.SetUiState(ui, state, session, true);
			}
		}

		// Token: 0x06002497 RID: 9367 RVA: 0x000BE8A8 File Offset: 0x000BCAA8
		public List<EntityUid> GetAvailablePrograms(EntityUid uid, [Nullable(2)] CartridgeLoaderComponent loader = null)
		{
			if (!base.Resolve<CartridgeLoaderComponent>(uid, ref loader, true))
			{
				return new List<EntityUid>();
			}
			if (loader.CartridgeSlot.HasItem)
			{
				EntityPrototype entityPrototype = base.Prototype(loader.CartridgeSlot.Item.Value, null);
				if (this.IsInstalled((entityPrototype != null) ? entityPrototype.ID : null, loader))
				{
					return loader.InstalledPrograms;
				}
			}
			List<EntityUid> available = new List<EntityUid>();
			available.AddRange(loader.InstalledPrograms);
			if (loader.CartridgeSlot.HasItem)
			{
				available.Add(loader.CartridgeSlot.Item.Value);
			}
			return available;
		}

		// Token: 0x06002498 RID: 9368 RVA: 0x000BE944 File Offset: 0x000BCB44
		[NullableContext(2)]
		public bool InstallCartridge(EntityUid loaderUid, EntityUid cartridgeUid, CartridgeLoaderComponent loader = null)
		{
			if (!base.Resolve<CartridgeLoaderComponent>(loaderUid, ref loader, true) || loader.InstalledPrograms.Count >= loader.DiskSpace)
			{
				return false;
			}
			EntityPrototype entityPrototype = base.Prototype(cartridgeUid, null);
			string prototypeId = (entityPrototype != null) ? entityPrototype.ID : null;
			return prototypeId != null && this.InstallProgram(loaderUid, prototypeId, true, loader);
		}

		// Token: 0x06002499 RID: 9369 RVA: 0x000BE998 File Offset: 0x000BCB98
		public bool InstallProgram(EntityUid loaderUid, string prototype, bool deinstallable = true, [Nullable(2)] CartridgeLoaderComponent loader = null)
		{
			if (!base.Resolve<CartridgeLoaderComponent>(loaderUid, ref loader, true) || loader.InstalledPrograms.Count >= loader.DiskSpace)
			{
				return false;
			}
			IContainer container;
			if (!this._containerSystem.TryGetContainer(loaderUid, "program-container", ref container, null))
			{
				return false;
			}
			if (this.IsInstalled(prototype, loader))
			{
				return false;
			}
			EntityUid installedProgram = base.Spawn(prototype, new EntityCoordinates(loaderUid, 0f, 0f));
			if (container != null)
			{
				container.Insert(installedProgram, null, null, null, null, null);
			}
			this.UpdateCartridgeInstallationStatus(installedProgram, deinstallable ? InstallationStatus.Installed : InstallationStatus.Readonly, null);
			loader.InstalledPrograms.Add(installedProgram);
			base.RaiseLocalEvent<CartridgeAddedEvent>(installedProgram, new CartridgeAddedEvent(loaderUid), false);
			this.UpdateUserInterfaceState(loaderUid, loader);
			return true;
		}

		// Token: 0x0600249A RID: 9370 RVA: 0x000BEA4C File Offset: 0x000BCC4C
		[NullableContext(2)]
		public bool UninstallProgram(EntityUid loaderUid, EntityUid programUid, CartridgeLoaderComponent loader = null)
		{
			if (!base.Resolve<CartridgeLoaderComponent>(loaderUid, ref loader, true) || !this.ContainsCartridge(programUid, loader, true))
			{
				return false;
			}
			EntityUid? activeProgram = loader.ActiveProgram;
			if (activeProgram != null && (activeProgram == null || activeProgram.GetValueOrDefault() == programUid))
			{
				loader.ActiveProgram = null;
			}
			loader.BackgroundPrograms.Remove(programUid);
			loader.InstalledPrograms.Remove(programUid);
			this.EntityManager.QueueDeleteEntity(programUid);
			this.UpdateUserInterfaceState(loaderUid, loader);
			return true;
		}

		// Token: 0x0600249B RID: 9371 RVA: 0x000BEAE0 File Offset: 0x000BCCE0
		[NullableContext(2)]
		public void ActivateProgram(EntityUid loaderUid, EntityUid programUid, CartridgeLoaderComponent loader = null)
		{
			if (!base.Resolve<CartridgeLoaderComponent>(loaderUid, ref loader, true))
			{
				return;
			}
			if (!this.ContainsCartridge(programUid, loader, false))
			{
				return;
			}
			if (loader.ActiveProgram != null)
			{
				this.DeactivateProgram(loaderUid, programUid, loader);
			}
			if (!loader.BackgroundPrograms.Contains(programUid))
			{
				base.RaiseLocalEvent<CartridgeActivatedEvent>(programUid, new CartridgeActivatedEvent(loaderUid), false);
			}
			loader.ActiveProgram = new EntityUid?(programUid);
			this.UpdateUserInterfaceState(loaderUid, loader);
		}

		// Token: 0x0600249C RID: 9372 RVA: 0x000BEB4C File Offset: 0x000BCD4C
		[NullableContext(2)]
		public void DeactivateProgram(EntityUid loaderUid, EntityUid programUid, CartridgeLoaderComponent loader = null)
		{
			if (!base.Resolve<CartridgeLoaderComponent>(loaderUid, ref loader, true))
			{
				return;
			}
			if (this.ContainsCartridge(programUid, loader, false))
			{
				EntityUid? activeProgram = loader.ActiveProgram;
				if (activeProgram != null && (activeProgram == null || !(activeProgram.GetValueOrDefault() != programUid)))
				{
					if (!loader.BackgroundPrograms.Contains(programUid))
					{
						base.RaiseLocalEvent<CartridgeDeactivatedEvent>(programUid, new CartridgeDeactivatedEvent(programUid), false);
					}
					loader.ActiveProgram = null;
					this.UpdateUserInterfaceState(loaderUid, loader);
					return;
				}
			}
		}

		// Token: 0x0600249D RID: 9373 RVA: 0x000BEBD4 File Offset: 0x000BCDD4
		[NullableContext(2)]
		public void RegisterBackgroundProgram(EntityUid loaderUid, EntityUid cartridgeUid, CartridgeLoaderComponent loader = null)
		{
			if (!base.Resolve<CartridgeLoaderComponent>(loaderUid, ref loader, true))
			{
				return;
			}
			if (!this.ContainsCartridge(cartridgeUid, loader, false))
			{
				return;
			}
			EntityUid? activeProgram = loader.ActiveProgram;
			if (activeProgram == null || (activeProgram != null && activeProgram.GetValueOrDefault() != cartridgeUid))
			{
				base.RaiseLocalEvent<CartridgeActivatedEvent>(cartridgeUid, new CartridgeActivatedEvent(loaderUid), false);
			}
			loader.BackgroundPrograms.Add(cartridgeUid);
		}

		// Token: 0x0600249E RID: 9374 RVA: 0x000BEC44 File Offset: 0x000BCE44
		[NullableContext(2)]
		public void UnregisterBackgroundProgram(EntityUid loaderUid, EntityUid cartridgeUid, CartridgeLoaderComponent loader = null)
		{
			if (!base.Resolve<CartridgeLoaderComponent>(loaderUid, ref loader, true))
			{
				return;
			}
			if (!this.ContainsCartridge(cartridgeUid, loader, false))
			{
				return;
			}
			EntityUid? activeProgram = loader.ActiveProgram;
			if (activeProgram == null || (activeProgram != null && activeProgram.GetValueOrDefault() != cartridgeUid))
			{
				base.RaiseLocalEvent<CartridgeDeactivatedEvent>(cartridgeUid, new CartridgeDeactivatedEvent(loaderUid), false);
			}
			loader.BackgroundPrograms.Remove(cartridgeUid);
		}

		// Token: 0x0600249F RID: 9375 RVA: 0x000BECB5 File Offset: 0x000BCEB5
		protected override void OnItemInserted(EntityUid uid, CartridgeLoaderComponent loader, EntInsertedIntoContainerMessage args)
		{
			base.RaiseLocalEvent<CartridgeAddedEvent>(args.Entity, new CartridgeAddedEvent(uid), false);
			base.OnItemInserted(uid, loader, args);
		}

		// Token: 0x060024A0 RID: 9376 RVA: 0x000BECD4 File Offset: 0x000BCED4
		protected override void OnItemRemoved(EntityUid uid, CartridgeLoaderComponent loader, EntRemovedFromContainerMessage args)
		{
			bool deactivate = loader.BackgroundPrograms.Remove(args.Entity);
			EntityUid? activeProgram = loader.ActiveProgram;
			EntityUid entity = args.Entity;
			if (activeProgram != null && (activeProgram == null || activeProgram.GetValueOrDefault() == entity))
			{
				loader.ActiveProgram = null;
				deactivate = true;
			}
			if (deactivate)
			{
				base.RaiseLocalEvent<CartridgeDeactivatedEvent>(args.Entity, new CartridgeDeactivatedEvent(uid), false);
			}
			base.RaiseLocalEvent<CartridgeRemovedEvent>(args.Entity, new CartridgeRemovedEvent(uid), false);
			base.OnItemRemoved(uid, loader, args);
		}

		// Token: 0x060024A1 RID: 9377 RVA: 0x000BED68 File Offset: 0x000BCF68
		private void OnMapInit(EntityUid uid, CartridgeLoaderComponent component, MapInitEvent args)
		{
			foreach (string prototype in component.PreinstalledPrograms)
			{
				this.InstallProgram(uid, prototype, false, null);
			}
		}

		// Token: 0x060024A2 RID: 9378 RVA: 0x000BEDC0 File Offset: 0x000BCFC0
		private void OnUsed(EntityUid uid, CartridgeLoaderComponent component, AfterInteractEvent args)
		{
			this.RelayEvent<CartridgeAfterInteractEvent>(component, new CartridgeAfterInteractEvent(uid, args), false);
		}

		// Token: 0x060024A3 RID: 9379 RVA: 0x000BEDD1 File Offset: 0x000BCFD1
		private void OnPacketReceived(EntityUid uid, CartridgeLoaderComponent component, DeviceNetworkPacketEvent args)
		{
			this.RelayEvent<CartridgeDeviceNetPacketEvent>(component, new CartridgeDeviceNetPacketEvent(uid, args), false);
		}

		// Token: 0x060024A4 RID: 9380 RVA: 0x000BEDE4 File Offset: 0x000BCFE4
		private void OnLoaderUiMessage(EntityUid loaderUid, CartridgeLoaderComponent component, CartridgeLoaderUiMessage message)
		{
			switch (message.Action)
			{
			case CartridgeUiMessageAction.Activate:
				this.ActivateProgram(loaderUid, message.CartridgeUid, component);
				return;
			case CartridgeUiMessageAction.Deactivate:
				this.DeactivateProgram(loaderUid, message.CartridgeUid, component);
				return;
			case CartridgeUiMessageAction.Install:
				this.InstallCartridge(loaderUid, message.CartridgeUid, component);
				return;
			case CartridgeUiMessageAction.Uninstall:
				this.UninstallProgram(loaderUid, message.CartridgeUid, component);
				return;
			case CartridgeUiMessageAction.UIReady:
				if (component.ActiveProgram != null)
				{
					base.RaiseLocalEvent<CartridgeUiReadyEvent>(component.ActiveProgram.Value, new CartridgeUiReadyEvent(loaderUid), false);
					return;
				}
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x060024A5 RID: 9381 RVA: 0x000BEE80 File Offset: 0x000BD080
		private void OnUiMessage(EntityUid uid, CartridgeLoaderComponent component, CartridgeUiMessage args)
		{
			CartridgeMessageEvent cartridgeEvent = args.MessageEvent;
			cartridgeEvent.LoaderUid = uid;
			this.RelayEvent<CartridgeMessageEvent>(component, cartridgeEvent, true);
		}

		// Token: 0x060024A6 RID: 9382 RVA: 0x000BEEA4 File Offset: 0x000BD0A4
		private void RelayEvent<TEvent>(CartridgeLoaderComponent loader, TEvent args, bool skipBackgroundPrograms = false)
		{
			if (loader.ActiveProgram != null)
			{
				base.RaiseLocalEvent<TEvent>(loader.ActiveProgram.Value, args, false);
			}
			if (skipBackgroundPrograms)
			{
				return;
			}
			foreach (EntityUid program in loader.BackgroundPrograms)
			{
				if (loader.ActiveProgram == null || !loader.ActiveProgram.Value.Equals(program))
				{
					base.RaiseLocalEvent<TEvent>(program, args, false);
				}
			}
		}

		// Token: 0x060024A7 RID: 9383 RVA: 0x000BEF40 File Offset: 0x000BD140
		private bool IsInstalled([Nullable(2)] string prototype, CartridgeLoaderComponent loader)
		{
			foreach (EntityUid program in loader.InstalledPrograms)
			{
				EntityPrototype entityPrototype = base.Prototype(program, null);
				if (((entityPrototype != null) ? entityPrototype.ID : null) == prototype)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060024A8 RID: 9384 RVA: 0x000BEFB0 File Offset: 0x000BD1B0
		private void UpdateUserInterfaceState(EntityUid loaderUid, CartridgeLoaderComponent loader)
		{
			this.UpdateUiState(loaderUid, new CartridgeLoaderUiState(), null, loader);
		}

		// Token: 0x060024A9 RID: 9385 RVA: 0x000BEFC0 File Offset: 0x000BD1C0
		[NullableContext(2)]
		private void UpdateCartridgeInstallationStatus(EntityUid cartridgeUid, InstallationStatus installationStatus, CartridgeComponent cartridgeComponent = null)
		{
			if (base.Resolve<CartridgeComponent>(cartridgeUid, ref cartridgeComponent, true))
			{
				cartridgeComponent.InstallationStatus = installationStatus;
				base.Dirty(cartridgeComponent, null);
			}
		}

		// Token: 0x060024AA RID: 9386 RVA: 0x000BEFE0 File Offset: 0x000BD1E0
		private bool ContainsCartridge(EntityUid cartridgeUid, CartridgeLoaderComponent loader, bool onlyInstalled = false)
		{
			EntityUid? entityUid;
			return (!onlyInstalled && loader.CartridgeSlot.Item != null && entityUid.GetValueOrDefault().Equals(cartridgeUid)) || loader.InstalledPrograms.Contains(cartridgeUid);
		}

		// Token: 0x04001681 RID: 5761
		[Dependency]
		private readonly ContainerSystem _containerSystem;

		// Token: 0x04001682 RID: 5762
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x04001683 RID: 5763
		private const string ContainerName = "program-container";
	}
}
