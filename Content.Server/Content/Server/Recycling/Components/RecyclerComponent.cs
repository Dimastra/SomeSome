using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Recycling.Components
{
	// Token: 0x0200024A RID: 586
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(RecyclerSystem)
	})]
	public sealed class RecyclerComponent : Component
	{
		// Token: 0x04000738 RID: 1848
		[Nullable(1)]
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04000739 RID: 1849
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled;

		// Token: 0x0400073A RID: 1850
		[ViewVariables]
		[DataField("efficiency", false, 1, false, false, null)]
		internal float Efficiency = 0.25f;

		// Token: 0x0400073B RID: 1851
		[Nullable(2)]
		[ViewVariables]
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/Effects/saw.ogg", null);

		// Token: 0x0400073C RID: 1852
		public TimeSpan LastSound;

		// Token: 0x0400073D RID: 1853
		public int ItemsProcessed;
	}
}
