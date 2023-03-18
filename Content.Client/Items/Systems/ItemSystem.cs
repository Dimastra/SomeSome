using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Hands;
using Content.Shared.Item;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Items.Systems
{
	// Token: 0x0200029D RID: 669
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ItemSystem : SharedItemSystem
	{
		// Token: 0x060010E9 RID: 4329 RVA: 0x00065273 File Offset: 0x00063473
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ItemComponent, GetInhandVisualsEvent>(new ComponentEventHandler<ItemComponent, GetInhandVisualsEvent>(this.OnGetVisuals), null, null);
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x00065290 File Offset: 0x00063490
		public override void VisualsChanged(EntityUid uid)
		{
			IContainer container;
			if (this.Container.TryGetContainingContainer(uid, ref container, null, null))
			{
				base.RaiseLocalEvent<VisualsChangedEvent>(container.Owner, new VisualsChangedEvent(uid, container.ID), false);
			}
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x000652C8 File Offset: 0x000634C8
		private void OnGetVisuals(EntityUid uid, ItemComponent item, GetInhandVisualsEvent args)
		{
			string text = "inhand-" + args.Location.ToString().ToLowerInvariant();
			List<SharedSpriteComponent.PrototypeLayerData> list;
			if (!item.InhandVisuals.TryGetValue(args.Location, out list) && !this.TryGetDefaultVisuals(uid, item, text, out list))
			{
				return;
			}
			int num = 0;
			foreach (SharedSpriteComponent.PrototypeLayerData prototypeLayerData in list)
			{
				HashSet<string> mapKeys = prototypeLayerData.MapKeys;
				string text2 = (mapKeys != null) ? mapKeys.FirstOrDefault<string>() : null;
				if (text2 == null)
				{
					string text3;
					if (num != 0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
						defaultInterpolatedStringHandler.AppendFormatted(text);
						defaultInterpolatedStringHandler.AppendLiteral("-");
						defaultInterpolatedStringHandler.AppendFormatted<int>(num);
						text3 = defaultInterpolatedStringHandler.ToStringAndClear();
					}
					else
					{
						text3 = text;
					}
					text2 = text3;
					num++;
				}
				args.Layers.Add(new ValueTuple<string, SharedSpriteComponent.PrototypeLayerData>(text2, prototypeLayerData));
			}
		}

		// Token: 0x060010EC RID: 4332 RVA: 0x000653BC File Offset: 0x000635BC
		private bool TryGetDefaultVisuals(EntityUid uid, ItemComponent item, string defaultKey, [Nullable(new byte[]
		{
			2,
			1
		})] [NotNullWhen(true)] out List<SharedSpriteComponent.PrototypeLayerData> result)
		{
			result = null;
			RSI rsi = null;
			SpriteComponent spriteComponent;
			if (item.RsiPath != null)
			{
				rsi = this._resCache.GetResource<RSIResource>(SharedSpriteComponent.TextureRoot / item.RsiPath, true).RSI;
			}
			else if (base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				rsi = spriteComponent.BaseRSI;
			}
			if (rsi == null || rsi.Path == null)
			{
				return false;
			}
			string text = (item.HeldPrefix == null) ? defaultKey : (item.HeldPrefix + "-" + defaultKey);
			RSI.State state;
			if (!rsi.TryGetState(text, ref state))
			{
				return false;
			}
			SharedSpriteComponent.PrototypeLayerData prototypeLayerData = new SharedSpriteComponent.PrototypeLayerData();
			prototypeLayerData.RsiPath = rsi.Path.ToString();
			prototypeLayerData.State = text;
			prototypeLayerData.MapKeys = new HashSet<string>
			{
				text
			};
			result = new List<SharedSpriteComponent.PrototypeLayerData>
			{
				prototypeLayerData
			};
			return true;
		}

		// Token: 0x0400084E RID: 2126
		[Dependency]
		private readonly IResourceCache _resCache;
	}
}
