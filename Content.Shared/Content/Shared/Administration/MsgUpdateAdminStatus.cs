using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
	// Token: 0x0200073A RID: 1850
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MsgUpdateAdminStatus : NetMessage
	{
		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x06001661 RID: 5729 RVA: 0x000492AA File Offset: 0x000474AA
		public override MsgGroups MsgGroup
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x06001662 RID: 5730 RVA: 0x000492B0 File Offset: 0x000474B0
		public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
		{
			int count = buffer.ReadVariableInt32();
			this.AvailableCommands = new string[count];
			for (int i = 0; i < count; i++)
			{
				this.AvailableCommands[i] = buffer.ReadString();
			}
			if (buffer.ReadBoolean())
			{
				bool active = buffer.ReadBoolean();
				buffer.ReadPadBits();
				AdminFlags flags = (AdminFlags)buffer.ReadUInt32();
				string title = buffer.ReadString();
				this.Admin = new AdminData
				{
					Active = active,
					Title = title,
					Flags = flags
				};
			}
		}

		// Token: 0x06001663 RID: 5731 RVA: 0x00049330 File Offset: 0x00047530
		public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
		{
			buffer.WriteVariableInt32(this.AvailableCommands.Length);
			foreach (string cmd in this.AvailableCommands)
			{
				buffer.Write(cmd);
			}
			buffer.Write(this.Admin != null);
			if (this.Admin == null)
			{
				return;
			}
			buffer.Write(this.Admin.Active);
			buffer.WritePadBits();
			buffer.Write((uint)this.Admin.Flags);
			buffer.Write(this.Admin.Title);
		}

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x06001664 RID: 5732 RVA: 0x000493BD File Offset: 0x000475BD
		public override NetDeliveryMethod DeliveryMethod
		{
			get
			{
				return 67;
			}
		}

		// Token: 0x040016AF RID: 5807
		[Nullable(2)]
		public AdminData Admin;

		// Token: 0x040016B0 RID: 5808
		public string[] AvailableCommands = Array.Empty<string>();
	}
}
