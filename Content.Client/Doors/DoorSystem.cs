using System;
using System.Runtime.CompilerServices;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Content.Client.Doors
{
	// Token: 0x02000346 RID: 838
	public sealed class DoorSystem : SharedDoorSystem
	{
		// Token: 0x060014C9 RID: 5321 RVA: 0x00079FF0 File Offset: 0x000781F0
		[NullableContext(2)]
		protected override void UpdateAppearance(EntityUid uid, DoorComponent door = null)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return;
			}
			base.UpdateAppearance(uid, door);
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				spriteComponent.DrawDepth = ((door.State == DoorState.Open) ? door.OpenDrawDepth : door.ClosedDrawDepth);
			}
		}

		// Token: 0x060014CA RID: 5322 RVA: 0x0007A03A File Offset: 0x0007823A
		[NullableContext(1)]
		protected override void PlaySound(EntityUid uid, SoundSpecifier soundSpecifier, AudioParams audioParams, EntityUid? predictingPlayer, bool predicted)
		{
			if (this.GameTiming.InPrediction && this.GameTiming.IsFirstTimePredicted)
			{
				this.Audio.Play(soundSpecifier, Filter.Local(), uid, false, new AudioParams?(audioParams));
			}
		}
	}
}
