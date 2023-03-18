using System;
using System.Runtime.CompilerServices;
using Content.Server.Instruments;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x02000049 RID: 73
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RandomInstrumentArtifactSystem : EntitySystem
	{
		// Token: 0x060000E3 RID: 227 RVA: 0x0000650C File Offset: 0x0000470C
		public override void Initialize()
		{
			base.SubscribeLocalEvent<RandomInstrumentArtifactComponent, ComponentStartup>(new ComponentEventHandler<RandomInstrumentArtifactComponent, ComponentStartup>(this.OnStartup), null, null);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00006524 File Offset: 0x00004724
		private void OnStartup(EntityUid uid, RandomInstrumentArtifactComponent component, ComponentStartup args)
		{
			InstrumentComponent instrument = base.EnsureComp<InstrumentComponent>(uid);
			this._instrument.SetInstrumentProgram(instrument, (byte)this._random.Next(0, 127), 0);
		}

		// Token: 0x040000A2 RID: 162
		[Dependency]
		private readonly InstrumentSystem _instrument;

		// Token: 0x040000A3 RID: 163
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
