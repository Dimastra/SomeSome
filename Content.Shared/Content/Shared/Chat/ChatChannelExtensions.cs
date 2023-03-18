using System;
using Robust.Shared.Maths;

namespace Content.Shared.Chat
{
	// Token: 0x020005FF RID: 1535
	public static class ChatChannelExtensions
	{
		// Token: 0x060012D6 RID: 4822 RVA: 0x0003DBB0 File Offset: 0x0003BDB0
		public static Color TextColor(this ChatChannel channel)
		{
			if (channel <= ChatChannel.LOOC)
			{
				if (channel <= ChatChannel.Server)
				{
					if (channel == ChatChannel.Whisper)
					{
						return Color.DarkGray;
					}
					if (channel == ChatChannel.Server)
					{
						return Color.Orange;
					}
				}
				else
				{
					if (channel == ChatChannel.Radio)
					{
						return Color.LimeGreen;
					}
					if (channel == ChatChannel.LOOC)
					{
						return Color.MediumTurquoise;
					}
				}
			}
			else if (channel <= ChatChannel.Dead)
			{
				if (channel == ChatChannel.OOC)
				{
					return Color.LightSkyBlue;
				}
				if (channel == ChatChannel.Dead)
				{
					return Color.MediumPurple;
				}
			}
			else
			{
				if (channel == ChatChannel.Admin)
				{
					return Color.Red;
				}
				if (channel == ChatChannel.AdminChat)
				{
					return Color.HotPink;
				}
			}
			return Color.LightGray;
		}
	}
}
