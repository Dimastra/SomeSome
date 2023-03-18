using System;
using System.Runtime.CompilerServices;
using Content.Server.Magic.Events;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Robust.Shared.GameObjects;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x02000046 RID: 70
	public sealed class KnockArtifactSystem : EntitySystem
	{
		// Token: 0x060000DA RID: 218 RVA: 0x00006384 File Offset: 0x00004584
		public override void Initialize()
		{
			base.SubscribeLocalEvent<KnockArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<KnockArtifactComponent, ArtifactActivatedEvent>(this.OnActivated), null, null);
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000639C File Offset: 0x0000459C
		[NullableContext(1)]
		private void OnActivated(EntityUid uid, KnockArtifactComponent component, ArtifactActivatedEvent args)
		{
			KnockSpellEvent ev = new KnockSpellEvent
			{
				Performer = uid,
				Range = component.KnockRange
			};
			base.RaiseLocalEvent<KnockSpellEvent>(ev);
		}
	}
}
