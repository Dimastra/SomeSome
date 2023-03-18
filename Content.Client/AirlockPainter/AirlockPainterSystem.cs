using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.AirlockPainter;
using Content.Shared.AirlockPainter.Prototypes;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client.AirlockPainter
{
	// Token: 0x0200047E RID: 1150
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AirlockPainterSystem : SharedAirlockPainterSystem
	{
		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x06001C67 RID: 7271 RVA: 0x000A4CC0 File Offset: 0x000A2EC0
		// (set) Token: 0x06001C68 RID: 7272 RVA: 0x000A4CC8 File Offset: 0x000A2EC8
		public List<AirlockPainterEntry> Entries { get; private set; } = new List<AirlockPainterEntry>();

		// Token: 0x06001C69 RID: 7273 RVA: 0x000A4CD4 File Offset: 0x000A2ED4
		public override void Initialize()
		{
			base.Initialize();
			using (List<string>.Enumerator enumerator = base.Styles.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string style = enumerator.Current;
					List<AirlockGroupPrototype> list = base.Groups.FindAll((AirlockGroupPrototype x) => x.StylePaths.ContainsKey(style));
					string text;
					if (list == null)
					{
						text = null;
					}
					else
					{
						AirlockGroupPrototype airlockGroupPrototype = list.MaxBy((AirlockGroupPrototype x) => x.IconPriority);
						text = ((airlockGroupPrototype != null) ? airlockGroupPrototype.StylePaths[style] : null);
					}
					string text2 = text;
					RSI.State state;
					if (text2 == null)
					{
						this.Entries.Add(new AirlockPainterEntry(style, null));
					}
					else if (!this._resourceCache.GetResource<RSIResource>(SharedSpriteComponent.TextureRoot / new ResourcePath(text2, "/"), true).RSI.TryGetState("closed", ref state))
					{
						this.Entries.Add(new AirlockPainterEntry(style, null));
					}
					else
					{
						this.Entries.Add(new AirlockPainterEntry(style, state.Frame0));
					}
				}
			}
		}

		// Token: 0x04000E34 RID: 3636
		[Dependency]
		private readonly IResourceCache _resourceCache;
	}
}
