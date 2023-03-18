using System;
using System.Runtime.CompilerServices;
using Content.Server.Sandbox;
using Robust.Shared.IoC;

namespace Content.Server.GameTicking.Rules
{
	// Token: 0x020004BF RID: 1215
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SandboxRuleSystem : GameRuleSystem
	{
		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06001900 RID: 6400 RVA: 0x000831FF File Offset: 0x000813FF
		public override string Prototype
		{
			get
			{
				return "Sandbox";
			}
		}

		// Token: 0x06001901 RID: 6401 RVA: 0x00083206 File Offset: 0x00081406
		public override void Started()
		{
			this._sandbox.IsSandboxEnabled = true;
		}

		// Token: 0x06001902 RID: 6402 RVA: 0x00083214 File Offset: 0x00081414
		public override void Ended()
		{
			this._sandbox.IsSandboxEnabled = false;
		}

		// Token: 0x04000F9C RID: 3996
		[Dependency]
		private readonly SandboxSystem _sandbox;
	}
}
