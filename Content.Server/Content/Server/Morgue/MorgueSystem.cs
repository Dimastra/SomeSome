using System;
using System.Runtime.CompilerServices;
using Content.Server.Morgue.Components;
using Content.Server.Storage.Components;
using Content.Shared.Body.Components;
using Content.Shared.Examine;
using Content.Shared.Morgue;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Morgue
{
	// Token: 0x02000398 RID: 920
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MorgueSystem : EntitySystem
	{
		// Token: 0x060012D7 RID: 4823 RVA: 0x00061938 File Offset: 0x0005FB38
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MorgueComponent, ExaminedEvent>(new ComponentEventHandler<MorgueComponent, ExaminedEvent>(this.OnExamine), null, null);
		}

		// Token: 0x060012D8 RID: 4824 RVA: 0x00061954 File Offset: 0x0005FB54
		private void OnExamine(EntityUid uid, MorgueComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			MorgueContents contents;
			this._appearance.TryGetData<MorgueContents>(uid, MorgueVisuals.Contents, ref contents, null);
			string text2;
			switch (contents)
			{
			case MorgueContents.HasMob:
				text2 = "morgue-entity-storage-component-on-examine-details-body-has-no-soul";
				break;
			case MorgueContents.HasSoul:
				text2 = "morgue-entity-storage-component-on-examine-details-body-has-soul";
				break;
			case MorgueContents.HasContents:
				text2 = "morgue-entity-storage-component-on-examine-details-has-contents";
				break;
			default:
				text2 = "morgue-entity-storage-component-on-examine-details-empty";
				break;
			}
			string text = text2;
			args.PushMarkup(Loc.GetString(text));
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x000619C4 File Offset: 0x0005FBC4
		[NullableContext(2)]
		private void CheckContents(EntityUid uid, MorgueComponent morgue = null, EntityStorageComponent storage = null, AppearanceComponent app = null)
		{
			if (!base.Resolve<MorgueComponent, EntityStorageComponent, AppearanceComponent>(uid, ref morgue, ref storage, ref app, true))
			{
				return;
			}
			if (storage.Contents.ContainedEntities.Count == 0)
			{
				this._appearance.SetData(uid, MorgueVisuals.Contents, MorgueContents.Empty, null);
				return;
			}
			bool hasMob = false;
			foreach (EntityUid ent in storage.Contents.ContainedEntities)
			{
				if (!hasMob && base.HasComp<BodyComponent>(ent))
				{
					hasMob = true;
				}
				if (base.HasComp<ActorComponent>(ent))
				{
					this._appearance.SetData(uid, MorgueVisuals.Contents, MorgueContents.HasSoul, app);
					return;
				}
			}
			this._appearance.SetData(uid, MorgueVisuals.Contents, hasMob ? MorgueContents.HasMob : MorgueContents.HasContents, app);
		}

		// Token: 0x060012DA RID: 4826 RVA: 0x00061AA4 File Offset: 0x0005FCA4
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<MorgueComponent, EntityStorageComponent, AppearanceComponent> valueTuple in base.EntityQuery<MorgueComponent, EntityStorageComponent, AppearanceComponent>(false))
			{
				MorgueComponent comp = valueTuple.Item1;
				EntityStorageComponent storage = valueTuple.Item2;
				AppearanceComponent appearance = valueTuple.Item3;
				comp.AccumulatedFrameTime += frameTime;
				this.CheckContents(comp.Owner, comp, storage, null);
				if (comp.AccumulatedFrameTime >= comp.BeepTime)
				{
					comp.AccumulatedFrameTime -= comp.BeepTime;
					MorgueContents contents;
					if (comp.DoSoulBeep && this._appearance.TryGetData<MorgueContents>(appearance.Owner, MorgueVisuals.Contents, ref contents, appearance) && contents == MorgueContents.HasSoul)
					{
						this._audio.PlayPvs(comp.OccupantHasSoulAlarmSound, comp.Owner, null);
					}
				}
			}
		}

		// Token: 0x04000B84 RID: 2948
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000B85 RID: 2949
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
