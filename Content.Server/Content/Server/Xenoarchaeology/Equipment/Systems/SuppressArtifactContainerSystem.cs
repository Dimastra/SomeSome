using System;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.Equipment.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

namespace Content.Server.Xenoarchaeology.Equipment.Systems
{
	// Token: 0x02000063 RID: 99
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SuppressArtifactContainerSystem : EntitySystem
	{
		// Token: 0x0600012A RID: 298 RVA: 0x00007C87 File Offset: 0x00005E87
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SuppressArtifactContainerComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<SuppressArtifactContainerComponent, EntInsertedIntoContainerMessage>(this.OnInserted), null, null);
			base.SubscribeLocalEvent<SuppressArtifactContainerComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<SuppressArtifactContainerComponent, EntRemovedFromContainerMessage>(this.OnRemoved), null, null);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00007CB8 File Offset: 0x00005EB8
		private void OnInserted(EntityUid uid, SuppressArtifactContainerComponent component, EntInsertedIntoContainerMessage args)
		{
			ArtifactComponent artifact;
			if (!base.TryComp<ArtifactComponent>(args.Entity, ref artifact))
			{
				return;
			}
			artifact.IsSuppressed = true;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00007CE0 File Offset: 0x00005EE0
		private void OnRemoved(EntityUid uid, SuppressArtifactContainerComponent component, EntRemovedFromContainerMessage args)
		{
			ArtifactComponent artifact;
			if (!base.TryComp<ArtifactComponent>(args.Entity, ref artifact))
			{
				return;
			}
			artifact.IsSuppressed = false;
		}
	}
}
