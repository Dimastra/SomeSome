using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Arcade.Components;
using Content.Server.Power.Components;
using Content.Server.UserInterface;
using Content.Shared.Arcade;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Server.Arcade
{
	// Token: 0x020007BC RID: 1980
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArcadeSystem : EntitySystem
	{
		// Token: 0x06002AE8 RID: 10984 RVA: 0x000E0F04 File Offset: 0x000DF104
		private void InitializeBlockGame()
		{
			ComponentEventRefHandler<BlockGameArcadeComponent, PowerChangedEvent> componentEventRefHandler;
			if ((componentEventRefHandler = ArcadeSystem.<>O.<0>__OnBlockPowerChanged) == null)
			{
				componentEventRefHandler = (ArcadeSystem.<>O.<0>__OnBlockPowerChanged = new ComponentEventRefHandler<BlockGameArcadeComponent, PowerChangedEvent>(ArcadeSystem.OnBlockPowerChanged));
			}
			base.SubscribeLocalEvent<BlockGameArcadeComponent, PowerChangedEvent>(componentEventRefHandler, null, null);
		}

		// Token: 0x06002AE9 RID: 10985 RVA: 0x000E0F29 File Offset: 0x000DF129
		private static void OnBlockPowerChanged(EntityUid uid, BlockGameArcadeComponent component, ref PowerChangedEvent args)
		{
			component.OnPowerStateChanged(args);
		}

		// Token: 0x06002AEA RID: 10986 RVA: 0x000E0F38 File Offset: 0x000DF138
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BlockGameArcadeComponent, AfterActivatableUIOpenEvent>(new ComponentEventHandler<BlockGameArcadeComponent, AfterActivatableUIOpenEvent>(this.OnAfterUIOpen), null, null);
			base.SubscribeLocalEvent<SpaceVillainArcadeComponent, AfterActivatableUIOpenEvent>(new ComponentEventHandler<SpaceVillainArcadeComponent, AfterActivatableUIOpenEvent>(this.OnAfterUIOpenSV), null, null);
			base.SubscribeLocalEvent<BlockGameArcadeComponent, BoundUIClosedEvent>(delegate(EntityUid _, BlockGameArcadeComponent c, BoundUIClosedEvent args)
			{
				c.UnRegisterPlayerSession((IPlayerSession)args.Session);
			}, null, null);
			this.InitializeBlockGame();
			this.InitializeSpaceVillain();
		}

		// Token: 0x06002AEB RID: 10987 RVA: 0x000E0FA8 File Offset: 0x000DF1A8
		private void OnAfterUIOpen(EntityUid uid, BlockGameArcadeComponent component, AfterActivatableUIOpenEvent args)
		{
			ActorComponent actor = base.Comp<ActorComponent>(args.User);
			BoundUserInterface userInterface = component.UserInterface;
			if (userInterface != null && userInterface.SessionHasOpen(actor.PlayerSession))
			{
				component.RegisterPlayerSession(actor.PlayerSession);
			}
		}

		// Token: 0x06002AEC RID: 10988 RVA: 0x000E0FE8 File Offset: 0x000DF1E8
		private void OnAfterUIOpenSV(EntityUid uid, SpaceVillainArcadeComponent component, AfterActivatableUIOpenEvent args)
		{
			if (component.Game == null)
			{
				component.Game = new SpaceVillainArcadeComponent.SpaceVillainGame(component);
			}
		}

		// Token: 0x06002AED RID: 10989 RVA: 0x000E100C File Offset: 0x000DF20C
		public ArcadeSystem.HighScorePlacement RegisterHighScore(string name, int score)
		{
			BlockGameMessages.HighScoreEntry entry = new BlockGameMessages.HighScoreEntry(name, score);
			return new ArcadeSystem.HighScorePlacement(this.TryInsertIntoList(this._roundHighscores, entry), this.TryInsertIntoList(this._globalHighscores, entry));
		}

		// Token: 0x06002AEE RID: 10990 RVA: 0x000E1040 File Offset: 0x000DF240
		public List<BlockGameMessages.HighScoreEntry> GetLocalHighscores()
		{
			return this.GetSortedHighscores(this._roundHighscores);
		}

		// Token: 0x06002AEF RID: 10991 RVA: 0x000E104E File Offset: 0x000DF24E
		public List<BlockGameMessages.HighScoreEntry> GetGlobalHighscores()
		{
			return this.GetSortedHighscores(this._globalHighscores);
		}

		// Token: 0x06002AF0 RID: 10992 RVA: 0x000E105C File Offset: 0x000DF25C
		private List<BlockGameMessages.HighScoreEntry> GetSortedHighscores(List<BlockGameMessages.HighScoreEntry> highScoreEntries)
		{
			List<BlockGameMessages.HighScoreEntry> list = Extensions.ShallowClone<BlockGameMessages.HighScoreEntry>(highScoreEntries);
			list.Sort((BlockGameMessages.HighScoreEntry p1, BlockGameMessages.HighScoreEntry p2) => p2.Score.CompareTo(p1.Score));
			return list;
		}

		// Token: 0x06002AF1 RID: 10993 RVA: 0x000E108C File Offset: 0x000DF28C
		private int? TryInsertIntoList(List<BlockGameMessages.HighScoreEntry> highScoreEntries, BlockGameMessages.HighScoreEntry entry)
		{
			if (highScoreEntries.Count < 5)
			{
				highScoreEntries.Add(entry);
				return this.GetPlacement(highScoreEntries, entry);
			}
			if (highScoreEntries.Min((BlockGameMessages.HighScoreEntry e) => e.Score) >= entry.Score)
			{
				return null;
			}
			BlockGameMessages.HighScoreEntry lowestHighscore = highScoreEntries.Min<BlockGameMessages.HighScoreEntry>();
			if (lowestHighscore == null)
			{
				return null;
			}
			highScoreEntries.Remove(lowestHighscore);
			highScoreEntries.Add(entry);
			return this.GetPlacement(highScoreEntries, entry);
		}

		// Token: 0x06002AF2 RID: 10994 RVA: 0x000E1114 File Offset: 0x000DF314
		private int? GetPlacement(List<BlockGameMessages.HighScoreEntry> highScoreEntries, BlockGameMessages.HighScoreEntry entry)
		{
			int? placement = null;
			if (highScoreEntries.Contains(entry))
			{
				highScoreEntries.Sort((BlockGameMessages.HighScoreEntry p1, BlockGameMessages.HighScoreEntry p2) => p2.Score.CompareTo(p1.Score));
				placement = new int?(1 + highScoreEntries.IndexOf(entry));
			}
			return placement;
		}

		// Token: 0x06002AF3 RID: 10995 RVA: 0x000E1168 File Offset: 0x000DF368
		public override void Update(float frameTime)
		{
			foreach (BlockGameArcadeComponent blockGameArcadeComponent in this.EntityManager.EntityQuery<BlockGameArcadeComponent>(false))
			{
				blockGameArcadeComponent.DoGameTick(frameTime);
			}
		}

		// Token: 0x06002AF4 RID: 10996 RVA: 0x000E11BC File Offset: 0x000DF3BC
		private void InitializeSpaceVillain()
		{
			base.SubscribeLocalEvent<SpaceVillainArcadeComponent, PowerChangedEvent>(new ComponentEventRefHandler<SpaceVillainArcadeComponent, PowerChangedEvent>(this.OnSVillainPower), null, null);
		}

		// Token: 0x06002AF5 RID: 10997 RVA: 0x000E11D2 File Offset: 0x000DF3D2
		private void OnSVillainPower(EntityUid uid, SpaceVillainArcadeComponent component, ref PowerChangedEvent args)
		{
			component.OnPowerStateChanged(args);
		}

		// Token: 0x04001A8B RID: 6795
		private readonly List<BlockGameMessages.HighScoreEntry> _roundHighscores = new List<BlockGameMessages.HighScoreEntry>();

		// Token: 0x04001A8C RID: 6796
		private readonly List<BlockGameMessages.HighScoreEntry> _globalHighscores = new List<BlockGameMessages.HighScoreEntry>();

		// Token: 0x02000B32 RID: 2866
		[NullableContext(0)]
		public readonly struct HighScorePlacement
		{
			// Token: 0x06003884 RID: 14468 RVA: 0x00125E7B File Offset: 0x0012407B
			public HighScorePlacement(int? globalPlacement, int? localPlacement)
			{
				this.GlobalPlacement = globalPlacement;
				this.LocalPlacement = localPlacement;
			}

			// Token: 0x04002960 RID: 10592
			public readonly int? GlobalPlacement;

			// Token: 0x04002961 RID: 10593
			public readonly int? LocalPlacement;
		}

		// Token: 0x02000B33 RID: 2867
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002962 RID: 10594
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<BlockGameArcadeComponent, PowerChangedEvent> <0>__OnBlockPowerChanged;
		}
	}
}
