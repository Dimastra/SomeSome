using System;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x0200004A RID: 74
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RandomTeleportArtifactSystem : EntitySystem
	{
		// Token: 0x060000E6 RID: 230 RVA: 0x0000655D File Offset: 0x0000475D
		public override void Initialize()
		{
			base.SubscribeLocalEvent<RandomTeleportArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<RandomTeleportArtifactComponent, ArtifactActivatedEvent>(this.OnActivate), null, null);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00006574 File Offset: 0x00004774
		private void OnActivate(EntityUid uid, RandomTeleportArtifactComponent component, ArtifactActivatedEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			this._popup.PopupCoordinates(Loc.GetString("blink-artifact-popup"), xform.Coordinates, PopupType.Medium);
			this._xform.SetCoordinates(uid, xform, xform.Coordinates.Offset(this._random.NextVector2(component.Range)), null, true, null, null);
		}

		// Token: 0x040000A4 RID: 164
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040000A5 RID: 165
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x040000A6 RID: 166
		[Dependency]
		private readonly SharedTransformSystem _xform;
	}
}
