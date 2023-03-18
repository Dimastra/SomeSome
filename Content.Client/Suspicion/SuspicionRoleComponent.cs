using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Suspicion;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Client.Suspicion
{
	// Token: 0x02000100 RID: 256
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class SuspicionRoleComponent : SharedSuspicionRoleComponent
	{
		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000730 RID: 1840 RVA: 0x00025E30 File Offset: 0x00024030
		// (set) Token: 0x06000731 RID: 1841 RVA: 0x00025E38 File Offset: 0x00024038
		public string Role
		{
			get
			{
				return this._role;
			}
			set
			{
				if (this._role == value)
				{
					return;
				}
				this._role = value;
				SuspicionGui gui = this._gui;
				if (gui != null)
				{
					gui.UpdateLabel();
				}
				base.Dirty(null);
			}
		}

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000732 RID: 1842 RVA: 0x00025E68 File Offset: 0x00024068
		// (set) Token: 0x06000733 RID: 1843 RVA: 0x00025E70 File Offset: 0x00024070
		public bool? Antagonist
		{
			get
			{
				return this._antagonist;
			}
			set
			{
				bool? antagonist = this._antagonist;
				bool? flag = value;
				if (antagonist.GetValueOrDefault() == flag.GetValueOrDefault() & antagonist != null == (flag != null))
				{
					return;
				}
				this._antagonist = value;
				SuspicionGui gui = this._gui;
				if (gui != null)
				{
					gui.UpdateLabel();
				}
				if (value.GetValueOrDefault())
				{
					this.AddTraitorOverlay();
				}
				base.Dirty(null);
			}
		}

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000734 RID: 1844 RVA: 0x00025ED8 File Offset: 0x000240D8
		[TupleElementNames(new string[]
		{
			"name",
			"uid"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		[ViewVariables]
		public List<ValueTuple<string, EntityUid>> Allies { [return: TupleElementNames(new string[]
		{
			"name",
			"uid"
		})] [return: Nullable(new byte[]
		{
			1,
			0,
			1
		})] get; } = new List<ValueTuple<string, EntityUid>>();

		// Token: 0x06000735 RID: 1845 RVA: 0x00025EE0 File Offset: 0x000240E0
		private void AddTraitorOverlay()
		{
			if (this._overlayManager.HasOverlay<TraitorOverlay>())
			{
				return;
			}
			this._overlayActive = true;
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			TraitorOverlay traitorOverlay = new TraitorOverlay(entityManager, IoCManager.Resolve<IPlayerManager>(), this._resourceCache, entityManager.System<EntityLookupSystem>());
			this._overlayManager.AddOverlay(traitorOverlay);
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x00025F2D File Offset: 0x0002412D
		private void RemoveTraitorOverlay()
		{
			if (!this._overlayActive)
			{
				return;
			}
			this._overlayManager.RemoveOverlay<TraitorOverlay>();
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x00025F44 File Offset: 0x00024144
		public override void HandleComponentState(ComponentState curState, ComponentState nextState)
		{
			base.HandleComponentState(curState, nextState);
			SuspicionRoleComponentState suspicionRoleComponentState = curState as SuspicionRoleComponentState;
			if (suspicionRoleComponentState == null)
			{
				return;
			}
			this.Role = suspicionRoleComponentState.Role;
			this.Antagonist = suspicionRoleComponentState.Antagonist;
			this.Allies.Clear();
			this.Allies.AddRange(suspicionRoleComponentState.Allies);
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x00025F98 File Offset: 0x00024198
		public void RemoveUI()
		{
			SuspicionGui gui = this._gui;
			if (gui != null)
			{
				Control parent = gui.Parent;
				if (parent != null)
				{
					parent.RemoveChild(this._gui);
				}
			}
			this.RemoveTraitorOverlay();
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x00025FC4 File Offset: 0x000241C4
		public void AddUI()
		{
			UIScreen activeScreen = this._ui.ActiveScreen;
			this._gui = ((activeScreen != null) ? activeScreen.GetOrAddWidget<SuspicionGui>() : null);
			this._gui.UpdateLabel();
			LayoutContainer.SetAnchorAndMarginPreset(this._gui, 2, 0, 0);
			if (this._antagonist.GetValueOrDefault())
			{
				this.AddTraitorOverlay();
			}
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x0002601A File Offset: 0x0002421A
		protected override void OnRemove()
		{
			base.OnRemove();
			SuspicionGui gui = this._gui;
			if (gui != null)
			{
				gui.Dispose();
			}
			this.RemoveTraitorOverlay();
		}

		// Token: 0x0400034F RID: 847
		[Nullable(1)]
		[Dependency]
		private readonly IOverlayManager _overlayManager;

		// Token: 0x04000350 RID: 848
		[Nullable(1)]
		[Dependency]
		private readonly IResourceCache _resourceCache;

		// Token: 0x04000351 RID: 849
		[Nullable(1)]
		[Dependency]
		private readonly IUserInterfaceManager _ui;

		// Token: 0x04000352 RID: 850
		private SuspicionGui _gui;

		// Token: 0x04000353 RID: 851
		private string _role;

		// Token: 0x04000354 RID: 852
		private bool? _antagonist;

		// Token: 0x04000355 RID: 853
		private bool _overlayActive;
	}
}
