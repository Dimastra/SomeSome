using System;
using System.Runtime.CompilerServices;
using Content.Server.Ghost.Components;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server.Warps
{
	// Token: 0x020000BD RID: 189
	public sealed class WarpPointSystem : EntitySystem
	{
		// Token: 0x0600031E RID: 798 RVA: 0x00010CD7 File Offset: 0x0000EED7
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<WarpPointComponent, ExaminedEvent>(new ComponentEventHandler<WarpPointComponent, ExaminedEvent>(this.OnWarpPointExamine), null, null);
		}

		// Token: 0x0600031F RID: 799 RVA: 0x00010CF4 File Offset: 0x0000EEF4
		[NullableContext(1)]
		private void OnWarpPointExamine(EntityUid uid, WarpPointComponent component, ExaminedEvent args)
		{
			if (!base.HasComp<GhostComponent>(args.Examiner))
			{
				return;
			}
			string loc = (component.Location == null) ? "<null>" : ("'" + component.Location + "'");
			args.PushText(Loc.GetString("warp-point-component-on-examine-success", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("location", loc)
			}));
		}
	}
}
