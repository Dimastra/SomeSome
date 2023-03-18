using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Mind.Components;
using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Mind
{
	// Token: 0x020003A2 RID: 930
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MindTrackerSystem : EntitySystem
	{
		// Token: 0x06001319 RID: 4889 RVA: 0x00062DC0 File Offset: 0x00060FC0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.Reset), null, null);
			base.SubscribeLocalEvent<MindComponent, MindAddedMessage>(new ComponentEventHandler<MindComponent, MindAddedMessage>(this.OnMindAdded), null, null);
		}

		// Token: 0x0600131A RID: 4890 RVA: 0x00062DF0 File Offset: 0x00060FF0
		private void Reset(RoundRestartCleanupEvent ev)
		{
			this.AllMinds.Clear();
		}

		// Token: 0x0600131B RID: 4891 RVA: 0x00062E00 File Offset: 0x00061000
		private void OnMindAdded(EntityUid uid, MindComponent mc, MindAddedMessage args)
		{
			Mind mind = mc.Mind;
			if (mind != null)
			{
				this.AllMinds.Add(mind);
			}
		}

		// Token: 0x04000BAC RID: 2988
		[ViewVariables]
		public readonly HashSet<Mind> AllMinds = new HashSet<Mind>();
	}
}
