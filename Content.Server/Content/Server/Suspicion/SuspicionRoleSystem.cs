using System;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server.Suspicion
{
	// Token: 0x02000137 RID: 311
	public sealed class SuspicionRoleSystem : EntitySystem
	{
		// Token: 0x060005B3 RID: 1459 RVA: 0x0001C010 File Offset: 0x0001A210
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SuspicionRoleComponent, ExaminedEvent>(new ComponentEventHandler<SuspicionRoleComponent, ExaminedEvent>(this.OnExamined), null, null);
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x0001C02C File Offset: 0x0001A22C
		[NullableContext(1)]
		private void OnExamined(EntityUid uid, SuspicionRoleComponent component, ExaminedEvent args)
		{
			if (!component.IsDead())
			{
				return;
			}
			bool flag = component.IsTraitor();
			string color = flag ? "red" : "green";
			string role = flag ? "suspicion-role-component-role-traitor" : "suspicion-role-component-role-innocent";
			string article = flag ? "generic-article-a" : "generic-article-an";
			string tooltip = Loc.GetString("suspicion-role-component-on-examine-tooltip", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("article", Loc.GetString(article)),
				new ValueTuple<string, object>("colorName", color),
				new ValueTuple<string, object>("role", Loc.GetString(role))
			});
			args.PushMarkup(tooltip);
		}
	}
}
