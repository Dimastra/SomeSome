using System;
using System.Runtime.CompilerServices;
using Content.Server.Speech.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Speech.EntitySystems
{
	// Token: 0x020001B3 RID: 435
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BackwardsAccentSystem : EntitySystem
	{
		// Token: 0x06000886 RID: 2182 RVA: 0x0002B7CC File Offset: 0x000299CC
		public override void Initialize()
		{
			base.SubscribeLocalEvent<BackwardsAccentComponent, AccentGetEvent>(new ComponentEventHandler<BackwardsAccentComponent, AccentGetEvent>(this.OnAccent), null, null);
		}

		// Token: 0x06000887 RID: 2183 RVA: 0x0002B7E2 File Offset: 0x000299E2
		public string Accentuate(string message)
		{
			char[] array = message.ToCharArray();
			Array.Reverse<char>(array);
			return new string(array);
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x0002B7F5 File Offset: 0x000299F5
		private void OnAccent(EntityUid uid, BackwardsAccentComponent component, AccentGetEvent args)
		{
			args.Message = this.Accentuate(args.Message);
		}
	}
}
