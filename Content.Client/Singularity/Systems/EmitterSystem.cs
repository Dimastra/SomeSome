using System;
using System.Runtime.CompilerServices;
using Content.Client.Storage.Visualizers;
using Content.Shared.Singularity.Components;
using Content.Shared.Singularity.EntitySystems;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Singularity.Systems
{
	// Token: 0x02000145 RID: 325
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EmitterSystem : SharedEmitterSystem
	{
		// Token: 0x06000877 RID: 2167 RVA: 0x000311D3 File Offset: 0x0002F3D3
		public override void Initialize()
		{
			base.SubscribeLocalEvent<EmitterComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<EmitterComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
		}

		// Token: 0x06000878 RID: 2168 RVA: 0x000311EC File Offset: 0x0002F3EC
		private void OnAppearanceChange(EntityUid uid, EmitterComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			int num;
			if (args.Sprite.LayerMapTryGet(StorageVisualLayers.Lock, ref num, false))
			{
				bool flag;
				if (!this._appearance.TryGetData<bool>(uid, StorageVisuals.Locked, ref flag, args.Component))
				{
					flag = false;
				}
				args.Sprite.LayerSetVisible(num, flag);
			}
			EmitterVisualState emitterVisualState;
			if (!this._appearance.TryGetData<EmitterVisualState>(uid, EmitterVisuals.VisualState, ref emitterVisualState, args.Component))
			{
				emitterVisualState = EmitterVisualState.Off;
			}
			int num2;
			if (!args.Sprite.LayerMapTryGet(EmitterVisualLayers.Lights, ref num2, false))
			{
				return;
			}
			switch (emitterVisualState)
			{
			case EmitterVisualState.On:
				if (component.OnState != null)
				{
					args.Sprite.LayerSetVisible(num2, true);
					args.Sprite.LayerSetState(num2, component.OnState);
					return;
				}
				break;
			case EmitterVisualState.Underpowered:
				if (component.UnderpoweredState != null)
				{
					args.Sprite.LayerSetVisible(num2, true);
					args.Sprite.LayerSetState(num2, component.UnderpoweredState);
					return;
				}
				break;
			case EmitterVisualState.Off:
				args.Sprite.LayerSetVisible(num2, false);
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x0400044F RID: 1103
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
