using System;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Systems
{
	// Token: 0x02000025 RID: 37
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArtifactExamineTriggerSystem : EntitySystem
	{
		// Token: 0x0600008E RID: 142 RVA: 0x00004FFA File Offset: 0x000031FA
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ArtifactExamineTriggerComponent, ExaminedEvent>(new ComponentEventHandler<ArtifactExamineTriggerComponent, ExaminedEvent>(this.OnExamine), null, null);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005010 File Offset: 0x00003210
		private void OnExamine(EntityUid uid, ArtifactExamineTriggerComponent component, ExaminedEvent args)
		{
			this._artifact.TryActivateArtifact(uid, null, null);
		}

		// Token: 0x04000066 RID: 102
		[Dependency]
		private readonly ArtifactSystem _artifact;
	}
}
