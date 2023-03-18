using System;
using System.Runtime.CompilerServices;
using Content.Shared.Singularity;
using Content.Shared.Singularity.Components;
using Content.Shared.Singularity.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client.Singularity.EntitySystems
{
	// Token: 0x02000144 RID: 324
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SingularitySystem : SharedSingularitySystem
	{
		// Token: 0x06000872 RID: 2162 RVA: 0x0003106D File Offset: 0x0002F26D
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SingularityComponent, ComponentHandleState>(new ComponentEventRefHandler<SingularityComponent, ComponentHandleState>(this.HandleSingularityState), null, null);
			base.SubscribeLocalEvent<SingularityComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<SingularityComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
		}

		// Token: 0x06000873 RID: 2163 RVA: 0x000310A0 File Offset: 0x0002F2A0
		private void HandleSingularityState(EntityUid uid, SingularityComponent comp, ref ComponentHandleState args)
		{
			SharedSingularitySystem.SingularityComponentState singularityComponentState = args.Current as SharedSingularitySystem.SingularityComponentState;
			if (singularityComponentState == null)
			{
				return;
			}
			base.SetLevel(uid, singularityComponentState.Level, comp);
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x000310CC File Offset: 0x0002F2CC
		protected override void OnSingularityStartup(EntityUid uid, SingularityComponent comp, ComponentStartup args)
		{
			base.OnSingularityStartup(uid, comp, args);
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				spriteComponent.LayerMapReserveBlank(comp.Layer);
			}
		}

		// Token: 0x06000875 RID: 2165 RVA: 0x00031100 File Offset: 0x0002F300
		private void OnAppearanceChange(EntityUid uid, SingularityComponent comp, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			byte value;
			if (!this._appearanceSystem.TryGetData<byte>(uid, SingularityVisuals.Level, ref value, args.Component))
			{
				return;
			}
			SpriteComponent sprite = args.Sprite;
			int layer = comp.Layer;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
			defaultInterpolatedStringHandler.AppendFormatted<ResourcePath>(comp.BaseSprite.RsiPath);
			defaultInterpolatedStringHandler.AppendLiteral("_");
			defaultInterpolatedStringHandler.AppendFormatted<byte>(value);
			defaultInterpolatedStringHandler.AppendLiteral(".rsi");
			ResourcePath resourcePath = new ResourcePath(defaultInterpolatedStringHandler.ToStringAndClear(), "/");
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler.AppendFormatted(comp.BaseSprite.RsiState);
			defaultInterpolatedStringHandler.AppendLiteral("_");
			defaultInterpolatedStringHandler.AppendFormatted<byte>(value);
			sprite.LayerSetSprite(layer, new SpriteSpecifier.Rsi(resourcePath, defaultInterpolatedStringHandler.ToStringAndClear()));
		}

		// Token: 0x0400044E RID: 1102
		[Dependency]
		private readonly AppearanceSystem _appearanceSystem;
	}
}
