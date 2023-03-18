using System;
using System.Runtime.CompilerServices;
using Content.Shared.Disease;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Disease
{
	// Token: 0x02000565 RID: 1381
	public sealed class DiseaseSnough : DiseaseEffect
	{
		// Token: 0x06001D49 RID: 7497 RVA: 0x0009C37C File Offset: 0x0009A57C
		public override void Effect(DiseaseEffectArgs args)
		{
			EntitySystem.Get<DiseaseSystem>().SneezeCough(args.DiseasedEntity, args.Disease, this.SnoughMessage, this.SnoughSound, this.AirTransmit, null);
		}

		// Token: 0x040012B2 RID: 4786
		[Nullable(1)]
		[DataField("snoughMessage", false, 1, false, false, null)]
		public string SnoughMessage = "disease-sneeze";

		// Token: 0x040012B3 RID: 4787
		[Nullable(2)]
		[DataField("snoughSound", false, 1, false, false, null)]
		public SoundSpecifier SnoughSound;

		// Token: 0x040012B4 RID: 4788
		[DataField("airTransmit", false, 1, false, false, null)]
		public bool AirTransmit = true;
	}
}
