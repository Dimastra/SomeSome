using System;
using System.Runtime.CompilerServices;
using Content.Client.Light.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;

namespace Content.Client.Light.EntitySystems
{
	// Token: 0x0200026A RID: 618
	public sealed class ExpendableLightSystem : EntitySystem
	{
		// Token: 0x06000FC2 RID: 4034 RVA: 0x0005F02F File Offset: 0x0005D22F
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ExpendableLightComponent, ComponentShutdown>(new ComponentEventHandler<ExpendableLightComponent, ComponentShutdown>(this.OnLightShutdown), null, null);
		}

		// Token: 0x06000FC3 RID: 4035 RVA: 0x0005F04B File Offset: 0x0005D24B
		[NullableContext(1)]
		private void OnLightShutdown(EntityUid uid, ExpendableLightComponent component, ComponentShutdown args)
		{
			IPlayingAudioStream playingStream = component.PlayingStream;
			if (playingStream == null)
			{
				return;
			}
			playingStream.Stop();
		}
	}
}
