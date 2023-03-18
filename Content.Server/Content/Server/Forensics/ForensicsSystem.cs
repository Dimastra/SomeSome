using System;
using System.Runtime.CompilerServices;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Server.Forensics
{
	// Token: 0x020004E8 RID: 1256
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ForensicsSystem : EntitySystem
	{
		// Token: 0x060019D6 RID: 6614 RVA: 0x00087880 File Offset: 0x00085A80
		public override void Initialize()
		{
			base.SubscribeLocalEvent<FingerprintComponent, ContactInteractionEvent>(new ComponentEventHandler<FingerprintComponent, ContactInteractionEvent>(this.OnInteract), null, null);
			base.SubscribeLocalEvent<FingerprintComponent, ComponentInit>(new ComponentEventHandler<FingerprintComponent, ComponentInit>(this.OnInit), null, null);
		}

		// Token: 0x060019D7 RID: 6615 RVA: 0x000878AA File Offset: 0x00085AAA
		private void OnInteract(EntityUid uid, FingerprintComponent component, ContactInteractionEvent args)
		{
			this.ApplyEvidence(uid, args.Other);
		}

		// Token: 0x060019D8 RID: 6616 RVA: 0x000878B9 File Offset: 0x00085AB9
		private void OnInit(EntityUid uid, FingerprintComponent component, ComponentInit args)
		{
			component.Fingerprint = this.GenerateFingerprint();
		}

		// Token: 0x060019D9 RID: 6617 RVA: 0x000878C8 File Offset: 0x00085AC8
		private string GenerateFingerprint()
		{
			byte[] fingerprint = new byte[16];
			this._random.NextBytes(fingerprint);
			return Convert.ToHexString(fingerprint);
		}

		// Token: 0x060019DA RID: 6618 RVA: 0x000878F0 File Offset: 0x00085AF0
		private void ApplyEvidence(EntityUid user, EntityUid target)
		{
			ForensicsComponent component = base.EnsureComp<ForensicsComponent>(target);
			EntityUid? gloves;
			if (this._inventory.TryGetSlotEntity(user, "gloves", out gloves, null, null))
			{
				FiberComponent fiber;
				if (base.TryComp<FiberComponent>(gloves, ref fiber) && !string.IsNullOrEmpty(fiber.FiberMaterial))
				{
					component.Fibers.Add(string.IsNullOrEmpty(fiber.FiberColor) ? Loc.GetString("forensic-fibers", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("material", fiber.FiberMaterial)
					}) : Loc.GetString("forensic-fibers-colored", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("color", fiber.FiberColor),
						new ValueTuple<string, object>("material", fiber.FiberMaterial)
					}));
				}
				if (base.HasComp<FingerprintMaskComponent>(gloves))
				{
					return;
				}
			}
			FingerprintComponent fingerprint;
			if (base.TryComp<FingerprintComponent>(user, ref fingerprint))
			{
				component.Fingerprints.Add(fingerprint.Fingerprint ?? "");
			}
		}

		// Token: 0x04001045 RID: 4165
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04001046 RID: 4166
		[Dependency]
		private readonly InventorySystem _inventory;
	}
}
