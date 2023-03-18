using System;
using System.Runtime.CompilerServices;
using Content.Client.Items;
using Content.Client.Radiation.UI;
using Content.Shared.Radiation.Components;
using Content.Shared.Radiation.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Radiation.Systems
{
	// Token: 0x02000179 RID: 377
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GeigerSystem : SharedGeigerSystem
	{
		// Token: 0x060009CB RID: 2507 RVA: 0x00038EB0 File Offset: 0x000370B0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PlayerAttachSysMessage>(new EntityEventHandler<PlayerAttachSysMessage>(this.OnAttachedEntityChanged), null, null);
			base.SubscribeLocalEvent<GeigerComponent, ComponentHandleState>(new ComponentEventRefHandler<GeigerComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<GeigerComponent, ItemStatusCollectMessage>(new ComponentEventHandler<GeigerComponent, ItemStatusCollectMessage>(this.OnGetStatusMessage), null, null);
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x00038F00 File Offset: 0x00037100
		private void OnHandleState(EntityUid uid, GeigerComponent component, ref ComponentHandleState args)
		{
			GeigerComponentState geigerComponentState = args.Current as GeigerComponentState;
			if (geigerComponentState == null)
			{
				return;
			}
			this.UpdateGeigerSound(uid, geigerComponentState.IsEnabled, geigerComponentState.User, geigerComponentState.DangerLevel, false, component);
			component.CurrentRadiation = geigerComponentState.CurrentRadiation;
			component.DangerLevel = geigerComponentState.DangerLevel;
			component.IsEnabled = geigerComponentState.IsEnabled;
			component.User = geigerComponentState.User;
			component.UiUpdateNeeded = true;
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x00038F6F File Offset: 0x0003716F
		private void OnGetStatusMessage(EntityUid uid, GeigerComponent component, ItemStatusCollectMessage args)
		{
			if (!component.ShowControl)
			{
				return;
			}
			args.Controls.Add(new GeigerItemControl(component));
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x00038F8C File Offset: 0x0003718C
		private void OnAttachedEntityChanged(PlayerAttachSysMessage ev)
		{
			foreach (GeigerComponent geigerComponent in base.EntityQuery<GeigerComponent>(false))
			{
				this.ForceUpdateGeigerSound(geigerComponent.Owner, geigerComponent);
			}
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x00038FE0 File Offset: 0x000371E0
		[NullableContext(2)]
		private void ForceUpdateGeigerSound(EntityUid uid, GeigerComponent component = null)
		{
			if (!base.Resolve<GeigerComponent>(uid, ref component, true))
			{
				return;
			}
			this.UpdateGeigerSound(uid, component.IsEnabled, component.User, component.DangerLevel, true, component);
		}

		// Token: 0x060009D0 RID: 2512 RVA: 0x0003900C File Offset: 0x0003720C
		[NullableContext(2)]
		private void UpdateGeigerSound(EntityUid uid, bool isEnabled, EntityUid? user, GeigerDangerLevel dangerLevel, bool force = false, GeigerComponent component = null)
		{
			if (!base.Resolve<GeigerComponent>(uid, ref component, true))
			{
				return;
			}
			if (!force && isEnabled == component.IsEnabled && user == component.User && dangerLevel == component.DangerLevel)
			{
				return;
			}
			IPlayingAudioStream stream = component.Stream;
			if (stream != null)
			{
				stream.Stop();
			}
			if (!isEnabled || user == null)
			{
				return;
			}
			SoundSpecifier soundSpecifier;
			if (!component.Sounds.TryGetValue(dangerLevel, out soundSpecifier))
			{
				return;
			}
			if (this._playerManager.LocalPlayer == null)
			{
				return;
			}
			if (this._playerManager.LocalPlayer.Session.AttachedEntity != user)
			{
				return;
			}
			string sound = this._audio.GetSound(soundSpecifier);
			AudioParams value = soundSpecifier.Params.WithLoop(true).WithVolume(-4f);
			component.Stream = this._audio.Play(sound, Filter.Local(), uid, false, new AudioParams?(value));
		}

		// Token: 0x040004DE RID: 1246
		[Dependency]
		private readonly AudioSystem _audio;

		// Token: 0x040004DF RID: 1247
		[Dependency]
		private readonly IPlayerManager _playerManager;
	}
}
