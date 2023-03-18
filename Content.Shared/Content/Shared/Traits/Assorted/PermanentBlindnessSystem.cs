using System;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Content.Shared.Eye.Blinding;
using Content.Shared.IdentityManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;

namespace Content.Shared.Traits.Assorted
{
	// Token: 0x020000AF RID: 175
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PermanentBlindnessSystem : EntitySystem
	{
		// Token: 0x060001E5 RID: 485 RVA: 0x0000A1F5 File Offset: 0x000083F5
		public override void Initialize()
		{
			base.SubscribeLocalEvent<PermanentBlindnessComponent, ComponentStartup>(new ComponentEventHandler<PermanentBlindnessComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<PermanentBlindnessComponent, ComponentShutdown>(new ComponentEventHandler<PermanentBlindnessComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<PermanentBlindnessComponent, ExaminedEvent>(new ComponentEventHandler<PermanentBlindnessComponent, ExaminedEvent>(this.OnExamined), null, null);
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000A234 File Offset: 0x00008434
		private void OnExamined(EntityUid uid, PermanentBlindnessComponent component, ExaminedEvent args)
		{
			if (args.IsInDetailsRange && !this._net.IsClient)
			{
				args.PushMarkup(Loc.GetString("permanent-blindness-trait-examined", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", Identity.Entity(uid, this.EntityManager))
				}));
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000A28E File Offset: 0x0000848E
		private void OnShutdown(EntityUid uid, PermanentBlindnessComponent component, ComponentShutdown args)
		{
			this._blinding.AdjustBlindSources(uid, -1, null);
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000A29E File Offset: 0x0000849E
		private void OnStartup(EntityUid uid, PermanentBlindnessComponent component, ComponentStartup args)
		{
			this._blinding.AdjustBlindSources(uid, 1, null);
		}

		// Token: 0x04000267 RID: 615
		[Dependency]
		private readonly INetManager _net;

		// Token: 0x04000268 RID: 616
		[Dependency]
		private readonly SharedBlindingSystem _blinding;
	}
}
