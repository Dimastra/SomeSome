using System;
using System.Runtime.CompilerServices;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Sound.Components;
using Content.Server.UserInterface;
using Content.Shared.Popups;
using Content.Shared.Sound;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Server.Sound
{
	// Token: 0x020001DA RID: 474
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EmitSoundSystem : SharedEmitSoundSystem
	{
		// Token: 0x0600090B RID: 2315 RVA: 0x0002D9F0 File Offset: 0x0002BBF0
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (SpamEmitSoundComponent soundSpammer in base.EntityQuery<SpamEmitSoundComponent>(false))
			{
				if (soundSpammer.Enabled)
				{
					soundSpammer.Accumulator += frameTime;
					if (soundSpammer.Accumulator >= soundSpammer.RollInterval)
					{
						soundSpammer.Accumulator -= soundSpammer.RollInterval;
						if (RandomExtensions.Prob(this.Random, soundSpammer.PlayChance))
						{
							if (soundSpammer.PopUp != null)
							{
								this.Popup.PopupEntity(Loc.GetString(soundSpammer.PopUp), soundSpammer.Owner, PopupType.Small);
							}
							base.TryEmitSound(soundSpammer, null, true);
						}
					}
				}
			}
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x0002DAC4 File Offset: 0x0002BCC4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EmitSoundOnTriggerComponent, TriggerEvent>(new ComponentEventHandler<EmitSoundOnTriggerComponent, TriggerEvent>(this.HandleEmitSoundOnTrigger), null, null);
			base.SubscribeLocalEvent<EmitSoundOnUIOpenComponent, AfterActivatableUIOpenEvent>(new ComponentEventHandler<EmitSoundOnUIOpenComponent, AfterActivatableUIOpenEvent>(this.HandleEmitSoundOnUIOpen), null, null);
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x0002DAF4 File Offset: 0x0002BCF4
		private void HandleEmitSoundOnUIOpen(EntityUid eUI, EmitSoundOnUIOpenComponent component, AfterActivatableUIOpenEvent args)
		{
			base.TryEmitSound(component, new EntityUid?(args.User), true);
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x0002DB09 File Offset: 0x0002BD09
		private void HandleEmitSoundOnTrigger(EntityUid uid, EmitSoundOnTriggerComponent component, TriggerEvent args)
		{
			base.TryEmitSound(component, args.User, true);
			args.Handled = true;
		}
	}
}
