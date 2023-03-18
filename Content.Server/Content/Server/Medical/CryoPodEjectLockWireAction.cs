using System;
using System.Runtime.CompilerServices;
using Content.Server.Medical.Components;
using Content.Server.Wires;
using Content.Shared.Medical.Cryogenics;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.Medical
{
	// Token: 0x020003AD RID: 941
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class CryoPodEjectLockWireAction : ComponentWireAction<CryoPodComponent>
	{
		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06001343 RID: 4931 RVA: 0x000633DC File Offset: 0x000615DC
		// (set) Token: 0x06001344 RID: 4932 RVA: 0x000633E4 File Offset: 0x000615E4
		public override Color Color { get; set; } = Color.Red;

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06001345 RID: 4933 RVA: 0x000633ED File Offset: 0x000615ED
		// (set) Token: 0x06001346 RID: 4934 RVA: 0x000633F5 File Offset: 0x000615F5
		public override string Name { get; set; } = "wire-name-lock";

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06001347 RID: 4935 RVA: 0x000633FE File Offset: 0x000615FE
		// (set) Token: 0x06001348 RID: 4936 RVA: 0x00063406 File Offset: 0x00061606
		public override bool LightRequiresPower { get; set; }

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06001349 RID: 4937 RVA: 0x0006340F File Offset: 0x0006160F
		[Nullable(2)]
		public override object StatusKey { [NullableContext(2)] get; } = CryoPodWireActionKey.Key;

		// Token: 0x0600134A RID: 4938 RVA: 0x00063417 File Offset: 0x00061617
		public override bool Cut(EntityUid user, Wire wire, CryoPodComponent cryoPodComponent)
		{
			if (!cryoPodComponent.PermaLocked)
			{
				cryoPodComponent.Locked = true;
			}
			return true;
		}

		// Token: 0x0600134B RID: 4939 RVA: 0x00063429 File Offset: 0x00061629
		public override bool Mend(EntityUid user, Wire wire, CryoPodComponent cryoPodComponent)
		{
			if (!cryoPodComponent.PermaLocked)
			{
				cryoPodComponent.Locked = false;
			}
			return true;
		}

		// Token: 0x0600134C RID: 4940 RVA: 0x0006343B File Offset: 0x0006163B
		public override void Pulse(EntityUid user, Wire wire, CryoPodComponent cryoPodComponent)
		{
		}

		// Token: 0x0600134D RID: 4941 RVA: 0x0006343D File Offset: 0x0006163D
		public override StatusLightState? GetLightState(Wire wire, CryoPodComponent comp)
		{
			return new StatusLightState?(comp.Locked ? StatusLightState.On : StatusLightState.Off);
		}
	}
}
