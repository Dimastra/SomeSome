using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind.Commands;
using Content.Server.Mind.Components;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Ghost.Roles.Components
{
	// Token: 0x0200049D RID: 1181
	[RegisterComponent]
	[ComponentReference(typeof(GhostRoleComponent))]
	public sealed class GhostTakeoverAvailableComponent : GhostRoleComponent
	{
		// Token: 0x060017C9 RID: 6089 RVA: 0x0007C298 File Offset: 0x0007A498
		[NullableContext(1)]
		public override bool Take(IPlayerSession session)
		{
			if (base.Taken)
			{
				return false;
			}
			base.Taken = true;
			if (ComponentExt.EnsureComponent<MindComponent>(base.Owner).HasMind)
			{
				return false;
			}
			if (this.MakeSentient)
			{
				MakeSentientCommand.MakeSentient(base.Owner, IoCManager.Resolve<IEntityManager>(), base.AllowMovement, base.AllowSpeech);
			}
			GhostRoleSystem ghostRoleSystem = EntitySystem.Get<GhostRoleSystem>();
			ghostRoleSystem.GhostRoleInternalCreateMindAndTransfer(session, base.Owner, base.Owner, this);
			ghostRoleSystem.UnregisterGhostRole(this);
			return true;
		}
	}
}
