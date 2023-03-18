using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Decals;
using Content.Shared.Sprite;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Sprite
{
	// Token: 0x020001AB RID: 427
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RandomSpriteSystem : SharedRandomSpriteSystem
	{
		// Token: 0x0600086D RID: 2157 RVA: 0x0002B163 File Offset: 0x00029363
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RandomSpriteComponent, ComponentGetState>(new ComponentEventRefHandler<RandomSpriteComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<RandomSpriteComponent, MapInitEvent>(new ComponentEventHandler<RandomSpriteComponent, MapInitEvent>(this.OnMapInit), null, null);
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x0002B194 File Offset: 0x00029394
		private void OnMapInit(EntityUid uid, RandomSpriteComponent component, MapInitEvent args)
		{
			if (component.Selected.Count > 0)
			{
				return;
			}
			if (component.Available.Count == 0)
			{
				return;
			}
			Dictionary<string, ValueTuple<string, string>> group = RandomExtensions.Pick<Dictionary<string, ValueTuple<string, string>>>(this._random, component.Available);
			component.Selected.EnsureCapacity(group.Count);
			foreach (KeyValuePair<string, ValueTuple<string, string>> layer in group)
			{
				Color? color = null;
				if (!string.IsNullOrEmpty(layer.Value.Item2))
				{
					color = new Color?(RandomExtensions.Pick<Color>(this._random, this._prototype.Index<ColorPalettePrototype>(layer.Value.Item2).Colors.Values));
				}
				component.Selected.Add(layer.Key, new ValueTuple<string, Color?>(layer.Value.Item1, color));
			}
			base.Dirty(component, null);
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x0002B29C File Offset: 0x0002949C
		private void OnGetState(EntityUid uid, RandomSpriteComponent component, ref ComponentGetState args)
		{
			args.State = new RandomSpriteColorComponentState
			{
				Selected = component.Selected
			};
		}

		// Token: 0x04000529 RID: 1321
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x0400052A RID: 1322
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
