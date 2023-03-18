using System;
using System.Collections.Generic;
using Robust.Server.Player;

namespace Content.Server.Players.PlayTimeTracking
{
	// Token: 0x020002D3 RID: 723
	// (Invoke) Token: 0x06000E8C RID: 3724
	public delegate void CalcPlayTimeTrackersCallback(IPlayerSession player, HashSet<string> trackers);
}
