using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Audio
{
	// Token: 0x02000426 RID: 1062
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AmbientOverlayCommand : IConsoleCommand
	{
		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x060019E9 RID: 6633 RVA: 0x00094665 File Offset: 0x00092865
		public string Command
		{
			get
			{
				return "showambient";
			}
		}

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x060019EA RID: 6634 RVA: 0x0009466C File Offset: 0x0009286C
		public string Description
		{
			get
			{
				return "Shows all AmbientSoundComponents in the viewport";
			}
		}

		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x060019EB RID: 6635 RVA: 0x00094673 File Offset: 0x00092873
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x060019EC RID: 6636 RVA: 0x00094684 File Offset: 0x00092884
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			AmbientSoundSystem entitySystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<AmbientSoundSystem>();
			AmbientSoundSystem ambientSoundSystem = entitySystem;
			ambientSoundSystem.OverlayEnabled = !ambientSoundSystem.OverlayEnabled;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Ambient sound overlay set to ");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(entitySystem.OverlayEnabled);
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
