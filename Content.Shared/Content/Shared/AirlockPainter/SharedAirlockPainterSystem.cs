using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.AirlockPainter.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.AirlockPainter
{
	// Token: 0x02000729 RID: 1833
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedAirlockPainterSystem : EntitySystem
	{
		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x06001637 RID: 5687 RVA: 0x00048B42 File Offset: 0x00046D42
		// (set) Token: 0x06001638 RID: 5688 RVA: 0x00048B4A File Offset: 0x00046D4A
		public List<string> Styles { get; private set; } = new List<string>();

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x06001639 RID: 5689 RVA: 0x00048B53 File Offset: 0x00046D53
		// (set) Token: 0x0600163A RID: 5690 RVA: 0x00048B5B File Offset: 0x00046D5B
		public List<AirlockGroupPrototype> Groups { get; private set; } = new List<AirlockGroupPrototype>();

		// Token: 0x0600163B RID: 5691 RVA: 0x00048B64 File Offset: 0x00046D64
		public override void Initialize()
		{
			base.Initialize();
			SortedSet<string> styles = new SortedSet<string>();
			foreach (AirlockGroupPrototype grp in this._prototypeManager.EnumeratePrototypes<AirlockGroupPrototype>())
			{
				this.Groups.Add(grp);
				foreach (string style in grp.StylePaths.Keys)
				{
					styles.Add(style);
				}
			}
			this.Styles = styles.ToList<string>();
		}

		// Token: 0x04001684 RID: 5764
		[Dependency]
		protected readonly IPrototypeManager _prototypeManager;
	}
}
