using System;
using System.Runtime.CompilerServices;
using Content.Client.SubFloor;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Piping;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.EntitySystems
{
	// Token: 0x02000459 RID: 1113
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AtmosPipeAppearanceSystem : EntitySystem
	{
		// Token: 0x06001BBF RID: 7103 RVA: 0x000A0654 File Offset: 0x0009E854
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PipeAppearanceComponent, ComponentInit>(new ComponentEventHandler<PipeAppearanceComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<PipeAppearanceComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<PipeAppearanceComponent, AppearanceChangeEvent>(this.OnAppearanceChanged), null, new Type[]
			{
				typeof(SubFloorHideSystem)
			});
		}

		// Token: 0x06001BC0 RID: 7104 RVA: 0x000A06A4 File Offset: 0x0009E8A4
		private void OnInit(EntityUid uid, PipeAppearanceComponent component, ComponentInit args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			RSIResource rsiresource;
			if (!this._resCache.TryGetResource<RSIResource>(SharedSpriteComponent.TextureRoot / component.RsiPath, ref rsiresource))
			{
				Logger.Error("AtmosPipeAppearanceSystem could not load to load RSI " + component.RsiPath + ".");
				return;
			}
			foreach (object obj in Enum.GetValues(typeof(AtmosPipeAppearanceSystem.PipeConnectionLayer)))
			{
				AtmosPipeAppearanceSystem.PipeConnectionLayer pipeConnectionLayer = (AtmosPipeAppearanceSystem.PipeConnectionLayer)obj;
				spriteComponent.LayerMapReserveBlank(pipeConnectionLayer);
				int num = spriteComponent.LayerMapGet(pipeConnectionLayer);
				spriteComponent.LayerSetRSI(num, rsiresource.RSI);
				string state = component.State;
				spriteComponent.LayerSetState(num, state);
				spriteComponent.LayerSetDirOffset(num, this.ToOffset(pipeConnectionLayer));
			}
		}

		// Token: 0x06001BC1 RID: 7105 RVA: 0x000A0798 File Offset: 0x0009E998
		private void OnAppearanceChanged(EntityUid uid, PipeAppearanceComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			if (!args.Sprite.Visible)
			{
				return;
			}
			Color white;
			if (!this._appearance.TryGetData<Color>(uid, PipeColorVisuals.Color, ref white, args.Component))
			{
				white = Color.White;
			}
			PipeDirection pipeDirection;
			if (!this._appearance.TryGetData<PipeDirection>(uid, PipeVisuals.VisualState, ref pipeDirection, args.Component))
			{
				return;
			}
			PipeDirection pipeDirection2 = pipeDirection.RotatePipeDirection(-base.Transform(uid).LocalRotation);
			foreach (object obj in Enum.GetValues(typeof(AtmosPipeAppearanceSystem.PipeConnectionLayer)))
			{
				AtmosPipeAppearanceSystem.PipeConnectionLayer pipeConnectionLayer = (AtmosPipeAppearanceSystem.PipeConnectionLayer)obj;
				int num;
				if (args.Sprite.LayerMapTryGet(pipeConnectionLayer, ref num, false))
				{
					ISpriteLayer spriteLayer = args.Sprite[num];
					PipeDirection other = (PipeDirection)pipeConnectionLayer;
					bool flag = pipeDirection2.HasDirection(other);
					spriteLayer.Visible = flag;
					if (flag)
					{
						spriteLayer.Color = white;
					}
				}
			}
		}

		// Token: 0x06001BC2 RID: 7106 RVA: 0x000A08BC File Offset: 0x0009EABC
		private SpriteComponent.DirectionOffset ToOffset(AtmosPipeAppearanceSystem.PipeConnectionLayer layer)
		{
			SpriteComponent.DirectionOffset result;
			if (layer != AtmosPipeAppearanceSystem.PipeConnectionLayer.NorthConnection)
			{
				if (layer != AtmosPipeAppearanceSystem.PipeConnectionLayer.WestConnection)
				{
					if (layer != AtmosPipeAppearanceSystem.PipeConnectionLayer.EastConnection)
					{
						result = 0;
					}
					else
					{
						result = 2;
					}
				}
				else
				{
					result = 1;
				}
			}
			else
			{
				result = 3;
			}
			return result;
		}

		// Token: 0x04000DD5 RID: 3541
		[Dependency]
		private readonly IResourceCache _resCache;

		// Token: 0x04000DD6 RID: 3542
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x0200045A RID: 1114
		[NullableContext(0)]
		private enum PipeConnectionLayer : byte
		{
			// Token: 0x04000DD8 RID: 3544
			NorthConnection = 1,
			// Token: 0x04000DD9 RID: 3545
			SouthConnection,
			// Token: 0x04000DDA RID: 3546
			EastConnection = 8,
			// Token: 0x04000DDB RID: 3547
			WestConnection = 4
		}
	}
}
