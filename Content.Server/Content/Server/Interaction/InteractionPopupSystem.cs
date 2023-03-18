using System;
using System.Runtime.CompilerServices;
using Content.Server.Interaction.Components;
using Content.Server.Popups;
using Content.Shared.Bed.Sleep;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Interaction
{
	// Token: 0x02000445 RID: 1093
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InteractionPopupSystem : EntitySystem
	{
		// Token: 0x0600160B RID: 5643 RVA: 0x000747E0 File Offset: 0x000729E0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<InteractionPopupComponent, InteractHandEvent>(new ComponentEventHandler<InteractionPopupComponent, InteractHandEvent>(this.OnInteractHand), null, null);
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x000747FC File Offset: 0x000729FC
		private void OnInteractHand(EntityUid uid, InteractionPopupComponent component, InteractHandEvent args)
		{
			if (args.Handled || args.User == args.Target)
			{
				return;
			}
			if (base.HasComp<SleepingComponent>(uid))
			{
				return;
			}
			args.Handled = true;
			TimeSpan curTime = this._gameTiming.CurTime;
			if (curTime < component.LastInteractTime + component.InteractDelay)
			{
				return;
			}
			MobStateComponent state;
			if (base.TryComp<MobStateComponent>(uid, ref state) && !this._mobStateSystem.IsAlive(uid, state))
			{
				return;
			}
			string msg = "";
			string sfx = null;
			if (RandomExtensions.Prob(this._random, component.SuccessChance))
			{
				if (component.InteractSuccessString != null)
				{
					msg = Loc.GetString(component.InteractSuccessString, new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("target", Identity.Entity(uid, this.EntityManager))
					});
				}
				if (component.InteractSuccessSound != null)
				{
					sfx = component.InteractSuccessSound.GetSound(null, null);
				}
			}
			else
			{
				if (component.InteractFailureString != null)
				{
					msg = Loc.GetString(component.InteractFailureString, new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("target", Identity.Entity(uid, this.EntityManager))
					});
				}
				if (component.InteractFailureSound != null)
				{
					sfx = component.InteractFailureSound.GetSound(null, null);
				}
			}
			if (component.MessagePerceivedByOthers != null)
			{
				string msgOthers = Loc.GetString(component.MessagePerceivedByOthers, new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("user", Identity.Entity(args.User, this.EntityManager)),
					new ValueTuple<string, object>("target", Identity.Entity(uid, this.EntityManager))
				});
				this._popupSystem.PopupEntity(msg, uid, args.User, PopupType.Small);
				this._popupSystem.PopupEntity(msgOthers, uid, Filter.PvsExcept(args.User, 2f, this.EntityManager), true, PopupType.Small);
			}
			else
			{
				this._popupSystem.PopupEntity(msg, uid, args.User, PopupType.Small);
			}
			if (sfx != null)
			{
				if (component.SoundPerceivedByOthers)
				{
					SoundSystem.Play(sfx, Filter.Pvs(args.Target, 2f, null, null, null), args.Target, null);
				}
				else
				{
					SoundSystem.Play(sfx, Filter.Entities(new EntityUid[]
					{
						args.User,
						args.Target
					}), args.Target, null);
				}
			}
			component.LastInteractTime = curTime;
		}

		// Token: 0x04000DCA RID: 3530
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000DCB RID: 3531
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000DCC RID: 3532
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000DCD RID: 3533
		[Dependency]
		private readonly PopupSystem _popupSystem;
	}
}
