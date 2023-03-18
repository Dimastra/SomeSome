using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Nuke
{
	// Token: 0x02000328 RID: 808
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NukeLabelSystem : EntitySystem
	{
		// Token: 0x060010A2 RID: 4258 RVA: 0x00055922 File Offset: 0x00053B22
		public override void Initialize()
		{
			base.SubscribeLocalEvent<NukeLabelComponent, MapInitEvent>(new ComponentEventHandler<NukeLabelComponent, MapInitEvent>(this.OnMapInit), null, null);
		}

		// Token: 0x060010A3 RID: 4259 RVA: 0x00055938 File Offset: 0x00053B38
		private void OnMapInit(EntityUid uid, NukeLabelComponent nuke, MapInitEvent args)
		{
			string label = Loc.GetString(nuke.NukeLabel, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("serial", this._nuke.GenerateRandomNumberString(nuke.SerialLength))
			});
			MetaDataComponent metaDataComponent = base.MetaData(uid);
			metaDataComponent.EntityName = metaDataComponent.EntityName + " (" + label + ")";
		}

		// Token: 0x040009E6 RID: 2534
		[Dependency]
		private readonly NukeSystem _nuke;
	}
}
