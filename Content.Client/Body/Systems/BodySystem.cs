using System;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Components;
using Content.Shared.Body.Prototypes;
using Content.Shared.Body.Systems;

namespace Content.Client.Body.Systems
{
	// Token: 0x0200041E RID: 1054
	public sealed class BodySystem : SharedBodySystem
	{
		// Token: 0x060019DA RID: 6618 RVA: 0x0001B008 File Offset: 0x00019208
		[NullableContext(1)]
		protected override void InitBody(BodyComponent body, BodyPrototype prototype)
		{
		}
	}
}
