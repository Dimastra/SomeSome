using System;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Systems
{
	// Token: 0x02000021 RID: 33
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArtifactAnchorTriggerSystem : EntitySystem
	{
		// Token: 0x06000080 RID: 128 RVA: 0x00004C2C File Offset: 0x00002E2C
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ArtifactAnchorTriggerComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<ArtifactAnchorTriggerComponent, AnchorStateChangedEvent>(this.OnAnchorStateChanged), null, null);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00004C44 File Offset: 0x00002E44
		private void OnAnchorStateChanged(EntityUid uid, ArtifactAnchorTriggerComponent component, ref AnchorStateChangedEvent args)
		{
			if (args.Detaching)
			{
				return;
			}
			this._artifact.TryActivateArtifact(uid, null, null);
		}

		// Token: 0x04000062 RID: 98
		[Dependency]
		private readonly ArtifactSystem _artifact;
	}
}
