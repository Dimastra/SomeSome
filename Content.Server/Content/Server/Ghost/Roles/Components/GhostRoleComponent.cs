using System;
using System.Runtime.CompilerServices;
using Robust.Server.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Ghost.Roles.Components
{
	// Token: 0x0200049B RID: 1179
	[NullableContext(1)]
	[Nullable(0)]
	[Access(new Type[]
	{
		typeof(GhostRoleSystem)
	})]
	public abstract class GhostRoleComponent : Component
	{
		// Token: 0x1700033A RID: 826
		// (get) Token: 0x060017B3 RID: 6067 RVA: 0x0007C06A File Offset: 0x0007A26A
		// (set) Token: 0x060017B4 RID: 6068 RVA: 0x0007C077 File Offset: 0x0007A277
		[ViewVariables]
		[Access]
		public string RoleName
		{
			get
			{
				return Loc.GetString(this._roleName);
			}
			set
			{
				this._roleName = value;
				EntitySystem.Get<GhostRoleSystem>().UpdateAllEui();
			}
		}

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x060017B5 RID: 6069 RVA: 0x0007C08A File Offset: 0x0007A28A
		// (set) Token: 0x060017B6 RID: 6070 RVA: 0x0007C097 File Offset: 0x0007A297
		[ViewVariables]
		[Access]
		public string RoleDescription
		{
			get
			{
				return Loc.GetString(this._roleDescription);
			}
			set
			{
				this._roleDescription = value;
				EntitySystem.Get<GhostRoleSystem>().UpdateAllEui();
			}
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x060017B7 RID: 6071 RVA: 0x0007C0AA File Offset: 0x0007A2AA
		// (set) Token: 0x060017B8 RID: 6072 RVA: 0x0007C0B2 File Offset: 0x0007A2B2
		[ViewVariables]
		[Access]
		public string RoleRules
		{
			get
			{
				return this._roleRules;
			}
			set
			{
				this._roleRules = value;
				EntitySystem.Get<GhostRoleSystem>().UpdateAllEui();
			}
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x060017B9 RID: 6073 RVA: 0x0007C0C5 File Offset: 0x0007A2C5
		// (set) Token: 0x060017BA RID: 6074 RVA: 0x0007C0CD File Offset: 0x0007A2CD
		[DataField("allowSpeech", false, 1, false, false, null)]
		[ViewVariables]
		public bool AllowSpeech { get; set; } = true;

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x060017BB RID: 6075 RVA: 0x0007C0D6 File Offset: 0x0007A2D6
		// (set) Token: 0x060017BC RID: 6076 RVA: 0x0007C0DE File Offset: 0x0007A2DE
		[DataField("allowMovement", false, 1, false, false, null)]
		[ViewVariables]
		public bool AllowMovement { get; set; }

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x060017BD RID: 6077 RVA: 0x0007C0E7 File Offset: 0x0007A2E7
		// (set) Token: 0x060017BE RID: 6078 RVA: 0x0007C0EF File Offset: 0x0007A2EF
		[ViewVariables]
		public bool Taken { get; set; }

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x060017BF RID: 6079 RVA: 0x0007C0F8 File Offset: 0x0007A2F8
		// (set) Token: 0x060017C0 RID: 6080 RVA: 0x0007C100 File Offset: 0x0007A300
		[ViewVariables]
		public uint Identifier { get; set; }

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x060017C1 RID: 6081 RVA: 0x0007C109 File Offset: 0x0007A309
		// (set) Token: 0x060017C2 RID: 6082 RVA: 0x0007C111 File Offset: 0x0007A311
		[ViewVariables]
		[DataField("reregister", false, 1, false, false, null)]
		public bool ReregisterOnGhost { get; set; } = true;

		// Token: 0x060017C3 RID: 6083
		public abstract bool Take(IPlayerSession session);

		// Token: 0x04000EAF RID: 3759
		[DataField("name", false, 1, false, false, null)]
		public string _roleName = "Unknown";

		// Token: 0x04000EB0 RID: 3760
		[DataField("description", false, 1, false, false, null)]
		private string _roleDescription = "Unknown";

		// Token: 0x04000EB1 RID: 3761
		[DataField("rules", false, 1, false, false, null)]
		private string _roleRules = "";

		// Token: 0x04000EB2 RID: 3762
		[ViewVariables]
		[DataField("makeSentient", false, 1, false, false, null)]
		protected bool MakeSentient = true;

		// Token: 0x04000EB3 RID: 3763
		[DataField("prob", false, 1, false, false, null)]
		public float Probability = 1f;
	}
}
