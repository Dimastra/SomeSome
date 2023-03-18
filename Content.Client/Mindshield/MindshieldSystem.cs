using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid;
using Content.Shared.White.Mindshield;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Mindshield
{
	// Token: 0x0200022F RID: 559
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MindshieldSystem : EntitySystem
	{
		// Token: 0x17000313 RID: 787
		// (get) Token: 0x06000E5E RID: 3678 RVA: 0x00056D35 File Offset: 0x00054F35
		// (set) Token: 0x06000E5F RID: 3679 RVA: 0x00056D3D File Offset: 0x00054F3D
		public bool OverlayEnabled
		{
			get
			{
				return this._overlayEnabled;
			}
			set
			{
				this._overlayEnabled = value;
				this.DisableHud();
			}
		}

		// Token: 0x06000E60 RID: 3680 RVA: 0x00056D4C File Offset: 0x00054F4C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ShowMindShieldHudComponent, ComponentInit>(new ComponentEventHandler<ShowMindShieldHudComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<ShowMindShieldHudComponent, ComponentRemove>(new ComponentEventHandler<ShowMindShieldHudComponent, ComponentRemove>(this.OnRemove), null, null);
			base.SubscribeLocalEvent<ShowMindShieldHudComponent, PlayerAttachedEvent>(new ComponentEventHandler<ShowMindShieldHudComponent, PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<ShowMindShieldHudComponent, PlayerDetachedEvent>(new ComponentEventHandler<ShowMindShieldHudComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
		}

		// Token: 0x06000E61 RID: 3681 RVA: 0x00056DB0 File Offset: 0x00054FB0
		private void OnInit(EntityUid uid, ShowMindShieldHudComponent component, ComponentInit args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = true;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity == null || (controlledEntity != null && controlledEntity.GetValueOrDefault() != uid));
			}
			if (flag)
			{
				return;
			}
			this.OverlayEnabled = true;
		}

		// Token: 0x06000E62 RID: 3682 RVA: 0x00056E08 File Offset: 0x00055008
		private void OnRemove(EntityUid uid, ShowMindShieldHudComponent component, ComponentRemove args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = true;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity == null || (controlledEntity != null && controlledEntity.GetValueOrDefault() != uid));
			}
			if (flag)
			{
				return;
			}
			this.OverlayEnabled = false;
		}

		// Token: 0x06000E63 RID: 3683 RVA: 0x00056E5E File Offset: 0x0005505E
		private void OnPlayerAttached(EntityUid uid, ShowMindShieldHudComponent component, PlayerAttachedEvent args)
		{
			this.OverlayEnabled = true;
		}

		// Token: 0x06000E64 RID: 3684 RVA: 0x00056E67 File Offset: 0x00055067
		private void OnPlayerDetached(EntityUid uid, ShowMindShieldHudComponent component, PlayerDetachedEvent args)
		{
			this.OverlayEnabled = false;
		}

		// Token: 0x06000E65 RID: 3685 RVA: 0x00056E70 File Offset: 0x00055070
		public override void FrameUpdate(float frameTime)
		{
			base.FrameUpdate(frameTime);
			if (!this._overlayEnabled)
			{
				return;
			}
			IEnumerable<ValueTuple<HumanoidAppearanceComponent, MindShieldComponent>> enumerable = this.EntityManager.EntityQuery<HumanoidAppearanceComponent, MindShieldComponent>(false);
			List<SpriteComponent> list = new List<SpriteComponent>();
			foreach (ValueTuple<HumanoidAppearanceComponent, MindShieldComponent> valueTuple in enumerable)
			{
				MindShieldComponent item = valueTuple.Item2;
				EntityUid owner = item.Owner;
				SpriteComponent spriteComponent = base.Comp<SpriteComponent>(owner);
				list.Add(spriteComponent);
				if (!this._huds.Contains(spriteComponent))
				{
					this.AddHud(item, spriteComponent);
				}
			}
			foreach (SpriteComponent spriteComponent2 in this._huds.Except(list).ToList<SpriteComponent>())
			{
				this.RemoveHud(spriteComponent2);
			}
		}

		// Token: 0x06000E66 RID: 3686 RVA: 0x00056F58 File Offset: 0x00055158
		private void AddHud(MindShieldComponent mindShieldComponent, SpriteComponent spriteComponent)
		{
			int num;
			if (!spriteComponent.LayerMapTryGet(MindShieldComponent.LayerName, ref num, false))
			{
				num = spriteComponent.LayerMapReserveBlank(MindShieldComponent.LayerName);
			}
			spriteComponent.LayerSetRSI(num, this._hudPath);
			spriteComponent.LayerSetState(num, this._state);
			spriteComponent.LayerSetShader(num, this._shader, null);
			this._huds.Add(spriteComponent);
		}

		// Token: 0x06000E67 RID: 3687 RVA: 0x00056FBC File Offset: 0x000551BC
		private void RemoveHud(SpriteComponent spriteComponent)
		{
			if (!this._huds.Contains(spriteComponent))
			{
				return;
			}
			int num;
			if (!spriteComponent.LayerMapTryGet(MindShieldComponent.LayerName, ref num, false))
			{
				return;
			}
			if (base.HasComp<TransformComponent>(spriteComponent.Owner))
			{
				spriteComponent.RemoveLayer(num);
			}
			this._huds.Remove(spriteComponent);
		}

		// Token: 0x06000E68 RID: 3688 RVA: 0x0005700C File Offset: 0x0005520C
		private void DisableHud()
		{
			foreach (SpriteComponent spriteComponent in this._huds)
			{
				int num;
				if (spriteComponent.LayerMapTryGet(MindShieldComponent.LayerName, ref num, false))
				{
					spriteComponent.RemoveLayer(num);
				}
			}
			this._huds.Clear();
		}

		// Token: 0x04000723 RID: 1827
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000724 RID: 1828
		[Dependency]
		private readonly IEntityManager _entities;

		// Token: 0x04000725 RID: 1829
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000726 RID: 1830
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000727 RID: 1831
		private ResourcePath _hudPath = new ResourcePath("/Textures/White/Overlays/MindShield/hud.rsi", "/");

		// Token: 0x04000728 RID: 1832
		private string _state = "hui";

		// Token: 0x04000729 RID: 1833
		private List<SpriteComponent> _huds = new List<SpriteComponent>();

		// Token: 0x0400072A RID: 1834
		private ShaderInstance _shader;

		// Token: 0x0400072B RID: 1835
		private bool _overlayEnabled;
	}
}
