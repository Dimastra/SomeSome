using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x02000694 RID: 1684
	public sealed class ChemicalReactionSystem : SharedChemicalReactionSystem
	{
		// Token: 0x060022DD RID: 8925 RVA: 0x000B550C File Offset: 0x000B370C
		[NullableContext(1)]
		protected override void OnReaction(Solution solution, ReactionPrototype reaction, ReagentPrototype randomReagent, EntityUid owner, FixedPoint2 unitReactions)
		{
			base.OnReaction(solution, reaction, randomReagent, owner, unitReactions);
			EntityCoordinates coordinates = base.Transform(owner).Coordinates;
			ISharedAdminLogManager adminLogger = this.AdminLogger;
			LogType type = LogType.ChemicalReaction;
			LogImpact impact = reaction.Impact;
			LogStringHandler logStringHandler = new LogStringHandler(57, 4);
			logStringHandler.AppendLiteral("Chemical reaction ");
			logStringHandler.AppendFormatted(reaction.ID, 0, "reaction");
			logStringHandler.AppendLiteral(" occurred with strength ");
			logStringHandler.AppendFormatted<FixedPoint2>(unitReactions, "strength", "unitReactions");
			logStringHandler.AppendLiteral(" on entity ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(owner), "metabolizer", "ToPrettyString(owner)");
			logStringHandler.AppendLiteral(" at ");
			logStringHandler.AppendFormatted<EntityCoordinates>(coordinates, "coordinates");
			adminLogger.Add(type, impact, ref logStringHandler);
			SoundSystem.Play(reaction.Sound.GetSound(null, null), Filter.Pvs(owner, 2f, this.EntityManager, null, null), owner, null);
		}
	}
}
