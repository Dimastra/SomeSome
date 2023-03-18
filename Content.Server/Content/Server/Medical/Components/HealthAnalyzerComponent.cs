using System;
using System.Runtime.CompilerServices;
using Content.Server.UserInterface;
using Content.Shared.Disease;
using Content.Shared.MedicalScanner;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Medical.Components
{
	// Token: 0x020003BD RID: 957
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SharedHealthAnalyzerComponent))]
	public sealed class HealthAnalyzerComponent : SharedHealthAnalyzerComponent
	{
		// Token: 0x170002CE RID: 718
		// (get) Token: 0x060013B9 RID: 5049 RVA: 0x00066131 File Offset: 0x00064331
		public BoundUserInterface UserInterface
		{
			get
			{
				return base.Owner.GetUIOrNull(SharedHealthAnalyzerComponent.HealthAnalyzerUiKey.Key);
			}
		}

		// Token: 0x04000C16 RID: 3094
		[DataField("scanDelay", false, 1, false, false, null)]
		public float ScanDelay = 0.8f;

		// Token: 0x04000C17 RID: 3095
		[DataField("scanningBeginSound", false, 1, false, false, null)]
		public SoundSpecifier ScanningBeginSound;

		// Token: 0x04000C18 RID: 3096
		[DataField("scanningEndSound", false, 1, false, false, null)]
		public SoundSpecifier ScanningEndSound;

		// Token: 0x04000C19 RID: 3097
		[DataField("disease", false, 1, false, false, typeof(PrototypeIdSerializer<DiseasePrototype>))]
		[ViewVariables]
		public string Disease;
	}
}
