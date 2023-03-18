using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking.Rules.Components;
using Content.Shared.Humanoid;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Revolution
{
	// Token: 0x02000167 RID: 359
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RevolutionSystem : EntitySystem
	{
		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000959 RID: 2393 RVA: 0x00036BA2 File Offset: 0x00034DA2
		// (set) Token: 0x0600095A RID: 2394 RVA: 0x00036BAA File Offset: 0x00034DAA
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

		// Token: 0x0600095B RID: 2395 RVA: 0x00036BBC File Offset: 0x00034DBC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RevolutionaryComponent, ComponentHandleState>(new ComponentEventRefHandler<RevolutionaryComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<RevolutionaryComponent, ComponentAdd>(new ComponentEventHandler<RevolutionaryComponent, ComponentAdd>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<RevolutionaryComponent, ComponentRemove>(new ComponentEventHandler<RevolutionaryComponent, ComponentRemove>(this.OnComponentRemoved), null, null);
			base.SubscribeLocalEvent<PlayerAttachSysMessage>(new EntityEventHandler<PlayerAttachSysMessage>(this.OnMobStateChanged), null, null);
			this._shader = this._prototypeManager.Index<ShaderPrototype>("shaded").Instance();
		}

		// Token: 0x0600095C RID: 2396 RVA: 0x00036C3C File Offset: 0x00034E3C
		private void OnMobStateChanged(PlayerAttachSysMessage ev)
		{
			bool overlayEnabled = base.HasComp<RevolutionaryComponent>(ev.AttachedEntity);
			this.OverlayEnabled = overlayEnabled;
		}

		// Token: 0x0600095D RID: 2397 RVA: 0x00036C60 File Offset: 0x00034E60
		private void OnComponentInit(EntityUid uid, RevolutionaryComponent component, ComponentAdd args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (uid != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return;
			}
			this.OverlayEnabled = true;
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x00036CB4 File Offset: 0x00034EB4
		private void OnComponentRemoved(EntityUid uid, RevolutionaryComponent component, ComponentRemove args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (uid != ((localPlayer != null) ? localPlayer.ControlledEntity : null))
			{
				return;
			}
			this.OverlayEnabled = false;
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x00036D08 File Offset: 0x00034F08
		private void OnHandleState(EntityUid uid, RevolutionaryComponent component, ref ComponentHandleState args)
		{
			RevolutionaryComponentState revolutionaryComponentState = args.Current as RevolutionaryComponentState;
			if (revolutionaryComponentState == null)
			{
				return;
			}
			component.HeadRevolutionary = revolutionaryComponentState.HeadRevolutionary;
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x00036D34 File Offset: 0x00034F34
		public override void FrameUpdate(float frameTime)
		{
			base.FrameUpdate(frameTime);
			if (!this._overlayEnabled)
			{
				return;
			}
			IEnumerable<ValueTuple<HumanoidAppearanceComponent, RevolutionaryComponent>> enumerable = this.EntityManager.EntityQuery<HumanoidAppearanceComponent, RevolutionaryComponent>(false);
			List<SpriteComponent> list = new List<SpriteComponent>();
			foreach (ValueTuple<HumanoidAppearanceComponent, RevolutionaryComponent> valueTuple in enumerable)
			{
				RevolutionaryComponent item = valueTuple.Item2;
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

		// Token: 0x06000961 RID: 2401 RVA: 0x00036E1C File Offset: 0x0003501C
		private void AddHud(RevolutionaryComponent revolutionaryComponent, SpriteComponent spriteComponent)
		{
			int num;
			if (!spriteComponent.LayerMapTryGet(RevolutionaryComponent.LayerName, ref num, false))
			{
				num = spriteComponent.LayerMapReserveBlank(RevolutionaryComponent.LayerName);
			}
			spriteComponent.LayerSetRSI(num, this._hudPath);
			spriteComponent.LayerSetShader(num, this._shader, null);
			if (revolutionaryComponent.HeadRevolutionary)
			{
				spriteComponent.LayerSetState(num, this._revolutionaryHeadState);
			}
			else
			{
				spriteComponent.LayerSetState(num, this._revolutionaryMinionState);
			}
			this._huds.Add(spriteComponent);
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x00036E9C File Offset: 0x0003509C
		private void RemoveHud(SpriteComponent spriteComponent)
		{
			if (!this._huds.Contains(spriteComponent))
			{
				return;
			}
			int num;
			if (!spriteComponent.LayerMapTryGet(RevolutionaryComponent.LayerName, ref num, false))
			{
				return;
			}
			if (base.HasComp<TransformComponent>(spriteComponent.Owner))
			{
				spriteComponent.RemoveLayer(num);
			}
			this._huds.Remove(spriteComponent);
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x00036EEC File Offset: 0x000350EC
		private void DisableHud()
		{
			foreach (SpriteComponent spriteComponent in this._huds)
			{
				int num;
				if (spriteComponent.LayerMapTryGet(RevolutionaryComponent.LayerName, ref num, false))
				{
					spriteComponent.RemoveLayer(num);
				}
			}
			this._huds.Clear();
		}

		// Token: 0x040004B8 RID: 1208
		[Dependency]
		private readonly IUserInterfaceManager _uiManager;

		// Token: 0x040004B9 RID: 1209
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040004BA RID: 1210
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x040004BB RID: 1211
		private ResourcePath _hudPath = new ResourcePath("/Textures/White/Overlays/Revolution/hud.rsi", "/");

		// Token: 0x040004BC RID: 1212
		private ShaderInstance _shader;

		// Token: 0x040004BD RID: 1213
		private string _revolutionaryHeadState = "reva_head_animated";

		// Token: 0x040004BE RID: 1214
		private string _revolutionaryMinionState = "reva_podsos_animated";

		// Token: 0x040004BF RID: 1215
		private List<SpriteComponent> _huds = new List<SpriteComponent>();

		// Token: 0x040004C0 RID: 1216
		private bool _overlayEnabled;
	}
}
