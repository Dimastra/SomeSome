using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Body.Components;
using Content.Server.Mind.Components;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Mind
{
	// Token: 0x020003A3 RID: 931
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TransferMindOnGibSystem : EntitySystem
	{
		// Token: 0x0600131D RID: 4893 RVA: 0x00062E37 File Offset: 0x00061037
		public override void Initialize()
		{
			base.SubscribeLocalEvent<TransferMindOnGibComponent, BeingGibbedEvent>(new ComponentEventHandler<TransferMindOnGibComponent, BeingGibbedEvent>(this.OnGib), null, null);
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x00062E50 File Offset: 0x00061050
		private void OnGib(EntityUid uid, TransferMindOnGibComponent component, BeingGibbedEvent args)
		{
			MindComponent mindcomp;
			if (!base.TryComp<MindComponent>(uid, ref mindcomp) || mindcomp.Mind == null)
			{
				return;
			}
			HashSet<EntityUid> validParts = (from p in args.GibbedParts
			where this._tag.HasTag(p, component.TargetTag)
			select p).ToHashSet<EntityUid>();
			if (!validParts.Any<EntityUid>())
			{
				return;
			}
			EntityUid ent = RandomExtensions.Pick<EntityUid>(this._random, validParts);
			mindcomp.Mind.TransferTo(new EntityUid?(ent), false, true);
		}

		// Token: 0x04000BAD RID: 2989
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000BAE RID: 2990
		[Dependency]
		private readonly TagSystem _tag;
	}
}
