using System;
using System.Runtime.CompilerServices;
using Content.Server.UserInterface;
using Content.Shared.PDA;
using Content.Shared.PDA.Ringer;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.PDA.Ringer
{
	// Token: 0x020002E1 RID: 737
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RingerSystem : SharedRingerSystem
	{
		// Token: 0x06000F0B RID: 3851 RVA: 0x0004D0D4 File Offset: 0x0004B2D4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RingerComponent, MapInitEvent>(new ComponentEventHandler<RingerComponent, MapInitEvent>(this.RandomizeRingtone), null, null);
			base.SubscribeLocalEvent<RingerComponent, RingerSetRingtoneMessage>(new ComponentEventHandler<RingerComponent, RingerSetRingtoneMessage>(this.OnSetRingtone), null, null);
			base.SubscribeLocalEvent<RingerComponent, RingerPlayRingtoneMessage>(new ComponentEventHandler<RingerComponent, RingerPlayRingtoneMessage>(this.RingerPlayRingtone), null, null);
			base.SubscribeLocalEvent<RingerComponent, RingerRequestUpdateInterfaceMessage>(new ComponentEventHandler<RingerComponent, RingerRequestUpdateInterfaceMessage>(this.UpdateRingerUserInterfaceDriver), null, null);
		}

		// Token: 0x06000F0C RID: 3852 RVA: 0x0004D137 File Offset: 0x0004B337
		private void RingerPlayRingtone(EntityUid uid, RingerComponent ringer, RingerPlayRingtoneMessage args)
		{
			base.EnsureComp<ActiveRingerComponent>(uid);
			this.UpdateRingerUserInterface(ringer);
		}

		// Token: 0x06000F0D RID: 3853 RVA: 0x0004D148 File Offset: 0x0004B348
		private void UpdateRingerUserInterfaceDriver(EntityUid uid, RingerComponent ringer, RingerRequestUpdateInterfaceMessage args)
		{
			this.UpdateRingerUserInterface(ringer);
		}

		// Token: 0x06000F0E RID: 3854 RVA: 0x0004D151 File Offset: 0x0004B351
		private void OnSetRingtone(EntityUid uid, RingerComponent ringer, RingerSetRingtoneMessage args)
		{
			if (args.Ringtone.Length != 4)
			{
				return;
			}
			this.UpdateRingerRingtone(ringer, args.Ringtone);
		}

		// Token: 0x06000F0F RID: 3855 RVA: 0x0004D170 File Offset: 0x0004B370
		public void RandomizeRingtone(EntityUid uid, RingerComponent ringer, MapInitEvent args)
		{
			Note[] array = new Note[5];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.55FAEA8AC86EB4D1A89C0C22578092F7F74A5074C9422EAE6F30A8FAE015B51F).FieldHandle);
			Note[] notes = array;
			Note[] ringtone = new Note[4];
			for (int i = 0; i < 4; i++)
			{
				ringtone[i] = RandomExtensions.Pick<Note>(this._random, notes);
			}
			this.UpdateRingerRingtone(ringer, ringtone);
		}

		// Token: 0x06000F10 RID: 3856 RVA: 0x0004D1BA File Offset: 0x0004B3BA
		private bool UpdateRingerRingtone(RingerComponent ringer, Note[] ringtone)
		{
			ringer.Ringtone = ringtone;
			this.UpdateRingerUserInterface(ringer);
			return true;
		}

		// Token: 0x06000F11 RID: 3857 RVA: 0x0004D1CB File Offset: 0x0004B3CB
		private void UpdateRingerUserInterface(RingerComponent ringer)
		{
			BoundUserInterface uiorNull = ringer.Owner.GetUIOrNull(RingerUiKey.Key);
			if (uiorNull == null)
			{
				return;
			}
			uiorNull.SetState(new RingerUpdateState(base.HasComp<ActiveRingerComponent>(ringer.Owner), ringer.Ringtone), null, true);
		}

		// Token: 0x06000F12 RID: 3858 RVA: 0x0004D201 File Offset: 0x0004B401
		public bool ToggleRingerUI(RingerComponent ringer, IPlayerSession session)
		{
			BoundUserInterface uiorNull = ringer.Owner.GetUIOrNull(RingerUiKey.Key);
			if (uiorNull != null)
			{
				uiorNull.Toggle(session);
			}
			return true;
		}

		// Token: 0x06000F13 RID: 3859 RVA: 0x0004D224 File Offset: 0x0004B424
		public override void Update(float frameTime)
		{
			RemQueue<EntityUid> remove = default(RemQueue<EntityUid>);
			foreach (ValueTuple<ActiveRingerComponent, RingerComponent> valueTuple in this.EntityManager.EntityQuery<ActiveRingerComponent, RingerComponent>(false))
			{
				RingerComponent ringer = valueTuple.Item2;
				ringer.TimeElapsed += frameTime;
				if (ringer.TimeElapsed >= 0.2f)
				{
					ringer.TimeElapsed -= 0.2f;
					TransformComponent ringerXform = base.Transform(ringer.Owner);
					SoundSystem.Play(this.GetSound(ringer.Ringtone[ringer.NoteCount]), Filter.Empty().AddInRange(ringerXform.MapPosition, ringer.Range, null, null), ringer.Owner, new AudioParams?(AudioParams.Default.WithMaxDistance(ringer.Range).WithVolume(ringer.Volume)));
					ringer.NoteCount++;
					if (ringer.NoteCount > 3)
					{
						remove.Add(ringer.Owner);
						this.UpdateRingerUserInterface(ringer);
						ringer.TimeElapsed = 0f;
						ringer.NoteCount = 0;
						break;
					}
				}
			}
			foreach (EntityUid ent in remove)
			{
				base.RemComp<ActiveRingerComponent>(ent);
			}
		}

		// Token: 0x06000F14 RID: 3860 RVA: 0x0004D39C File Offset: 0x0004B59C
		private string GetSound(Note note)
		{
			ResourcePath resourcePath = new ResourcePath("/Audio/Effects/RingtoneNotes/" + note.ToString().ToLower(), "/");
			return ((resourcePath != null) ? resourcePath.ToString() : null) + ".ogg";
		}

		// Token: 0x040008DA RID: 2266
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
