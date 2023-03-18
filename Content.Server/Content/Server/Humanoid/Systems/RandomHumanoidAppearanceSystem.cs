using System;
using System.Runtime.CompilerServices;
using Content.Server.CharacterAppearance.Components;
using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Humanoid.Systems
{
	// Token: 0x0200045A RID: 1114
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RandomHumanoidAppearanceSystem : EntitySystem
	{
		// Token: 0x06001686 RID: 5766 RVA: 0x000770C3 File Offset: 0x000752C3
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RandomHumanoidAppearanceComponent, MapInitEvent>(new ComponentEventHandler<RandomHumanoidAppearanceComponent, MapInitEvent>(this.OnMapInit), null, null);
		}

		// Token: 0x06001687 RID: 5767 RVA: 0x000770E0 File Offset: 0x000752E0
		private void OnMapInit(EntityUid uid, RandomHumanoidAppearanceComponent component, MapInitEvent args)
		{
			HumanoidAppearanceComponent humanoid;
			if (!base.TryComp<HumanoidAppearanceComponent>(uid, ref humanoid) || !string.IsNullOrEmpty(humanoid.Initial))
			{
				return;
			}
			HumanoidCharacterProfile profile = HumanoidCharacterProfile.RandomWithSpecies(humanoid.Species);
			this._humanoid.LoadProfile(uid, profile, humanoid);
			if (component.RandomizeName)
			{
				base.MetaData(uid).EntityName = profile.Name;
			}
		}

		// Token: 0x04000E10 RID: 3600
		[Dependency]
		private readonly HumanoidAppearanceSystem _humanoid;
	}
}
