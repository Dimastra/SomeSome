using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Chat.Managers;
using Content.Server.Mind.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.UserInterface;
using Content.Shared.Administration.Logs;
using Content.Shared.AME;
using Content.Shared.Database;
using Content.Shared.Hands.EntitySystems;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.AME.Components
{
	// Token: 0x020007D6 RID: 2006
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class AMEControllerComponent : SharedAMEControllerComponent
	{
		// Token: 0x170006B8 RID: 1720
		// (get) Token: 0x06002B93 RID: 11155 RVA: 0x000E4E03 File Offset: 0x000E3003
		[Nullable(2)]
		[ViewVariables]
		private BoundUserInterface UserInterface
		{
			[NullableContext(2)]
			get
			{
				return base.Owner.GetUIOrNull(SharedAMEControllerComponent.AMEControllerUiKey.Key);
			}
		}

		// Token: 0x170006B9 RID: 1721
		// (get) Token: 0x06002B94 RID: 11156 RVA: 0x000E4E16 File Offset: 0x000E3016
		[ViewVariables]
		public bool Injecting
		{
			get
			{
				return this._injecting;
			}
		}

		// Token: 0x170006BA RID: 1722
		// (get) Token: 0x06002B95 RID: 11157 RVA: 0x000E4E20 File Offset: 0x000E3020
		private bool Powered
		{
			get
			{
				ApcPowerReceiverComponent receiver;
				return !this._entities.TryGetComponent<ApcPowerReceiverComponent>(base.Owner, ref receiver) || receiver.Powered;
			}
		}

		// Token: 0x170006BB RID: 1723
		// (get) Token: 0x06002B96 RID: 11158 RVA: 0x000E4E4C File Offset: 0x000E304C
		[ViewVariables]
		public bool HasJar
		{
			get
			{
				return this.JarSlot.ContainedEntity != null;
			}
		}

		// Token: 0x06002B97 RID: 11159 RVA: 0x000E4E6C File Offset: 0x000E306C
		protected override void Initialize()
		{
			base.Initialize();
			if (this.UserInterface != null)
			{
				this.UserInterface.OnReceiveMessage += this.OnUiReceiveMessage;
			}
			this._entities.TryGetComponent<AppearanceComponent>(base.Owner, ref this._appearance);
			this._entities.TryGetComponent<PowerSupplierComponent>(base.Owner, ref this._powerSupplier);
			this._injecting = false;
			this.InjectionAmount = 2;
			this.JarSlot = ContainerHelpers.EnsureContainer<ContainerSlot>(base.Owner, "AMEController-fuelJarContainer", null);
		}

		// Token: 0x06002B98 RID: 11160 RVA: 0x000E4EF4 File Offset: 0x000E30F4
		internal void OnUpdate(float frameTime)
		{
			if (!this._injecting)
			{
				return;
			}
			AMENodeGroup group = this.GetAMENodeGroup();
			if (group == null)
			{
				return;
			}
			EntityUid? containedEntity = this.JarSlot.ContainedEntity;
			if (containedEntity != null)
			{
				EntityUid jar = containedEntity.GetValueOrDefault();
				if (jar.Valid)
				{
					AMEFuelContainerComponent fuelJar;
					this._entities.TryGetComponent<AMEFuelContainerComponent>(jar, ref fuelJar);
					if (fuelJar != null && this._powerSupplier != null)
					{
						if (fuelJar.FuelAmount == 0)
						{
							this.ToggleInjection();
							AMENodeGroup amenodeGroup = this.GetAMENodeGroup();
							if (amenodeGroup != null)
							{
								amenodeGroup.UpdateCoreVisuals();
							}
							this.InjectSound(false);
							return;
						}
						int fuelСonsumed = this.InjectionAmount * this.AmountFuelConsumedPerInjection;
						int availableInject;
						if (fuelJar.FuelAmount >= fuelСonsumed)
						{
							availableInject = this.InjectionAmount;
							fuelJar.FuelAmount -= fuelСonsumed;
						}
						else
						{
							availableInject = fuelJar.FuelAmount / this.AmountFuelConsumedPerInjection;
							fuelJar.FuelAmount = 0;
						}
						bool overloading;
						this._powerSupplier.MaxSupply = group.InjectFuel(availableInject, out overloading);
						this.InjectSound(overloading);
						this.UpdateUserInterface();
					}
					this._stability = group.GetTotalStability();
					this.UpdateDisplay(this._stability);
					if (this._stability <= 0)
					{
						group.ExplodeCores();
					}
					return;
				}
			}
		}

		// Token: 0x06002B99 RID: 11161 RVA: 0x000E5015 File Offset: 0x000E3215
		public void OnAMENodeGroupUpdate()
		{
			this.UpdateUserInterface();
		}

		// Token: 0x06002B9A RID: 11162 RVA: 0x000E5020 File Offset: 0x000E3220
		private SharedAMEControllerComponent.AMEControllerBoundUserInterfaceState GetUserInterfaceState()
		{
			EntityUid? containedEntity = this.JarSlot.ContainedEntity;
			if (containedEntity != null)
			{
				EntityUid jar = containedEntity.GetValueOrDefault();
				if (jar.Valid)
				{
					AMEFuelContainerComponent jarComponent = this._entities.GetComponent<AMEFuelContainerComponent>(jar);
					return new SharedAMEControllerComponent.AMEControllerBoundUserInterfaceState(this.Powered, this.IsMasterController(), this._injecting, this.HasJar, jarComponent.FuelAmount, this.InjectionAmount, this.GetCoreCount());
				}
			}
			return new SharedAMEControllerComponent.AMEControllerBoundUserInterfaceState(this.Powered, this.IsMasterController(), false, this.HasJar, 0, this.InjectionAmount, this.GetCoreCount());
		}

		// Token: 0x06002B9B RID: 11163 RVA: 0x000E50B8 File Offset: 0x000E32B8
		private bool PlayerCanUseController(EntityUid playerEntity, bool needsPower = true)
		{
			return !(playerEntity == default(EntityUid)) && (!needsPower || this.Powered);
		}

		// Token: 0x06002B9C RID: 11164 RVA: 0x000E50E8 File Offset: 0x000E32E8
		public void UpdateUserInterface()
		{
			SharedAMEControllerComponent.AMEControllerBoundUserInterfaceState state = this.GetUserInterfaceState();
			BoundUserInterface userInterface = this.UserInterface;
			if (userInterface == null)
			{
				return;
			}
			userInterface.SetState(state, null, true);
		}

		// Token: 0x06002B9D RID: 11165 RVA: 0x000E5110 File Offset: 0x000E3310
		private void OnUiReceiveMessage(ServerBoundUserInterfaceMessage obj)
		{
			EntityUid? attachedEntity = obj.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid player = attachedEntity.GetValueOrDefault();
				if (player.Valid)
				{
					SharedAMEControllerComponent.UiButtonPressedMessage msg = (SharedAMEControllerComponent.UiButtonPressedMessage)obj.Message;
					bool button = msg.Button != SharedAMEControllerComponent.UiButton.Eject;
					bool needsPower = button;
					if (!this.PlayerCanUseController(player, needsPower))
					{
						return;
					}
					switch (msg.Button)
					{
					case SharedAMEControllerComponent.UiButton.Eject:
						this.TryEject(player);
						break;
					case SharedAMEControllerComponent.UiButton.ToggleInjection:
						this.ToggleInjection();
						break;
					case SharedAMEControllerComponent.UiButton.IncreaseFuel:
						this.InjectionAmount += 2;
						this._lastPlayerIncreasedFuel = new EntityUid?(player);
						break;
					case SharedAMEControllerComponent.UiButton.DecreaseFuel:
						this.InjectionAmount = ((this.InjectionAmount > 0) ? (this.InjectionAmount -= 2) : 0);
						break;
					}
					MindComponent mindComponent;
					this._entities.TryGetComponent<MindComponent>(player, ref mindComponent);
					if (mindComponent != null)
					{
						string humanReadableState = this._injecting ? "Inject" : "Not inject";
						if (msg.Button == SharedAMEControllerComponent.UiButton.IncreaseFuel || msg.Button == SharedAMEControllerComponent.UiButton.DecreaseFuel)
						{
							ISharedAdminLogManager adminLogger = this._adminLogger;
							LogType type = LogType.Action;
							LogImpact impact = LogImpact.Extreme;
							LogStringHandler logStringHandler = new LogStringHandler(41, 3);
							logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entities.ToPrettyString(mindComponent.Owner), "player", "_entities.ToPrettyString(mindComponent.Owner)");
							logStringHandler.AppendLiteral(" has set the AME to inject ");
							logStringHandler.AppendFormatted<int>(this.InjectionAmount, "InjectionAmount");
							logStringHandler.AppendLiteral(" while set to ");
							logStringHandler.AppendFormatted(humanReadableState);
							adminLogger.Add(type, impact, ref logStringHandler);
						}
						if (msg.Button == SharedAMEControllerComponent.UiButton.ToggleInjection)
						{
							ISharedAdminLogManager adminLogger2 = this._adminLogger;
							LogType type2 = LogType.Action;
							LogImpact impact2 = LogImpact.Extreme;
							LogStringHandler logStringHandler = new LogStringHandler(20, 2);
							logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entities.ToPrettyString(mindComponent.Owner), "player", "_entities.ToPrettyString(mindComponent.Owner)");
							logStringHandler.AppendLiteral(" has set the AME to ");
							logStringHandler.AppendFormatted(humanReadableState);
							adminLogger2.Add(type2, impact2, ref logStringHandler);
						}
					}
					AMENodeGroup amenodeGroup = this.GetAMENodeGroup();
					if (amenodeGroup != null)
					{
						amenodeGroup.UpdateCoreVisuals();
					}
					this.UpdateUserInterface();
					this.ClickSound();
					return;
				}
			}
		}

		// Token: 0x06002B9E RID: 11166 RVA: 0x000E5308 File Offset: 0x000E3508
		private void TryEject(EntityUid user)
		{
			if (!this.HasJar || this._injecting)
			{
				return;
			}
			EntityUid? containedEntity = this.JarSlot.ContainedEntity;
			if (containedEntity != null)
			{
				EntityUid jar = containedEntity.GetValueOrDefault();
				if (jar.Valid)
				{
					this.JarSlot.Remove(jar, null, null, null, true, false, null, null);
					this.UpdateUserInterface();
					this._sysMan.GetEntitySystem<SharedHandsSystem>().PickupOrDrop(new EntityUid?(user), jar, true, false, null, null);
					return;
				}
			}
		}

		// Token: 0x06002B9F RID: 11167 RVA: 0x000E5394 File Offset: 0x000E3594
		private void ToggleInjection()
		{
			if (!this._injecting)
			{
				AppearanceComponent appearance = this._appearance;
				if (appearance != null)
				{
					appearance.SetData(SharedAMEControllerComponent.AMEControllerVisuals.DisplayState, "on");
				}
			}
			else
			{
				AppearanceComponent appearance2 = this._appearance;
				if (appearance2 != null)
				{
					appearance2.SetData(SharedAMEControllerComponent.AMEControllerVisuals.DisplayState, "off");
				}
				if (this._powerSupplier != null)
				{
					this._powerSupplier.MaxSupply = 0f;
				}
			}
			this._injecting = !this._injecting;
			this.UpdateUserInterface();
		}

		// Token: 0x06002BA0 RID: 11168 RVA: 0x000E5410 File Offset: 0x000E3610
		private void UpdateDisplay(int stability)
		{
			if (this._appearance == null)
			{
				return;
			}
			string state;
			this._appearance.TryGetData<string>(SharedAMEControllerComponent.AMEControllerVisuals.DisplayState, ref state);
			string newState = "on";
			string warn_message = "";
			if (stability < 50)
			{
				newState = "critical";
				warn_message = "admin-chatalert-caution-stability";
			}
			if (stability < 10)
			{
				newState = "fuck";
				warn_message = "admin-chatalert-danger-stability";
			}
			if (state != newState)
			{
				this._chatManager.SendAdminAnnouncement(Loc.GetString(warn_message, new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("lastplayer", this._entityManager.ToPrettyString(this._lastPlayerIncreasedFuel.GetValueOrDefault()))
				}));
				AppearanceComponent appearance = this._appearance;
				if (appearance == null)
				{
					return;
				}
				appearance.SetData(SharedAMEControllerComponent.AMEControllerVisuals.DisplayState, newState);
			}
		}

		// Token: 0x06002BA1 RID: 11169 RVA: 0x000E54CC File Offset: 0x000E36CC
		[NullableContext(2)]
		private AMENodeGroup GetAMENodeGroup()
		{
			NodeContainerComponent nodeContainer;
			this._entities.TryGetComponent<NodeContainerComponent>(base.Owner, ref nodeContainer);
			if (nodeContainer == null)
			{
				return null;
			}
			return (from node in nodeContainer.Nodes.Values
			select node.NodeGroup).OfType<AMENodeGroup>().FirstOrDefault<AMENodeGroup>();
		}

		// Token: 0x06002BA2 RID: 11170 RVA: 0x000E552B File Offset: 0x000E372B
		private bool IsMasterController()
		{
			AMENodeGroup amenodeGroup = this.GetAMENodeGroup();
			return ((amenodeGroup != null) ? amenodeGroup.MasterController : null) == this;
		}

		// Token: 0x06002BA3 RID: 11171 RVA: 0x000E5548 File Offset: 0x000E3748
		private int GetCoreCount()
		{
			int coreCount = 0;
			AMENodeGroup group = this.GetAMENodeGroup();
			if (group != null)
			{
				coreCount = group.CoreCount;
			}
			return coreCount;
		}

		// Token: 0x06002BA4 RID: 11172 RVA: 0x000E556C File Offset: 0x000E376C
		private void ClickSound()
		{
			SoundSystem.Play(this._clickSound.GetSound(null, null), Filter.Pvs(base.Owner, 2f, null, null, null), base.Owner, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
		}

		// Token: 0x06002BA5 RID: 11173 RVA: 0x000E55BC File Offset: 0x000E37BC
		private void InjectSound(bool overloading)
		{
			SoundSystem.Play(this._injectSound.GetSound(null, null), Filter.Pvs(base.Owner, 2f, null, null, null), base.Owner, new AudioParams?(AudioParams.Default.WithVolume(overloading ? 10f : 0f)));
		}

		// Token: 0x04001B07 RID: 6919
		[Dependency]
		private readonly IEntityManager _entities;

		// Token: 0x04001B08 RID: 6920
		[Dependency]
		private readonly IEntitySystemManager _sysMan;

		// Token: 0x04001B09 RID: 6921
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04001B0A RID: 6922
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04001B0B RID: 6923
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04001B0C RID: 6924
		private bool _injecting;

		// Token: 0x04001B0D RID: 6925
		[ViewVariables]
		public int InjectionAmount;

		// Token: 0x04001B0E RID: 6926
		[ViewVariables]
		[DataField("amountFuelConsumedPerInjection", false, 1, false, false, null)]
		public int AmountFuelConsumedPerInjection = 1;

		// Token: 0x04001B0F RID: 6927
		[Nullable(2)]
		private AppearanceComponent _appearance;

		// Token: 0x04001B10 RID: 6928
		[Nullable(2)]
		private PowerSupplierComponent _powerSupplier;

		// Token: 0x04001B11 RID: 6929
		[DataField("clickSound", false, 1, false, false, null)]
		private SoundSpecifier _clickSound = new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg", null);

		// Token: 0x04001B12 RID: 6930
		[DataField("injectSound", false, 1, false, false, null)]
		private SoundSpecifier _injectSound = new SoundPathSpecifier("/Audio/Effects/bang.ogg", null);

		// Token: 0x04001B13 RID: 6931
		[ViewVariables]
		private int _stability = 100;

		// Token: 0x04001B14 RID: 6932
		[ViewVariables]
		public EntityUid? _lastPlayerIncreasedFuel;

		// Token: 0x04001B15 RID: 6933
		public ContainerSlot JarSlot;
	}
}
