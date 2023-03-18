using System;
using System.Runtime.CompilerServices;
using Content.Server.Polymorph.Systems;
using Content.Shared.Audio;
using Content.Shared.Disease;
using Content.Shared.Polymorph;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disease.Effects
{
	// Token: 0x0200056B RID: 1387
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class DiseasePolymorph : DiseaseEffect
	{
		// Token: 0x06001D53 RID: 7507 RVA: 0x0009C600 File Offset: 0x0009A800
		public override void Effect(DiseaseEffectArgs args)
		{
			EntityUid? polyUid = EntitySystem.Get<PolymorphableSystem>().PolymorphEntity(args.DiseasedEntity, this.PolymorphId);
			if (this.PolymorphSound != null && polyUid != null)
			{
				SoundSystem.Play(this.PolymorphSound.GetSound(null, null), Filter.Pvs(polyUid.Value, 2f, null, null, null), polyUid.Value, new AudioParams?(AudioHelpers.WithVariation(0.2f)));
			}
			if (this.PolymorphMessage != null && polyUid != null)
			{
				EntitySystem.Get<SharedPopupSystem>().PopupEntity(Loc.GetString(this.PolymorphMessage), polyUid.Value, polyUid.Value, PopupType.Large);
			}
		}

		// Token: 0x040012C2 RID: 4802
		[Nullable(1)]
		[DataField("polymorphId", false, 1, true, false, typeof(PrototypeIdSerializer<PolymorphPrototype>))]
		[ViewVariables]
		public readonly string PolymorphId;

		// Token: 0x040012C3 RID: 4803
		[DataField("polymorphSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier PolymorphSound;

		// Token: 0x040012C4 RID: 4804
		[DataField("polymorphMessage", false, 1, false, false, null)]
		[ViewVariables]
		public string PolymorphMessage;
	}
}
