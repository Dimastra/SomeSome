using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Systems;
using Content.Shared.ActionBlocker;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC
{
	// Token: 0x0200032F RID: 815
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class NPCBlackboard : IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		// Token: 0x060010D1 RID: 4305 RVA: 0x00056859 File Offset: 0x00054A59
		public void Clear()
		{
			this._blackboard.Clear();
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x00056868 File Offset: 0x00054A68
		public NPCBlackboard ShallowClone()
		{
			NPCBlackboard dict = new NPCBlackboard();
			foreach (KeyValuePair<string, object> item in this._blackboard)
			{
				dict.SetValue(item.Key, item.Value);
			}
			return dict;
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x000568D0 File Offset: 0x00054AD0
		public bool ContainsKey(string key)
		{
			return this._blackboard.ContainsKey(key);
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x000568DE File Offset: 0x00054ADE
		public T GetValue<[Nullable(2)] T>(string key)
		{
			return (T)((object)this._blackboard[key]);
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x000568F4 File Offset: 0x00054AF4
		[return: Nullable(2)]
		public T GetValueOrDefault<[Nullable(2)] T>(string key, IEntityManager entManager)
		{
			object value;
			if (this._blackboard.TryGetValue(key, out value))
			{
				return (T)((object)value);
			}
			if (this.TryGetEntityDefault(key, out value, entManager))
			{
				return (T)((object)value);
			}
			if (NPCBlackboard.BlackboardDefaults.TryGetValue(key, out value))
			{
				return (T)((object)value);
			}
			return default(T);
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x0005694C File Offset: 0x00054B4C
		public bool TryGetValue<[Nullable(2)] T>(string key, [Nullable(2)] [NotNullWhen(true)] out T value, IEntityManager entManager)
		{
			object data;
			if (this._blackboard.TryGetValue(key, out data))
			{
				value = (T)((object)data);
				return true;
			}
			if (this.TryGetEntityDefault(key, out data, entManager))
			{
				value = (T)((object)data);
				return true;
			}
			if (NPCBlackboard.BlackboardDefaults.TryGetValue(key, out data))
			{
				value = (T)((object)data);
				return true;
			}
			value = default(T);
			return false;
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x000569B6 File Offset: 0x00054BB6
		public void SetValue(string key, object value)
		{
			if (this.ReadOnly)
			{
				this.AssertReadonly();
				return;
			}
			this._blackboard[key] = value;
		}

		// Token: 0x060010D8 RID: 4312 RVA: 0x000569D4 File Offset: 0x00054BD4
		private void AssertReadonly()
		{
		}

		// Token: 0x060010D9 RID: 4313 RVA: 0x000569D8 File Offset: 0x00054BD8
		private bool TryGetEntityDefault(string key, [Nullable(2)] [NotNullWhen(true)] out object value, IEntityManager entManager)
		{
			value = null;
			if (!(key == "Access"))
			{
				if (!(key == "CanMove"))
				{
					if (!(key == "OwnerCoordinates"))
					{
						return false;
					}
					EntityUid owner;
					if (!this.TryGetValue<EntityUid>("Owner", out owner, entManager))
					{
						return false;
					}
					TransformComponent xform;
					if (entManager.TryGetComponent<TransformComponent>(owner, ref xform))
					{
						value = xform.Coordinates;
						return true;
					}
					return false;
				}
				else
				{
					EntityUid owner;
					if (!this.TryGetValue<EntityUid>("Owner", out owner, entManager))
					{
						return false;
					}
					ActionBlockerSystem blocker = entManager.EntitySysManager.GetEntitySystem<ActionBlockerSystem>();
					value = blocker.CanMove(owner, null);
					return true;
				}
			}
			else
			{
				EntityUid owner;
				if (!this.TryGetValue<EntityUid>("Owner", out owner, entManager))
				{
					return false;
				}
				AccessReaderSystem access = entManager.EntitySysManager.GetEntitySystem<AccessReaderSystem>();
				value = access.FindAccessTags(owner);
				return true;
			}
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x00056A9B File Offset: 0x00054C9B
		public bool Remove<[Nullable(2)] T>(string key)
		{
			return this._blackboard.Remove(key);
		}

		// Token: 0x060010DB RID: 4315 RVA: 0x00056AA9 File Offset: 0x00054CA9
		[return: Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return this._blackboard.GetEnumerator();
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x00056ABB File Offset: 0x00054CBB
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x040009F8 RID: 2552
		private static readonly Dictionary<string, object> BlackboardDefaults = new Dictionary<string, object>
		{
			{
				"BufferRange",
				10f
			},
			{
				"FollowCloseRange",
				3f
			},
			{
				"FollowRange",
				7f
			},
			{
				"IdleRange",
				7f
			},
			{
				"MaximumIdleTime",
				7f
			},
			{
				"MedibotInjectRange",
				4f
			},
			{
				"MeleeMissChance",
				0.3f
			},
			{
				"MeleeRange",
				1f
			},
			{
				"MinimumIdleTime",
				2f
			},
			{
				"MovementRange",
				1.5f
			},
			{
				"RangedRange",
				7f
			},
			{
				"RotateSpeed",
				3.1415927f
			},
			{
				"VisionRadius",
				7f
			}
		};

		// Token: 0x040009F9 RID: 2553
		private readonly Dictionary<string, object> _blackboard = new Dictionary<string, object>();

		// Token: 0x040009FA RID: 2554
		public bool ReadOnly;

		// Token: 0x040009FB RID: 2555
		public const string Access = "Access";

		// Token: 0x040009FC RID: 2556
		public const string CanMove = "CanMove";

		// Token: 0x040009FD RID: 2557
		public const string FollowTarget = "FollowTarget";

		// Token: 0x040009FE RID: 2558
		public const string MedibotInjectRange = "MedibotInjectRange";

		// Token: 0x040009FF RID: 2559
		public const string MeleeMissChance = "MeleeMissChance";

		// Token: 0x04000A00 RID: 2560
		public const string Owner = "Owner";

		// Token: 0x04000A01 RID: 2561
		public const string OwnerCoordinates = "OwnerCoordinates";

		// Token: 0x04000A02 RID: 2562
		public const string MovementTarget = "MovementTarget";

		// Token: 0x04000A03 RID: 2563
		public const string NavInteract = "NavInteract";

		// Token: 0x04000A04 RID: 2564
		public const string NavPry = "NavPry";

		// Token: 0x04000A05 RID: 2565
		public const string NavSmash = "NavSmash";

		// Token: 0x04000A06 RID: 2566
		public const string PathfindKey = "MovementPathfind";

		// Token: 0x04000A07 RID: 2567
		public const string RotateSpeed = "RotateSpeed";

		// Token: 0x04000A08 RID: 2568
		public const string VisionRadius = "VisionRadius";
	}
}
