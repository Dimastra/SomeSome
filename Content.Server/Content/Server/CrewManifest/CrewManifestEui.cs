using System;
using System.Runtime.CompilerServices;
using Content.Server.EUI;
using Content.Shared.CrewManifest;
using Content.Shared.Eui;
using Robust.Shared.GameObjects;

namespace Content.Server.CrewManifest
{
	// Token: 0x020005D7 RID: 1495
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CrewManifestEui : BaseEui
	{
		// Token: 0x06001FEB RID: 8171 RVA: 0x000A6E8D File Offset: 0x000A508D
		public CrewManifestEui(EntityUid station, EntityUid? owner, CrewManifestSystem crewManifestSystem)
		{
			this._station = station;
			this.Owner = owner;
			this._crewManifest = crewManifestSystem;
		}

		// Token: 0x06001FEC RID: 8172 RVA: 0x000A6EAC File Offset: 0x000A50AC
		[PreserveBaseOverrides]
		public new CrewManifestEuiState GetNewState()
		{
			ValueTuple<string, CrewManifestEntries> crewManifest = this._crewManifest.GetCrewManifest(this._station);
			string name = crewManifest.Item1;
			CrewManifestEntries entries = crewManifest.Item2;
			return new CrewManifestEuiState(name, entries);
		}

		// Token: 0x06001FED RID: 8173 RVA: 0x000A6EDE File Offset: 0x000A50DE
		public override void HandleMessage(EuiMessageBase msg)
		{
			base.HandleMessage(msg);
			if (msg is CrewManifestEuiClosed)
			{
				this.Closed();
			}
		}

		// Token: 0x06001FEE RID: 8174 RVA: 0x000A6EF5 File Offset: 0x000A50F5
		public override void Closed()
		{
			base.Closed();
			this._crewManifest.CloseEui(this._station, base.Player, this.Owner);
		}

		// Token: 0x040013D1 RID: 5073
		private readonly CrewManifestSystem _crewManifest;

		// Token: 0x040013D2 RID: 5074
		private readonly EntityUid _station;

		// Token: 0x040013D3 RID: 5075
		public readonly EntityUid? Owner;
	}
}
