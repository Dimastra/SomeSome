using System;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Localizations;
using Robust.Shared.IoC;

namespace Content.Shared.IoC
{
	// Token: 0x020003AA RID: 938
	public static class SharedContentIoC
	{
		// Token: 0x06000AC0 RID: 2752 RVA: 0x0002311F File Offset: 0x0002131F
		public static void Register()
		{
			IoCManager.Register<MarkingManager, MarkingManager>(false);
			IoCManager.Register<ContentLocalizationManager, ContentLocalizationManager>(false);
		}
	}
}
