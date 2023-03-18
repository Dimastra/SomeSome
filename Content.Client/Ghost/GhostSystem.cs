using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Ghost;
using Robust.Client.Console;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Client.Ghost
{
	// Token: 0x02000305 RID: 773
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GhostSystem : SharedGhostSystem
	{
		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06001346 RID: 4934 RVA: 0x00072A41 File Offset: 0x00070C41
		// (set) Token: 0x06001347 RID: 4935 RVA: 0x00072A49 File Offset: 0x00070C49
		public int AvailableGhostRoleCount { get; private set; }

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06001348 RID: 4936 RVA: 0x00072A52 File Offset: 0x00070C52
		// (set) Token: 0x06001349 RID: 4937 RVA: 0x00072A5C File Offset: 0x00070C5C
		private bool GhostVisibility
		{
			get
			{
				return this._ghostVisibility;
			}
			set
			{
				if (this._ghostVisibility == value)
				{
					return;
				}
				this._ghostVisibility = value;
				foreach (ValueTuple<GhostComponent, SpriteComponent> valueTuple in base.EntityQuery<GhostComponent, SpriteComponent>(true))
				{
					valueTuple.Item2.Visible = true;
				}
			}
		}

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x0600134A RID: 4938 RVA: 0x00072AC0 File Offset: 0x00070CC0
		[Nullable(2)]
		public GhostComponent Player
		{
			[NullableContext(2)]
			get
			{
				LocalPlayer localPlayer = this._playerManager.LocalPlayer;
				return base.CompOrNull<GhostComponent>((localPlayer != null) ? localPlayer.ControlledEntity : null);
			}
		}

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x0600134B RID: 4939 RVA: 0x00072AF2 File Offset: 0x00070CF2
		public bool IsGhost
		{
			get
			{
				return this.Player != null;
			}
		}

		// Token: 0x1400006F RID: 111
		// (add) Token: 0x0600134C RID: 4940 RVA: 0x00072B00 File Offset: 0x00070D00
		// (remove) Token: 0x0600134D RID: 4941 RVA: 0x00072B38 File Offset: 0x00070D38
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<GhostComponent> PlayerRemoved;

		// Token: 0x14000070 RID: 112
		// (add) Token: 0x0600134E RID: 4942 RVA: 0x00072B70 File Offset: 0x00070D70
		// (remove) Token: 0x0600134F RID: 4943 RVA: 0x00072BA8 File Offset: 0x00070DA8
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<GhostComponent> PlayerUpdated;

		// Token: 0x14000071 RID: 113
		// (add) Token: 0x06001350 RID: 4944 RVA: 0x00072BE0 File Offset: 0x00070DE0
		// (remove) Token: 0x06001351 RID: 4945 RVA: 0x00072C18 File Offset: 0x00070E18
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<GhostComponent> PlayerAttached;

		// Token: 0x14000072 RID: 114
		// (add) Token: 0x06001352 RID: 4946 RVA: 0x00072C50 File Offset: 0x00070E50
		// (remove) Token: 0x06001353 RID: 4947 RVA: 0x00072C88 File Offset: 0x00070E88
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action PlayerDetached;

		// Token: 0x14000073 RID: 115
		// (add) Token: 0x06001354 RID: 4948 RVA: 0x00072CC0 File Offset: 0x00070EC0
		// (remove) Token: 0x06001355 RID: 4949 RVA: 0x00072CF8 File Offset: 0x00070EF8
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<GhostWarpsResponseEvent> GhostWarpsResponse;

		// Token: 0x14000074 RID: 116
		// (add) Token: 0x06001356 RID: 4950 RVA: 0x00072D30 File Offset: 0x00070F30
		// (remove) Token: 0x06001357 RID: 4951 RVA: 0x00072D68 File Offset: 0x00070F68
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<GhostUpdateGhostRoleCountEvent> GhostRoleCountUpdated;

		// Token: 0x06001358 RID: 4952 RVA: 0x00072DA0 File Offset: 0x00070FA0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GhostComponent, ComponentInit>(new ComponentEventHandler<GhostComponent, ComponentInit>(this.OnGhostInit), null, null);
			base.SubscribeLocalEvent<GhostComponent, ComponentRemove>(new ComponentEventHandler<GhostComponent, ComponentRemove>(this.OnGhostRemove), null, null);
			base.SubscribeLocalEvent<GhostComponent, ComponentHandleState>(new ComponentEventRefHandler<GhostComponent, ComponentHandleState>(this.OnGhostState), null, null);
			base.SubscribeLocalEvent<GhostComponent, PlayerAttachedEvent>(new ComponentEventHandler<GhostComponent, PlayerAttachedEvent>(this.OnGhostPlayerAttach), null, null);
			base.SubscribeLocalEvent<GhostComponent, PlayerDetachedEvent>(new ComponentEventHandler<GhostComponent, PlayerDetachedEvent>(this.OnGhostPlayerDetach), null, null);
			base.SubscribeLocalEvent<PlayerAttachedEvent>(new EntityEventHandler<PlayerAttachedEvent>(this.OnPlayerAttach), null, null);
			base.SubscribeNetworkEvent<GhostWarpsResponseEvent>(new EntityEventHandler<GhostWarpsResponseEvent>(this.OnGhostWarpsResponse), null, null);
			base.SubscribeNetworkEvent<GhostUpdateGhostRoleCountEvent>(new EntityEventHandler<GhostUpdateGhostRoleCountEvent>(this.OnUpdateGhostRoleCount), null, null);
			base.SubscribeLocalEvent<GhostComponent, ToggleLightingActionEvent>(new ComponentEventHandler<GhostComponent, ToggleLightingActionEvent>(this.OnToggleLighting), null, null);
			base.SubscribeLocalEvent<GhostComponent, ToggleFoVActionEvent>(new ComponentEventHandler<GhostComponent, ToggleFoVActionEvent>(this.OnToggleFoV), null, null);
			base.SubscribeLocalEvent<GhostComponent, ToggleGhostsActionEvent>(new ComponentEventHandler<GhostComponent, ToggleGhostsActionEvent>(this.OnToggleGhosts), null, null);
		}

		// Token: 0x06001359 RID: 4953 RVA: 0x00072E90 File Offset: 0x00071090
		private void OnGhostInit(EntityUid uid, GhostComponent component, ComponentInit args)
		{
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(component.Owner, ref spriteComponent))
			{
				spriteComponent.Visible = this.GhostVisibility;
			}
			this._actions.AddAction(uid, component.ToggleLightingAction, null, null, true);
			this._actions.AddAction(uid, component.ToggleFoVAction, null, null, true);
			this._actions.AddAction(uid, component.ToggleGhostsAction, null, null, true);
		}

		// Token: 0x0600135A RID: 4954 RVA: 0x00072F10 File Offset: 0x00071110
		private void OnToggleLighting(EntityUid uid, GhostComponent component, ToggleLightingActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this._lightManager.Enabled = !this._lightManager.Enabled;
			args.Handled = true;
		}

		// Token: 0x0600135B RID: 4955 RVA: 0x00072F3B File Offset: 0x0007113B
		private void OnToggleFoV(EntityUid uid, GhostComponent component, ToggleFoVActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this._eye.CurrentEye.DrawFov = !this._eye.CurrentEye.DrawFov;
			args.Handled = true;
		}

		// Token: 0x0600135C RID: 4956 RVA: 0x00072F70 File Offset: 0x00071170
		private void OnToggleGhosts(EntityUid uid, GhostComponent component, ToggleGhostsActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this.ToggleGhostVisibility();
			args.Handled = true;
		}

		// Token: 0x0600135D RID: 4957 RVA: 0x00072F88 File Offset: 0x00071188
		private void OnGhostRemove(EntityUid uid, GhostComponent component, ComponentRemove args)
		{
			this._actions.RemoveAction(uid, component.ToggleLightingAction, null);
			this._actions.RemoveAction(uid, component.ToggleFoVAction, null);
			this._actions.RemoveAction(uid, component.ToggleGhostsAction, null);
			this._lightManager.Enabled = true;
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (uid != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return;
			}
			if (component.IsAttached)
			{
				this.GhostVisibility = false;
			}
			Action<GhostComponent> playerRemoved = this.PlayerRemoved;
			if (playerRemoved == null)
			{
				return;
			}
			playerRemoved(component);
		}

		// Token: 0x0600135E RID: 4958 RVA: 0x00073038 File Offset: 0x00071238
		private void OnGhostPlayerAttach(EntityUid uid, GhostComponent component, PlayerAttachedEvent playerAttachedEvent)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (uid != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return;
			}
			this.GhostVisibility = true;
			component.IsAttached = true;
			Action<GhostComponent> playerAttached = this.PlayerAttached;
			if (playerAttached == null)
			{
				return;
			}
			playerAttached(component);
		}

		// Token: 0x0600135F RID: 4959 RVA: 0x000730A4 File Offset: 0x000712A4
		private void OnGhostState(EntityUid uid, GhostComponent component, ref ComponentHandleState args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (uid != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return;
			}
			Action<GhostComponent> playerUpdated = this.PlayerUpdated;
			if (playerUpdated == null)
			{
				return;
			}
			playerUpdated(component);
		}

		// Token: 0x06001360 RID: 4960 RVA: 0x00073100 File Offset: 0x00071300
		private bool PlayerDetach(EntityUid uid)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (uid != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return false;
			}
			this.GhostVisibility = false;
			Action playerDetached = this.PlayerDetached;
			if (playerDetached != null)
			{
				playerDetached();
			}
			return true;
		}

		// Token: 0x06001361 RID: 4961 RVA: 0x00073165 File Offset: 0x00071365
		private void OnGhostPlayerDetach(EntityUid uid, GhostComponent component, PlayerDetachedEvent args)
		{
			if (this.PlayerDetach(uid))
			{
				component.IsAttached = false;
			}
		}

		// Token: 0x06001362 RID: 4962 RVA: 0x00073177 File Offset: 0x00071377
		private void OnPlayerAttach(PlayerAttachedEvent ev)
		{
			if (!base.HasComp<GhostComponent>(ev.Entity))
			{
				this.PlayerDetach(ev.Entity);
			}
		}

		// Token: 0x06001363 RID: 4963 RVA: 0x00073194 File Offset: 0x00071394
		private void OnGhostWarpsResponse(GhostWarpsResponseEvent msg)
		{
			if (!this.IsGhost)
			{
				return;
			}
			Action<GhostWarpsResponseEvent> ghostWarpsResponse = this.GhostWarpsResponse;
			if (ghostWarpsResponse == null)
			{
				return;
			}
			ghostWarpsResponse(msg);
		}

		// Token: 0x06001364 RID: 4964 RVA: 0x000731B0 File Offset: 0x000713B0
		private void OnUpdateGhostRoleCount(GhostUpdateGhostRoleCountEvent msg)
		{
			this.AvailableGhostRoleCount = msg.AvailableGhostRoles;
			Action<GhostUpdateGhostRoleCountEvent> ghostRoleCountUpdated = this.GhostRoleCountUpdated;
			if (ghostRoleCountUpdated == null)
			{
				return;
			}
			ghostRoleCountUpdated(msg);
		}

		// Token: 0x06001365 RID: 4965 RVA: 0x000731CF File Offset: 0x000713CF
		public void RequestWarps()
		{
			base.RaiseNetworkEvent(new GhostWarpsRequestEvent());
		}

		// Token: 0x06001366 RID: 4966 RVA: 0x000731DC File Offset: 0x000713DC
		public void ReturnToBody()
		{
			GhostReturnToBodyRequest ghostReturnToBodyRequest = new GhostReturnToBodyRequest();
			base.RaiseNetworkEvent(ghostReturnToBodyRequest);
		}

		// Token: 0x06001367 RID: 4967 RVA: 0x000731F6 File Offset: 0x000713F6
		public void OpenGhostRoles()
		{
			this._console.RemoteExecuteCommand(null, "ghostroles");
		}

		// Token: 0x06001368 RID: 4968 RVA: 0x00073209 File Offset: 0x00071409
		public void ToggleGhostVisibility()
		{
			this._console.RemoteExecuteCommand(null, "toggleghosts");
		}

		// Token: 0x06001369 RID: 4969 RVA: 0x0007321C File Offset: 0x0007141C
		public void ReturnToRound()
		{
			GhostReturnToRoundRequest ghostReturnToRoundRequest = new GhostReturnToRoundRequest();
			base.RaiseNetworkEvent(ghostReturnToRoundRequest);
		}

		// Token: 0x040009A6 RID: 2470
		[Dependency]
		private readonly IClientConsoleHost _console;

		// Token: 0x040009A7 RID: 2471
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x040009A8 RID: 2472
		[Dependency]
		private readonly SharedActionsSystem _actions;

		// Token: 0x040009A9 RID: 2473
		[Dependency]
		private readonly ILightManager _lightManager;

		// Token: 0x040009AA RID: 2474
		[Dependency]
		private readonly IEyeManager _eye;

		// Token: 0x040009AC RID: 2476
		private bool _ghostVisibility = true;
	}
}
