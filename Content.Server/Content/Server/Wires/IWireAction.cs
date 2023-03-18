using System;
using System.Runtime.CompilerServices;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;

namespace Content.Server.Wires
{
	// Token: 0x02000071 RID: 113
	[NullableContext(1)]
	public interface IWireAction
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000164 RID: 356
		[Nullable(2)]
		object StatusKey { [NullableContext(2)] get; }

		// Token: 0x06000165 RID: 357
		void Initialize();

		// Token: 0x06000166 RID: 358
		bool AddWire(Wire wire, int count);

		// Token: 0x06000167 RID: 359
		bool Cut(EntityUid user, Wire wire);

		// Token: 0x06000168 RID: 360
		bool Mend(EntityUid user, Wire wire);

		// Token: 0x06000169 RID: 361
		void Pulse(EntityUid user, Wire wire);

		// Token: 0x0600016A RID: 362
		void Update(Wire wire);

		// Token: 0x0600016B RID: 363
		StatusLightData? GetStatusLightData(Wire wire);
	}
}
