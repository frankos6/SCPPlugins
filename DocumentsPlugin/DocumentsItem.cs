using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using UnityEngine;
using MEC;

namespace SCPPlugins.DocumentsPlugin
{
    [CustomItem(ItemType.Coin)]
    public class DocumentsItem : CustomItem
    {
        public override uint Id { get; set; } = 1;
        public override string Name { get; set; } = "Documents";

        public override string Description { get; set; } =
            "Important research documents. Steal all 4 of them to win the round!";
        public override float Weight { get; set; } = 10.0f;
        public override Vector3 Scale { get; set; } = new Vector3(4.5f,1.7f,4.5f);
        
        //TODO: replace with a couple presets
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 4,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside012Locker
                },
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside173Armory
                },
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside914
                },
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideGr18
                },
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocationType.InsideLocker
                },
                new DynamicSpawnPoint
                {
                    Chance = 100,
                    Location = SpawnLocationType.InsideLocker
                },
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideIntercom
                },
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideLczCafe
                },
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideLczWc
                },
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideHid
                },
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideServersBottom
                },
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideHidLeft
                },
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideHidRight
                },
                new DynamicSpawnPoint
                {
                    Chance = 50,
                    Location = SpawnLocationType.InsideHczArmory
                }
            }
        };

        protected override void OnPickingUp(PickingUpItemEventArgs ev)
        {
            if (ev.Player.IsCHI)
            {
                ev.Player.ShowHint("You have destroyed the documents.");
                ev.Pickup.Destroy();
                ev.IsAllowed = false;
            }
            else if (ev.Player.Role != RoleTypeId.Scientist && ev.Player.Role != RoleTypeId.FacilityGuard)
            {
                ev.Player.ShowHint("Only Scientists or Guards can pick this up.");
                ev.IsAllowed = false;
                Timing.CallDelayed(3f, () => ev.IsAllowed = true);
            }
            else
            {
                if (!ev.Player.TryGetSessionVariable("Documents", out int count))
                {
                    ev.IsAllowed = false;
                    Timing.CallDelayed(3f, () =>
                    {
                        ev.IsAllowed = true;
                        throw new Exception($"Could not get Documents variable from {ev.Player.Nickname}");
                    });
                }
                else
                {
                    if (count >= 4)
                    {
                        ev.IsAllowed = false;
                        ev.Player.ShowHint("You already have all 4 documents.");
                        Log.Warn($"{ev.Player.Nickname} tried to pick up 5th document");
                        Timing.CallDelayed(3f, () => ev.IsAllowed = true);
                    } else {
                        count += 1;
                        ev.Player.ShowHint($"You picked up a document!\n Collect all 4 and escape to win the round.\n You currently have {count}/4 documents",4.0f);
                        ev.Player.SessionVariables["Documents"] = count;
                        ev.Pickup.Destroy();
                        ev.IsAllowed = false;
                    }
                }
            }
            base.OnPickingUp(ev);
        }
    }
}