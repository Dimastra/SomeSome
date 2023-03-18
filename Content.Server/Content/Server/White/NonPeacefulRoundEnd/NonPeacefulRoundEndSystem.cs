using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking;
using Content.Server.Traits.Assorted;
using Content.Shared.CCVar;
using Content.Shared.CombatMode.Pacification;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Humanoid;
using Content.Shared.White.NonPeacefulRoundEnd;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.White.NonPeacefulRoundEnd
{
	// Token: 0x02000094 RID: 148
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NonPeacefulRoundEndSystem : EntitySystem
	{
		// Token: 0x0600024C RID: 588 RVA: 0x0000C870 File Offset: 0x0000AA70
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RoundEndTextAppendEvent>(new EntityEventHandler<RoundEndTextAppendEvent>(this.OnRoundEnded), null, null);
			this._cfg.OnValueChanged<bool>(CCVars.NonPeacefulRoundEndEnabled, delegate(bool value)
			{
				this._enabled = value;
			}, true);
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000C8AC File Offset: 0x0000AAAC
		private void OnRoundEnded(RoundEndTextAppendEvent ev)
		{
			if (!this._enabled)
			{
				return;
			}
			List<NonPeacefulRoundItemsPrototype> prototypes = this._prototypeManager.EnumeratePrototypes<NonPeacefulRoundItemsPrototype>().ToList<NonPeacefulRoundItemsPrototype>();
			if (prototypes.Count < 1)
			{
				return;
			}
			this._nonPeacefulRoundItemsPrototype = RandomExtensions.Pick<NonPeacefulRoundItemsPrototype>(this._robustRandom, prototypes);
			foreach (ICommonSession session in this._playerManager.Sessions)
			{
				if (session.AttachedEntity != null)
				{
					base.RemComp<PacifiedComponent>(session.AttachedEntity.Value);
					base.RemComp<PacifistComponent>(session.AttachedEntity.Value);
					this.GiveItem(session.AttachedEntity.Value);
				}
			}
			int announceCount = this._robustRandom.Next(5, 15);
			for (int i = 0; i <= announceCount; i++)
			{
				this._chatManager.SendAdminAnnouncement("!!!РЕЗНЯ!!!");
			}
			this._sharedAudioSystem.PlayGlobal("/Audio/White/RoundEnd/rezniya.ogg", Filter.Broadcast(), false, null);
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000C9D0 File Offset: 0x0000ABD0
		private void GiveItem(EntityUid player)
		{
			string item = RandomExtensions.Pick<string>(this._robustRandom, this._nonPeacefulRoundItemsPrototype.Items);
			TransformComponent transform = base.CompOrNull<TransformComponent>(player);
			if (transform == null)
			{
				return;
			}
			if (!base.HasComp<HumanoidAppearanceComponent>(player))
			{
				return;
			}
			if (!base.HasComp<SharedHandsComponent>(player))
			{
				return;
			}
			EntityUid weaponEntity = this._entityManager.SpawnEntity(item, transform.Coordinates);
			this._handsSystem.TryDrop(player, null, true, true, null);
			this._handsSystem.PickupOrDrop(new EntityUid?(player), weaponEntity, true, false, null, null);
		}

		// Token: 0x040001A0 RID: 416
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x040001A1 RID: 417
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x040001A2 RID: 418
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040001A3 RID: 419
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x040001A4 RID: 420
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x040001A5 RID: 421
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x040001A6 RID: 422
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x040001A7 RID: 423
		[Dependency]
		private readonly SharedAudioSystem _sharedAudioSystem;

		// Token: 0x040001A8 RID: 424
		private NonPeacefulRoundItemsPrototype _nonPeacefulRoundItemsPrototype;

		// Token: 0x040001A9 RID: 425
		private bool _enabled;
	}
}
