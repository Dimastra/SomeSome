using System;
using System.Runtime.CompilerServices;
using Content.Client.Reflection;
using Content.Shared.CCVar;
using Robust.Client.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Reflection;

namespace Content.Client.Audio
{
	// Token: 0x02000431 RID: 1073
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UIAudioManager
	{
		// Token: 0x06001A33 RID: 6707 RVA: 0x00095DCD File Offset: 0x00093FCD
		public void Initialize()
		{
			IoCManager.InjectDependencies<UIAudioManager>(this);
		}

		// Token: 0x06001A34 RID: 6708 RVA: 0x00095DD6 File Offset: 0x00093FD6
		[NullableContext(2)]
		private SharedAudioSystem GetAudioSystem()
		{
			if (!this._refl.IsInIntegrationTest())
			{
				return IoCManager.Resolve<IEntitySystemManager>().GetEntitySystemOrNull<SharedAudioSystem>();
			}
			return null;
		}

		// Token: 0x06001A35 RID: 6709 RVA: 0x00095DF4 File Offset: 0x00093FF4
		[return: Nullable(2)]
		public AudioSystem.PlayingStream Play(string filename, AudioParams? audioParams = null)
		{
			SharedAudioSystem audioSystem = this.GetAudioSystem();
			if (audioSystem == null)
			{
				return null;
			}
			AudioParams value = audioParams.GetValueOrDefault();
			if (audioParams == null)
			{
				value = AudioParams.Default;
				audioParams = new AudioParams?(value);
			}
			audioParams = new AudioParams?(audioParams.Value.WithVolume(audioParams.Value.Volume + this._cfg.GetCVar<float>(CCVars.UIVolume)));
			return audioSystem.PlayGlobal(filename, Filter.Local(), false, audioParams) as AudioSystem.PlayingStream;
		}

		// Token: 0x04000D56 RID: 3414
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000D57 RID: 3415
		[Dependency]
		private readonly IReflectionManager _refl;
	}
}
