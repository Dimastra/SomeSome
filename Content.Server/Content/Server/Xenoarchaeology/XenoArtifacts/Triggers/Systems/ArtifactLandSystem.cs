using System;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Systems
{
	// Token: 0x02000029 RID: 41
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArtifactLandSystem : EntitySystem
	{
		// Token: 0x060000A0 RID: 160 RVA: 0x00005486 File Offset: 0x00003686
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ArtifactLandTriggerComponent, LandEvent>(new ComponentEventRefHandler<ArtifactLandTriggerComponent, LandEvent>(this.OnLand), null, null);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000054A2 File Offset: 0x000036A2
		private void OnLand(EntityUid uid, ArtifactLandTriggerComponent component, ref LandEvent args)
		{
			this._artifact.TryActivateArtifact(uid, args.User, null);
		}

		// Token: 0x0400006E RID: 110
		[Dependency]
		private readonly ArtifactSystem _artifact;
	}
}
