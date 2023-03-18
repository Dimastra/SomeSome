using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006A6 RID: 1702
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class HyposprayComponent : SharedHyposprayComponent
	{
		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x0600237B RID: 9083 RVA: 0x000B9977 File Offset: 0x000B7B77
		// (set) Token: 0x0600237C RID: 9084 RVA: 0x000B997F File Offset: 0x000B7B7F
		[DataField("clumsyFailChance", false, 1, false, false, null)]
		[ViewVariables]
		public float ClumsyFailChance { get; set; } = 0.5f;

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x0600237D RID: 9085 RVA: 0x000B9988 File Offset: 0x000B7B88
		// (set) Token: 0x0600237E RID: 9086 RVA: 0x000B9990 File Offset: 0x000B7B90
		[DataField("transferAmount", false, 1, false, false, null)]
		[ViewVariables]
		public FixedPoint2 TransferAmount { get; set; } = FixedPoint2.New(5);

		// Token: 0x0600237F RID: 9087 RVA: 0x000B999C File Offset: 0x000B7B9C
		public override ComponentState GetComponentState()
		{
			Solution solution;
			if (!this._entMan.EntitySysManager.GetEntitySystem<SolutionContainerSystem>().TryGetSolution(base.Owner, this.SolutionName, out solution, null))
			{
				return new HyposprayComponentState(FixedPoint2.Zero, FixedPoint2.Zero);
			}
			return new HyposprayComponentState(solution.Volume, solution.MaxVolume);
		}

		// Token: 0x040015DE RID: 5598
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x040015E1 RID: 5601
		[DataField("injectSound", false, 1, false, false, null)]
		public SoundSpecifier InjectSound = new SoundPathSpecifier("/Audio/Items/hypospray.ogg", null);
	}
}
