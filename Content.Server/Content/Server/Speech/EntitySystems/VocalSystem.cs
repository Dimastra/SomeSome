using System;
using System.Runtime.CompilerServices;
using Content.Server.Actions;
using Content.Server.Chat.Systems;
using Content.Server.Speech.Components;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Humanoid;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Speech.EntitySystems
{
	// Token: 0x020001C0 RID: 448
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VocalSystem : EntitySystem
	{
		// Token: 0x060008C2 RID: 2242 RVA: 0x0002CE38 File Offset: 0x0002B038
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<VocalComponent, MapInitEvent>(new ComponentEventHandler<VocalComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<VocalComponent, ComponentShutdown>(new ComponentEventHandler<VocalComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<VocalComponent, SexChangedEvent>(new ComponentEventHandler<VocalComponent, SexChangedEvent>(this.OnSexChanged), null, null);
			base.SubscribeLocalEvent<VocalComponent, EmoteEvent>(new ComponentEventRefHandler<VocalComponent, EmoteEvent>(this.OnEmote), null, null);
			base.SubscribeLocalEvent<VocalComponent, ScreamActionEvent>(new ComponentEventHandler<VocalComponent, ScreamActionEvent>(this.OnScreamAction), null, null);
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x0002CEB0 File Offset: 0x0002B0B0
		private void OnMapInit(EntityUid uid, VocalComponent component, MapInitEvent args)
		{
			InstantActionPrototype proto;
			if (this._proto.TryIndex<InstantActionPrototype>(component.ScreamActionId, ref proto))
			{
				component.ScreamAction = new InstantAction(proto);
				this._actions.AddAction(uid, component.ScreamAction, null, null, true);
			}
			this.LoadSounds(uid, component, null);
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x0002CF0C File Offset: 0x0002B10C
		private void OnShutdown(EntityUid uid, VocalComponent component, ComponentShutdown args)
		{
			if (component.ScreamAction != null)
			{
				this._actions.RemoveAction(uid, component.ScreamAction, null);
			}
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x0002CF2C File Offset: 0x0002B12C
		private void OnSexChanged(EntityUid uid, VocalComponent component, SexChangedEvent args)
		{
			this.LoadSounds(uid, component, null);
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x0002CF4C File Offset: 0x0002B14C
		private void OnEmote(EntityUid uid, VocalComponent component, ref EmoteEvent args)
		{
			if (args.Handled || !args.Emote.Category.HasFlag(EmoteCategory.Vocal))
			{
				return;
			}
			if (args.Emote.ID == component.ScreamId)
			{
				args.Handled = this.TryPlayScreamSound(uid, component);
				return;
			}
			args.Handled = this._chat.TryPlayEmoteSound(uid, component.EmoteSounds, args.Emote);
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x0002CFC4 File Offset: 0x0002B1C4
		private void OnScreamAction(EntityUid uid, VocalComponent component, ScreamActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this._chat.TryEmoteWithChat(uid, component.ScreamActionId, false, false, null);
			args.Handled = true;
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x0002CFEC File Offset: 0x0002B1EC
		private bool TryPlayScreamSound(EntityUid uid, VocalComponent component)
		{
			if (RandomExtensions.Prob(this._random, component.WilhelmProbability))
			{
				this._audio.PlayPvs(component.Wilhelm, uid, new AudioParams?(component.Wilhelm.Params));
				return true;
			}
			return this._chat.TryPlayEmoteSound(uid, component.EmoteSounds, component.ScreamId);
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x0002D04C File Offset: 0x0002B24C
		private void LoadSounds(EntityUid uid, VocalComponent component, Sex? sex = null)
		{
			if (component.Sounds == null)
			{
				return;
			}
			Sex value = sex.GetValueOrDefault();
			if (sex == null)
			{
				HumanoidAppearanceComponent humanoidAppearanceComponent = base.CompOrNull<HumanoidAppearanceComponent>(uid);
				value = ((humanoidAppearanceComponent != null) ? humanoidAppearanceComponent.Sex : Sex.Unsexed);
				sex = new Sex?(value);
			}
			string protoId;
			if (!component.Sounds.TryGetValue(sex.Value, out protoId))
			{
				return;
			}
			this._proto.TryIndex<EmoteSoundsPrototype>(protoId, ref component.EmoteSounds);
		}

		// Token: 0x0400054A RID: 1354
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400054B RID: 1355
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x0400054C RID: 1356
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x0400054D RID: 1357
		[Dependency]
		private readonly ChatSystem _chat;

		// Token: 0x0400054E RID: 1358
		[Dependency]
		private readonly ActionsSystem _actions;
	}
}
