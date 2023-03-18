using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.RandomAppearance
{
	// Token: 0x02000257 RID: 599
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RandomAppearanceSystem : EntitySystem
	{
		// Token: 0x06000BDE RID: 3038 RVA: 0x0003EA38 File Offset: 0x0003CC38
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RandomAppearanceComponent, ComponentInit>(new ComponentEventHandler<RandomAppearanceComponent, ComponentInit>(this.OnComponentInit), null, null);
		}

		// Token: 0x06000BDF RID: 3039 RVA: 0x0003EA54 File Offset: 0x0003CC54
		private void OnComponentInit(EntityUid uid, RandomAppearanceComponent component, ComponentInit args)
		{
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(uid, ref appearance) && component.EnumKey != null)
			{
				this._appearance.SetData(uid, component.EnumKey, RandomExtensions.Pick<string>(this._random, component.SpriteStates), appearance);
			}
		}

		// Token: 0x0400076B RID: 1899
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400076C RID: 1900
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
