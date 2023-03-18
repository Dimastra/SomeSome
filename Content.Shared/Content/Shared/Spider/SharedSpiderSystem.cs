using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Spider
{
	// Token: 0x02000175 RID: 373
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedSpiderSystem : EntitySystem
	{
		// Token: 0x06000482 RID: 1154 RVA: 0x00011DCC File Offset: 0x0000FFCC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SpiderWebObjectComponent, ComponentStartup>(new ComponentEventHandler<SpiderWebObjectComponent, ComponentStartup>(this.OnWebStartup), null, null);
			base.SubscribeLocalEvent<SpiderComponent, ComponentStartup>(new ComponentEventHandler<SpiderComponent, ComponentStartup>(this.OnSpiderStartup), null, null);
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x00011DFC File Offset: 0x0000FFFC
		private void OnSpiderStartup(EntityUid uid, SpiderComponent component, ComponentStartup args)
		{
			InstantAction netAction = new InstantAction(this._proto.Index<InstantActionPrototype>(component.WebActionName));
			this._action.AddAction(uid, netAction, null, null, true);
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x00011E38 File Offset: 0x00010038
		private void OnWebStartup(EntityUid uid, SpiderWebObjectComponent component, ComponentStartup args)
		{
			this._appearance.SetData(uid, SpiderWebVisuals.Variant, this._robustRandom.Next(1, 3), null);
		}

		// Token: 0x0400043C RID: 1084
		[Dependency]
		private readonly SharedActionsSystem _action;

		// Token: 0x0400043D RID: 1085
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x0400043E RID: 1086
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x0400043F RID: 1087
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
