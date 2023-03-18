using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Content.Server.Chat.Systems;
using Robust.Shared.GameObjects;

namespace Content.Server.Speech
{
	// Token: 0x020001AD RID: 429
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AccentSystem : EntitySystem
	{
		// Token: 0x06000875 RID: 2165 RVA: 0x0002B450 File Offset: 0x00029650
		public override void Initialize()
		{
			base.SubscribeLocalEvent<TransformSpeechEvent>(new EntityEventHandler<TransformSpeechEvent>(this.AccentHandler), null, null);
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x0002B468 File Offset: 0x00029668
		private void AccentHandler(TransformSpeechEvent args)
		{
			AccentGetEvent accentEvent = new AccentGetEvent(args.Sender, args.Message);
			base.RaiseLocalEvent<AccentGetEvent>(args.Sender, accentEvent, true);
			args.Message = accentEvent.Message;
		}

		// Token: 0x0400052C RID: 1324
		public static readonly Regex SentenceRegex = new Regex("(?<=[\\.!\\?])", RegexOptions.Compiled);
	}
}
