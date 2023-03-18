using System;
using System.Runtime.CompilerServices;
using Content.Server.Speech.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Speech.EntitySystems
{
	// Token: 0x020001B9 RID: 441
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ReplacementAccentSystem : EntitySystem
	{
		// Token: 0x0600089F RID: 2207 RVA: 0x0002C3B8 File Offset: 0x0002A5B8
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ReplacementAccentComponent, AccentGetEvent>(new ComponentEventHandler<ReplacementAccentComponent, AccentGetEvent>(this.OnAccent), null, null);
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x0002C3D0 File Offset: 0x0002A5D0
		private void OnAccent(EntityUid uid, ReplacementAccentComponent component, AccentGetEvent args)
		{
			string[] words = this._proto.Index<ReplacementAccentPrototype>(component.Accent).Words;
			args.Message = ((words.Length != 0) ? Loc.GetString(RandomExtensions.Pick<string>(this._random, words)) : "");
		}

		// Token: 0x0400053E RID: 1342
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x0400053F RID: 1343
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
