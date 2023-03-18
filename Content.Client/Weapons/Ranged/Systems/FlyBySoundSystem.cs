using System;
using System.Runtime.CompilerServices;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Client.Weapons.Ranged.Systems
{
	// Token: 0x0200002D RID: 45
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FlyBySoundSystem : SharedFlyBySoundSystem
	{
		// Token: 0x060000B9 RID: 185 RVA: 0x00006B2D File Offset: 0x00004D2D
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FlyBySoundComponent, StartCollideEvent>(new ComponentEventRefHandler<FlyBySoundComponent, StartCollideEvent>(this.OnCollide), null, null);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00006B4C File Offset: 0x00004D4C
		private void OnCollide(EntityUid uid, FlyBySoundComponent component, ref StartCollideEvent args)
		{
			LocalPlayer localPlayer = this._player.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			ProjectileComponent projectileComponent;
			if (entityUid == null || args.OtherFixture.Body.Owner != entityUid || (base.TryComp<ProjectileComponent>(args.OurFixture.Body.Owner, ref projectileComponent) && projectileComponent.Shooter == entityUid))
			{
				return;
			}
			if (args.OurFixture.ID != "fly-by" || !RandomExtensions.Prob(this._random, component.Prob))
			{
				return;
			}
			this._audio.Play(component.Sound, Filter.Local(), entityUid.Value, false, null);
		}

		// Token: 0x0400007E RID: 126
		[Dependency]
		private readonly IPlayerManager _player;

		// Token: 0x0400007F RID: 127
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000080 RID: 128
		[Dependency]
		private readonly SharedAudioSystem _audio;
	}
}
