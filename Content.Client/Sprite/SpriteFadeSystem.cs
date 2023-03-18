using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Gameplay;
using Content.Shared.Sprite;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Sprite
{
	// Token: 0x02000138 RID: 312
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpriteFadeSystem : EntitySystem
	{
		// Token: 0x06000854 RID: 2132 RVA: 0x000306D0 File Offset: 0x0002E8D0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FadingSpriteComponent, ComponentShutdown>(new ComponentEventHandler<FadingSpriteComponent, ComponentShutdown>(this.OnFadingShutdown), null, null);
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x000306EC File Offset: 0x0002E8EC
		private void OnFadingShutdown(EntityUid uid, FadingSpriteComponent component, ComponentShutdown args)
		{
			SpriteComponent spriteComponent;
			if (base.MetaData(uid).EntityLifeStage >= 4 || !base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			spriteComponent.Color = spriteComponent.Color.WithAlpha(component.OriginalAlpha);
		}

		// Token: 0x06000856 RID: 2134 RVA: 0x00030730 File Offset: 0x0002E930
		public override void FrameUpdate(float frameTime)
		{
			base.FrameUpdate(frameTime);
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			EntityQuery<SpriteComponent> entityQuery = base.GetEntityQuery<SpriteComponent>();
			float num = 1f * frameTime;
			TransformComponent transformComponent;
			if (base.TryComp<TransformComponent>(entityUid, ref transformComponent))
			{
				GameplayState gameplayState = this._stateManager.CurrentState as GameplayState;
				SpriteComponent spriteComponent;
				if (gameplayState != null && entityQuery.TryGetComponent(entityUid, ref spriteComponent))
				{
					EntityQuery<SpriteFadeComponent> entityQuery2 = base.GetEntityQuery<SpriteFadeComponent>();
					MapCoordinates mapPosition = transformComponent.MapPosition;
					foreach (EntityUid entityUid2 in gameplayState.GetClickableEntities(mapPosition))
					{
						SpriteComponent spriteComponent2;
						if (!(entityUid2 == entityUid) && entityQuery2.HasComponent(entityUid2) && entityQuery.TryGetComponent(entityUid2, ref spriteComponent2) && spriteComponent2.DrawDepth >= spriteComponent.DrawDepth)
						{
							FadingSpriteComponent fadingSpriteComponent;
							if (!base.TryComp<FadingSpriteComponent>(entityUid2, ref fadingSpriteComponent))
							{
								fadingSpriteComponent = base.AddComp<FadingSpriteComponent>(entityUid2);
								fadingSpriteComponent.OriginalAlpha = spriteComponent2.Color.A;
							}
							this._comps.Add(fadingSpriteComponent);
							float num2 = Math.Max(spriteComponent2.Color.A - num, 0.4f);
							Color color = spriteComponent2.Color;
							if (!color.A.Equals(num2))
							{
								SpriteComponent spriteComponent3 = spriteComponent2;
								color = spriteComponent2.Color;
								spriteComponent3.Color = color.WithAlpha(num2);
							}
						}
					}
				}
			}
			foreach (FadingSpriteComponent fadingSpriteComponent2 in base.EntityQuery<FadingSpriteComponent>(true))
			{
				SpriteComponent spriteComponent4;
				if (!this._comps.Contains(fadingSpriteComponent2) && entityQuery.TryGetComponent(fadingSpriteComponent2.Owner, ref spriteComponent4))
				{
					float num3 = Math.Min(spriteComponent4.Color.A + num, fadingSpriteComponent2.OriginalAlpha);
					if (!num3.Equals(spriteComponent4.Color.A))
					{
						SpriteComponent spriteComponent5 = spriteComponent4;
						Color color = spriteComponent4.Color;
						spriteComponent5.Color = color.WithAlpha(num3);
					}
					else
					{
						base.RemCompDeferred<FadingSpriteComponent>(fadingSpriteComponent2.Owner);
					}
				}
			}
			this._comps.Clear();
		}

		// Token: 0x04000433 RID: 1075
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000434 RID: 1076
		[Dependency]
		private readonly IStateManager _stateManager;

		// Token: 0x04000435 RID: 1077
		private readonly HashSet<FadingSpriteComponent> _comps = new HashSet<FadingSpriteComponent>();

		// Token: 0x04000436 RID: 1078
		private const float TargetAlpha = 0.4f;

		// Token: 0x04000437 RID: 1079
		private const float ChangeRate = 1f;
	}
}
