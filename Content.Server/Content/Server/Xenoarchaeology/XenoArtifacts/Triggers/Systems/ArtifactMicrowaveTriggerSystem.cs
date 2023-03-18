using System;
using System.Runtime.CompilerServices;
using Content.Server.Kitchen.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Systems
{
	// Token: 0x0200002B RID: 43
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArtifactMicrowaveTriggerSystem : EntitySystem
	{
		// Token: 0x060000A7 RID: 167 RVA: 0x00005728 File Offset: 0x00003928
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ArtifactMicrowaveTriggerComponent, BeingMicrowavedEvent>(new ComponentEventHandler<ArtifactMicrowaveTriggerComponent, BeingMicrowavedEvent>(this.OnMicrowaved), null, null);
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00005740 File Offset: 0x00003940
		private void OnMicrowaved(EntityUid uid, ArtifactMicrowaveTriggerComponent component, BeingMicrowavedEvent args)
		{
			this._artifact.TryActivateArtifact(uid, null, null);
		}

		// Token: 0x04000070 RID: 112
		[Dependency]
		private readonly ArtifactSystem _artifact;
	}
}
