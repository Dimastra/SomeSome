using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared.Access.Components
{
	// Token: 0x0200077E RID: 1918
	[NullableContext(1)]
	[Nullable(0)]
	[NetworkedComponent]
	public abstract class SharedIdCardConsoleComponent : Component
	{
		// Token: 0x04001760 RID: 5984
		public const int MaxFullNameLength = 30;

		// Token: 0x04001761 RID: 5985
		public const int MaxJobTitleLength = 30;

		// Token: 0x04001762 RID: 5986
		public static string PrivilegedIdCardSlotId = "IdCardConsole-privilegedId";

		// Token: 0x04001763 RID: 5987
		public static string TargetIdCardSlotId = "IdCardConsole-targetId";

		// Token: 0x04001764 RID: 5988
		[DataField("privilegedIdSlot", false, 1, false, false, null)]
		public ItemSlot PrivilegedIdSlot = new ItemSlot();

		// Token: 0x04001765 RID: 5989
		[DataField("targetIdSlot", false, 1, false, false, null)]
		public ItemSlot TargetIdSlot = new ItemSlot();

		// Token: 0x04001766 RID: 5990
		[DataField("accessLevels", false, 1, false, false, typeof(PrototypeIdListSerializer<AccessLevelPrototype>))]
		public List<string> AccessLevels = new List<string>
		{
			"Armory",
			"Atmospherics",
			"Bar",
			"Brig",
			"Captain",
			"Cargo",
			"Chapel",
			"Chemistry",
			"ChiefEngineer",
			"ChiefMedicalOfficer",
			"Command",
			"Engineering",
			"External",
			"HeadOfPersonnel",
			"HeadOfSecurity",
			"Hydroponics",
			"Janitor",
			"Kitchen",
			"Maintenance",
			"Medical",
			"Quartermaster",
			"Research",
			"ResearchDirector",
			"Salvage",
			"Security",
			"Service",
			"Theatre"
		};

		// Token: 0x020008AD RID: 2221
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class WriteToTargetIdMessage : BoundUserInterfaceMessage
		{
			// Token: 0x06001A49 RID: 6729 RVA: 0x00051FDC File Offset: 0x000501DC
			public WriteToTargetIdMessage(string fullName, string jobTitle, List<string> accessList, string jobPrototype)
			{
				this.FullName = fullName;
				this.JobTitle = jobTitle;
				this.AccessList = accessList;
				this.JobPrototype = jobPrototype;
			}

			// Token: 0x04001AB0 RID: 6832
			public readonly string FullName;

			// Token: 0x04001AB1 RID: 6833
			public readonly string JobTitle;

			// Token: 0x04001AB2 RID: 6834
			public readonly List<string> AccessList;

			// Token: 0x04001AB3 RID: 6835
			public readonly string JobPrototype;
		}

		// Token: 0x020008AE RID: 2222
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class IdCardConsoleBoundUserInterfaceState : BoundUserInterfaceState
		{
			// Token: 0x06001A4A RID: 6730 RVA: 0x00052004 File Offset: 0x00050204
			public IdCardConsoleBoundUserInterfaceState(bool isPrivilegedIdPresent, bool isPrivilegedIdAuthorized, bool isTargetIdPresent, [Nullable(2)] string targetIdFullName, [Nullable(2)] string targetIdJobTitle, [Nullable(new byte[]
			{
				2,
				1
			})] string[] targetIdAccessList, string targetIdJobPrototype, string privilegedIdName, string targetIdName)
			{
				this.IsPrivilegedIdPresent = isPrivilegedIdPresent;
				this.IsPrivilegedIdAuthorized = isPrivilegedIdAuthorized;
				this.IsTargetIdPresent = isTargetIdPresent;
				this.TargetIdFullName = targetIdFullName;
				this.TargetIdJobTitle = targetIdJobTitle;
				this.TargetIdAccessList = targetIdAccessList;
				this.TargetIdJobPrototype = targetIdJobPrototype;
				this.PrivilegedIdName = privilegedIdName;
				this.TargetIdName = targetIdName;
			}

			// Token: 0x04001AB4 RID: 6836
			public readonly string PrivilegedIdName;

			// Token: 0x04001AB5 RID: 6837
			public readonly bool IsPrivilegedIdPresent;

			// Token: 0x04001AB6 RID: 6838
			public readonly bool IsPrivilegedIdAuthorized;

			// Token: 0x04001AB7 RID: 6839
			public readonly bool IsTargetIdPresent;

			// Token: 0x04001AB8 RID: 6840
			public readonly string TargetIdName;

			// Token: 0x04001AB9 RID: 6841
			[Nullable(2)]
			public readonly string TargetIdFullName;

			// Token: 0x04001ABA RID: 6842
			[Nullable(2)]
			public readonly string TargetIdJobTitle;

			// Token: 0x04001ABB RID: 6843
			[Nullable(new byte[]
			{
				2,
				1
			})]
			public readonly string[] TargetIdAccessList;

			// Token: 0x04001ABC RID: 6844
			public readonly string TargetIdJobPrototype;
		}

		// Token: 0x020008AF RID: 2223
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public enum IdCardConsoleUiKey : byte
		{
			// Token: 0x04001ABE RID: 6846
			Key
		}
	}
}
