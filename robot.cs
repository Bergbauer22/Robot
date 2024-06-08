using MelonLoader;
using BTD_Mod_Helper;
using robot;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.Display;
using UnityEngine;
using Il2CppSystem.IO;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Utils;
using System.Collections.Generic;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Unity.Towers.Projectiles.Behaviors;
using Il2CppSystem.Runtime.Remoting.Lifetime;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using System.Linq;
using static Il2CppAssets.Scripts.Models.Rounds.FreeplayBloonGroupModel;
using BTD_Mod_Helper.Api;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using BTD_Mod_Helper.Api.ModOptions;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Unity.Audio;
using HarmonyLib;
using Il2CppAssets.Scripts.Data.Gameplay.Mods;




[assembly: MelonInfo(typeof(robot.robot), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace robot;

public class robot : BloonsTD6Mod
{
    public int LastPortalSound;
    public float LastSoundAgo;
    public float LastHubSoundAgo;
    [HarmonyPatch(typeof(AudioFactory), "Start")]
    public class AudioFactoryStart_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(AudioFactory __instance)
        {
            foreach (UnityEngine.Object asset in ModContent.GetBundle<robot>("robot").LoadAllAssetsAsync<AudioClip>().allAssets)
            {
                __instance.RegisterAudioClip(asset.name, asset.Cast<AudioClip>());
            }
        }
    }
    public static void PlaySound(string name)
    {
        Game.instance.audioFactory.PlaySoundFromUnity(null, name, "Fx", -1, 1f, 0, false, "", false, false, true, Il2CppAssets.Scripts.Unity.Bridge.AudioType.FX);
    }
    public override void OnApplicationStart()
    {
        ModHelper.Msg<robot>("You got terminated!");
    }
    static int rnd(int min, int max)
    {
        System.Random random = new System.Random();
        return random.Next(min, max);
    }
    public static readonly ModSettingHotkey RobotMonkeyHotkey = new(KeyCode.R, HotkeyModifier.Shift)
    {
        icon = ModContent.GetTextureGUID<robot>("Icon"),
    };
    private static float MovmentSpeed = 1.2f;
    public Il2CppAssets.Scripts.Simulation.SMath.Vector2 CalculateNewPosition(float oldPosX, float oldPosY, float oldPosZ, float directionDegrees)
    {
        // Umwandlung des Winkels von Grad in Radiant
        float directionRadians = directionDegrees * Mathf.Deg2Rad;

        // Berechnung der neuen X und Z Positionen
        float newX = oldPosX - Mathf.Cos(directionRadians) * MovmentSpeed; // 1.0f ist die Entfernung in Metern
        float newZ = oldPosZ + Mathf.Sin(directionRadians) * MovmentSpeed;

        return new Il2CppAssets.Scripts.Simulation.SMath.Vector2(newX, newZ);
    }
    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse)
    {
        if (tower != null && tower.towerModel.name.Contains("Robot"))
        {
            PlayRandomVoiceline();
        }
    }
    public override void OnTowerSelected(Tower tower)
    {
        if (tower != null && tower.towerModel.name.Contains("Robot"))
        {
            PlayRandomVoiceline();
        }
    }
    public void PlayRandomVoiceline()
    {
        if(LastSoundAgo < 0)
        {
            LastSoundAgo = 10;
            int random1 = rnd(1, 8);
            while (LastPortalSound == random1)
            {
                random1 = rnd(1, 8);
            }
            LastPortalSound = random1;
            switch (random1)
            {
                case 1:
                    PlaySound("1");
                    break;
                case 2:
                    PlaySound("2");
                    break;
                case 3:
                    PlaySound("3");
                    break;
                case 4:
                    PlaySound("4");
                    break;
                case 5:
                    PlaySound("5");
                    break;
                case 6:
                    PlaySound("6");
                    break;
                case 7:
                    PlaySound("7");
                    break;
            }
        }
    }
    public override void OnUpdate()
    {
        if (InGameData.CurrentGame != null && InGame.Bridge != null)
        {
            List<Tower> towers = InGame.instance.GetTowers();
            int TC = towers.Count;
            LastSoundAgo -= 0.025f;
            LastHubSoundAgo -= 0.025f;
            for (int i = 0; i < TC; i++)
            {
                if (towers[i] != null && towers[i].towerModel.name.Contains("Robot"))
                {
                    int TabBooster = 1;
                    if (Input.GetKey(KeyCode.Tab) && towers[i].towerModel.tiers[0] >= 2)
                    {
                        TabBooster = 2;
                    }
                    if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.LeftShift)) && LastHubSoundAgo < 0)
                    {
                        PlaySound("8");
                        LastHubSoundAgo = 6;
                    }
                    if (rnd(1, 9999) == 22)
                    {
                        int random1 = rnd(1, 8);
                        switch (random1)
                        {
                            case 1:
                                towers[i].ShowTowerEffect("When I grow up, I want to be like Arnold Schwarzenegger, a real Terminator", 100000f);
                                break;
                            case 2:
                                towers[i].ShowTowerEffect("Luckily, no one knows that pressing LeftShift makes hub noises", 100000f);
                                break;
                            case 3:
                                towers[i].ShowTowerEffect("I hope the German Bloons scene gets revived someday... I really miss Bador's Bloons times", 100000f);
                                break;
                            case 4:
                                towers[i].ShowTowerEffect("Legends say that blue Barney destroyed Bergbauer's hard drive...", 100000f);
                                break;
                            case 5:
                                towers[i].ShowTowerEffect("Another fever dream mod... **** you, Bergbauer.", 100000f);
                                break;
                            case 6:
                                towers[i].ShowTowerEffect("I wish I were as lucky as Dirk and had a cool voice, not such a load of crap", 100000f);
                                break;
                            case 7:
                                towers[i].ShowTowerEffect("You definitely have to check out the Hypixel Skyblock Minecraft server. You can waste time there for free (just kidding)", 100000f);
                                break;
                        }
                    }
                    for (int i2 = 0; i2 < TimeManager.networkScale * TabBooster; i2++)
                    {

                        Il2CppAssets.Scripts.Simulation.SMath.Vector2 newPos = CalculateNewPosition(towers[i].Position.X, towers[i].Position.Z, towers[i].Position.Y, towers[i].Rotation + 90);
                        bool allowedToMove = true;
                        // Check if tower is within the allowable Y-axis upper bound and move upwards
                        if (!(newPos.y >= -109))
                        {
                            towers[i].RotateTower(rnd(-90, 90), false, false);
                            allowedToMove = false;
                        }

                        // Check if tower is within the allowable Y-axis lower bound and move downwards
                        if (!(newPos.y <= 114))
                        {
                            towers[i].RotateTower(rnd(-90, 90), false, false);
                            allowedToMove = false;
                        }

                        // Check if tower is within the allowable X-axis left bound and move left
                        if (!(newPos.x >= -145))
                        {
                            towers[i].RotateTower(rnd(-90, 90), false, false);
                            allowedToMove = false;
                        }

                        // Check if tower is within the allowable X-axis right bound and move right
                        if (!(newPos.x <= 145))
                        {
                            towers[i].RotateTower(rnd(-90, 90), false, false);
                            allowedToMove = false;
                        }
                        List<Tower> towers2 = InGame.instance.GetTowers();
                        int TC2 = towers2.Count;
                        for (int iT = 0; iT < TC2; iT++)
                            {
                                if (towers2[iT] != null && TC2 >= 2)
                                {
                                    if (towers2[iT].Position.X != towers[i].Position.X)
                                    {
                                        float maxDistance = 3 + towers2[iT].towerModel.radius;
                                        UnityEngine.Vector2 thisPosition = new UnityEngine.Vector2(newPos.x, newPos.y);
                                        UnityEngine.Vector2 otherPosition = new UnityEngine.Vector2(towers2[iT].Position.X, towers2[iT].Position.Y);

                                        // Berechne die Distanz zwischen den Positionen
                                        float distance = UnityEngine.Vector2.Distance(thisPosition, otherPosition);

                                        // Wenn die Distanz kleiner oder gleich der maximalen Entfernung ist
                                        if (distance <= maxDistance)
                                        {
                                            //ModHelper.Msg<Monkeywalkaround>("distance: " + distance + " x1: " + newPos.x + " x2: " + towers2[iT].Position.X);
                                            towers[i].RotateTower(rnd(-90, 90), false, false);
                                            allowedToMove = false;
                                        }
                                    }
                                }
                            }
                        if (allowedToMove)
                        {
                            towers[i].PositionTower(newPos);
                        }
                    }
                }
            }
        }
    } 
    public static Shader? GetOutlineShader()
    {
        var superMonkey = GetVanillaAsset("Assets/Monkeys/DartMonkey/Graphics/SuperMonkey.prefab")?.Cast<GameObject>();
        if (superMonkey == null)
        {
            return null;
        }
        superMonkey.AddComponent<UnityDisplayNode>();
        var litOutlineShader = superMonkey.GetComponent<UnityDisplayNode>().GetMeshRenderer().material.shader;
        return litOutlineShader;
    }
    public static UnityEngine.Object? GetVanillaAsset(string name)
    {
        foreach (var assetBundle in AssetBundle.GetAllLoadedAssetBundles().ToArray())
        {
            if (assetBundle.Contains(name))
            {
                return assetBundle.LoadAsset(name);
            }
        }
        return null;
    }
}
public class RobotDP : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "Robot_000";
    public override float Scale => 1;
    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        foreach (var meshRenderer in node.GetMeshRenderers())
        {
            meshRenderer.ApplyOutlineShader();

            meshRenderer.SetOutlineColor(new Color(108 / 255f, 0 / 255f, 0 / 255f));
        }
    }
}
public class Robot300DP : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "Robot_300";
    public override float Scale => 1;

}
public class Robot030DP : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "Robot_030";
    public override float Scale => 1;

}
public class Robot003DP : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "Robot_003";
    public override float Scale => 1;
}
public class Robot400DP : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "Robot_400";
    public override float Scale => 1;


}
public class Robot040DP : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "Robot_040";
    public override float Scale => 1;

}
public class Robot004DP : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "Robot_004";
    public override float Scale => 1;

}
public class Robot500DP : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "Robot_500";
    public override float Scale => 1;


}
public class Robot050DP : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "Robot_050";
    public override float Scale => 1;

}
public class Robot005DP : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "Robot_005";
    public override float Scale => 1;

}
public class RobotParagonGUID : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "Robot_Paragon";
    public override float Scale => 1;

}
public class Robot_Paragon_DP : ModTowerCustomDisplay<RobotTower>
{
    public override float Scale => 1.0f + ParagonDisplayIndex * .025f;  // Higher degree Paragon displays will be bigger

    public override string AssetBundleName => "robot";
    public override string PrefabName => "Robot_Paragon";
    public override bool UseForTower(int[] tiers)
    {
        return IsParagon(tiers);
    }
    public Robot_Paragon_DP()
    {

    }
    public Robot_Paragon_DP(int i)
    {
        ParagonDisplayIndex = i;
    }

    public override int ParagonDisplayIndex { get; }  // Overriding in this way lets us set it in the constructor

    public override IEnumerable<ModContent> Load()
    {
        for (var i = 0; i < TotalParagonDisplays; i++)
        {
            yield return new Robot_Paragon_DP(i);
        }
    }


    public override string Name => nameof(Robot_Paragon_DP) + ParagonDisplayIndex;  // make sure each instance has its own name

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
        SetMeshTexture(node, nameof(Robot_Paragon_DP));
        
    }
}
public class Fireball : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "Fireball";
    public override float Scale => 1;

}
public class Fireball300 : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "Fireball300";
    public override float Scale => 1;

}
public class FireballGreen555 : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "FireballGreen555";
    public override float Scale => 1;

}

public class Fireball003 : ModCustomDisplay
{
    public override string AssetBundleName => "robot";
    public override string PrefabName => "Fireball003";
    public override float Scale => 1;

}
public class Robo_Buff : ModBuffIcon
{
    public override string Icon => "Icon";
    public override int MaxStackSize => 0;
}
public class Robo_Paragon_Buff : ModBuffIcon
{
    public override string Icon => "Paragon_Buff";
    public override int MaxStackSize => 0;
}
public class RobotTower : ModTower
{
    public override string DisplayName => "Robot";
    public override string Name => "Robot";
    public override string BaseTower => "DartMonkey-032";
    public override TowerSet TowerSet => TowerSet.Magic;
    public override int Cost => 2000;
    public override int TopPathUpgrades => 5;
    public override int MiddlePathUpgrades => 5;
    public override int BottomPathUpgrades => 5;
    
    public override string Portrait => "Icon";
    public override string Icon => "Icon";
    public override ParagonMode ParagonMode => ParagonMode.Base000;
    public override ModSettingHotkey Hotkey => robot.RobotMonkeyHotkey;

    public override string Description => "I'm going to terminate every Monk...i mean bloon";
    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        





        towerModel.ApplyDisplay<RobotDP>();
        towerModel.GetAttackModel().weapons[0].projectile.ApplyDisplay<Fireball>();
        towerModel.range *= 1.3f;
        towerModel.GetAttackModel().range = towerModel.range;
        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan = 2;
        towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.Purple;
        towerModel.GetAttackModel().weapons[0].projectile.pierce = 5;
        towerModel.GetAttackModel().weapons[0].ejectY = 0;
        towerModel.GetAttackModel().weapons[0].ejectX = 0;
        towerModel.GetAttackModel().weapons[0].ejectZ = -1;
        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().speed = 120f;
    }
    public override bool IsValidCrosspath(int[] tiers) =>
    ModHelper.HasMod("UltimateCrosspathing") || base.IsValidCrosspath(tiers);
}
public class M_1 : ModUpgrade<RobotTower>
{
    public override int Path => Middle;
    public override int Tier => 1;
    public override int Cost => 750;

    public override string DisplayName => "Actually hot!";
    public override string Description => "You can set Bloons on fire now";

    public override string Portrait => "Portrait_010";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var Fire = Game.instance.model.GetTower(TowerType.MortarMonkey, 0, 0, 2).GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetBehavior<AddBehaviorToBloonModel>();
        Fire.lifespan = 9;
        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(Fire);
        towerModel.GetAttackModel().weapons[0].projectile.collisionPasses = new[] { -1, 0 };
    }
}
public class B_1 : ModUpgrade<RobotTower>
{
    public override int Path => Bottom;
    public override int Tier => 1;
    public override int Cost => 750;

    public override string DisplayName => "Chat GPT V.2";
    public override string Description => "Towers in your range recieves a small Attack-Buff";

    public override string Portrait => "Portrait_001";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var Buff1 = new RangeSupportModel("RangeSupport", true, 0.1f, 0, "Range:Support", null, false, null, null);
        var Buff2 = new PierceSupportModel("PierceSupport", true, 1f, "Pierce:Support", null, false, "PierceBuff", "Zombey_Buff");
        var Buff3 = new RateSupportModel("MaudadoBuff", 0.95f, true, "Rate:Support", false, 1, null, "MaudadoBuff", "maudado_Buff", false);
        var Buff4 = new DamageSupportModel("DamageAddaptive", true, 1, "Damage:Support", null, false, false, 0);
        Buff1.ApplyBuffIcon<Robo_Buff>();
        towerModel.AddBehavior(Buff2);
        towerModel.AddBehavior(Buff3);
        towerModel.AddBehavior(Buff4);
        towerModel.AddBehavior(Buff1);
    }
}
public class T_1 : ModUpgrade<RobotTower>
{
    public override int Path => Top;
    public override int Tier => 1;
    public override int Cost => 750;

    public override string DisplayName => "Speedhack!";
    public override string Description => "You attack 15% faster";

    public override string Portrait => "Portrait_100";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.GetAttackModel().weapons[0].rate *= 0.85f;
    }
}
public class M_2 : ModUpgrade<RobotTower>
{
    public override int Path => Middle;
    public override int Tier => 2;
    public override int Cost => 1000;

    public override string DisplayName => "Aim Assist";
    public override string Description => "Your Projectiles aim a bit towards the Bloons";

    public override string Portrait => "Portrait_020";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var aimAssist = Game.instance.model.GetTowerFromId("Adora 20").GetAttackModel().weapons[0].projectile.GetBehavior<AdoraTrackTargetModel>().Duplicate();
        aimAssist.maximumSpeed = 110;
        aimAssist.minimumSpeed = 100;

        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(aimAssist);
    }
}
public class B_2 : ModUpgrade<RobotTower>
{
    public override int Path => Bottom;
    public override int Tier => 2;
    public override int Cost => 1000;

    public override string DisplayName => "Robot Stuff!";
    public override string Description => "You clean the Tarn Ability away from Bloons!";
    public override string Portrait => "Portrait_002";
    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var Spray = Game.instance.model.GetTower(TowerType.MonkeySub, 3,0,0).GetAttackModel(1).Duplicate();
        towerModel.AddBehavior(Spray);
    }
}
public class T_2 : ModUpgrade<RobotTower>
{
    public override int Path => Top;
    public override int Tier => 2;
    public override int Cost => 1000;

    public override string DisplayName => "Integrated Booster!";
    public override string Description => "You move 200% faster if the key 'Tab' is pressed";

    public override string Portrait => "Portrait_200";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        
    }
}
public class M_3 : ModUpgrade<RobotTower>
{
    public override int Path => Middle;
    public override int Tier => 3;
    public override int Cost => 7500;

    public override string DisplayName => "Fire Aura";
    public override string Description => "You are kinda hot.....like Isab but in generell this Upgrade adds a Fire Aura attack";

    public override string Portrait => "Portrait_030";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.ApplyDisplay<Robot030DP>();
        //towerModel.GetAttackModel().weapons[0].projectile.ApplyDisplay<Fireball300>();
        var Fire = Game.instance.model.GetTower(TowerType.TackShooter, 5, 0, 0).GetAttackModel(0).Duplicate();
        Fire.weapons[0].projectile.GetDamageModel().damage = 1;
        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().speed = 140f;

        Fire.weapons[0].projectile.pierce -= 2;
        if (towerModel.appliedUpgrades.Contains(UpgradeID<M_5>()))
        {
            Fire.weapons[0].projectile.pierce += 3;
            Fire.weapons[0].projectile.GetDamageModel().damage = 3;
        }
        towerModel.AddBehavior(Fire);
    }
}
public class B_3 : ModUpgrade<RobotTower>
{
    public override int Path => Bottom;
    public override int Tier => 3;
    public override int Cost => 5200;

    public override string DisplayName => "Integrated Bloonshipper!";
    public override string Description => "You shoot homing Tornados towards the Bloons";
    public override string Portrait => "Portrait_003";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.ApplyDisplay<Robot003DP>();
        towerModel.GetAttackModel().weapons[0].projectile.ApplyDisplay<Fireball003>();
        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().speed = 240f;
        towerModel.GetAttackModel().weapons[0].rate *= 0.9f;
        var Tornade = Game.instance.model.GetTower(TowerType.Druid, 3, 0, 0).GetAttackModel(1).Duplicate();
        var homing = Game.instance.model.GetTowerFromId("WizardMonkey-500").GetWeapon().projectile.GetBehavior<TrackTargetModel>().Duplicate();
        Tornade.weapons[0].emission = new ArcEmissionModel("ArcEmissionModel_", 2, 0, 180, null, false, false);
        Tornade.weapons[0].rate = 4f;
        Tornade.weapons[0].projectile.GetBehavior<TravelStraitModel>().Lifespan *= 10;
        Tornade.weapons[0].projectile.GetBehavior<TravelStraitModel>().speed *= 2f;
        if (towerModel.appliedUpgrades.Contains(UpgradeID<B_5>()))
        {
            var Tornade2 = Game.instance.model.GetTower(TowerType.Druid, 5, 0, 0).GetAttackModel().weapons.First(w => w.name.Contains("Superstorm")).Duplicate();

            Tornade.weapons[0].rate = 2f;
            Tornade.weapons[0].projectile.GetDamageModel().damage += 10f;
            Tornade.weapons[0].projectile.GetBehavior<TravelStraitModel>().Lifespan *= 15;
            Tornade.weapons[0].projectile.GetBehavior<TravelStraitModel>().speed *= 3f;
            Tornade.weapons[0].projectile.pierce += 9999;
            Tornade2.rate = 9f;
            Tornade2.projectile.GetBehavior<TravelStraitModel>().Lifespan *= 45;
            Tornade2.projectile.GetDamageModel().damage += 125;
            Tornade2.projectile.GetBehavior<TravelStraitModel>().speed *= 1f;
            Tornade2.projectile.pierce += 999999;
            Tornade2.projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ceramic", "Ceramic", 1, 75, false, false));
            Tornade2.projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Fortified", "Fortified", 2, 1, false, false));
            Tornade2.projectile.AddBehavior(new DamageModifierForTagModel("Bad", "Bad", 3, 1, false, true));
            Tornade2.projectile.AddBehavior(new TrackTargetModel("", 500, homing.trackNewTargets, true, homing.maxSeekAngle, homing.ignoreSeekAngle, homing.turnRate * 4, homing.overrideRotation, homing.useLifetimeAsDistance));
            towerModel.GetAttackModel().AddWeapon(Tornade2);
        }
        
        Tornade.weapons[0].projectile.AddBehavior(new TrackTargetModel("", 500, homing.trackNewTargets, true, homing.maxSeekAngle, homing.ignoreSeekAngle, homing.turnRate * 4, homing.overrideRotation, homing.useLifetimeAsDistance));
        towerModel.AddBehavior(Tornade);
    }
}
public class T_3 : ModUpgrade<RobotTower>
{
    public override int Path => Top;
    public override int Tier => 3;
    public override int Cost => 5600;

    public override string DisplayName => "Anti Offtrack!";
    public override string Description => "You always move towards the Bloons, and occasionally, cacti are placed along the track";

    public override string Portrait => "Portrait_300";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.ApplyDisplay<Robot300DP>();
        towerModel.GetAttackModel().weapons[0].projectile.ApplyDisplay<Fireball300>();
        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().speed = 160f;
        var Spike = Game.instance.model.GetTower(TowerType.NinjaMonkey, 0, 0, 2).GetAttackModel(1).Duplicate();
        Spike.range = 9;
        Spike.weapons[0].rate *= 2f;
        Spike.weapons[0].projectile.ApplyDisplay<Cactus1DP>();
        
        Spike.weapons[0].projectile.pierce = 12;
        Spike.weapons[0].projectile.GetDamageModel().damage = 8;
        var SpikeExplosionProjectile = Game.instance.model.GetTower(TowerType.BoomerangMonkey, 2, 0, 2).GetAttackModel().weapons[0].projectile.Duplicate();
        SpikeExplosionProjectile.pierce = 3;
        SpikeExplosionProjectile.ApplyDisplay<Small_Spike>();
        SpikeExplosionProjectile.GetDamageModel().damage = 5;
        SpikeExplosionProjectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Fortified", "Fortified", 1, 8, false, false));
        SpikeExplosionProjectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ddt", "Ddt", 1, 18, false, true));
        SpikeExplosionProjectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ceramic", "Ceramic", 1, 7, false, false));
        SpikeExplosionProjectile.AddBehavior(new DamageModifierForTagModel("Moab", "Moab", 1, 6, false, true));
        SpikeExplosionProjectile.AddBehavior(new DamageModifierForTagModel("Bfb", "Bfb", 1, 8, false, true));
        SpikeExplosionProjectile.AddBehavior(new DamageModifierForTagModel("Zomg", "Zomg", 1, 13, false, true));
        SpikeExplosionProjectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        Spike.weapons[0].projectile.AddBehavior(new CreateProjectileOnExpireModel("SpectralShards", SpikeExplosionProjectile, new ArcEmissionModel("", 9, 0, 360, null, false, false), false));
        Spike.weapons[0].projectile.AddBehavior(new CreateProjectileOnExhaustFractionModel("SpectralShards", SpikeExplosionProjectile, new ArcEmissionModel("", 9, 0, 360, null, false, false), 1, 1, false, false, true));
        towerModel.AddBehavior(Spike);
        var Aim = Game.instance.model.GetTower(TowerType.SniperMonkey, 1, 2, 0).GetAttackModel(0).Duplicate();
        Aim.weapons[0].rate = 0.8f;
        towerModel.AddBehavior(Aim);
    }
}

public class M_4 : ModUpgrade<RobotTower>
{
    public override int Path => Middle;
    public override int Tier => 4;
    public override int Cost => 10500;

    public override string DisplayName => "Balls of Fire";
    public override string Description => "You have now a global range Fire-Shooting Attack. Ability: Shoot 8 gigantic Fireballs around you";

    public override string Portrait => "Portrait_040";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.ApplyDisplay<Robot040DP>();
        var OldAttackModel = towerModel.GetAttackModel().Duplicate();
        TowerModelBehaviorExt.GetBehavior<AttackModel>(towerModel).fireWithoutTarget = true;
        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().speed = 175;
        towerModel.GetAttackModel().weapons[0].projectile.ignoreBlockers = true;
        towerModel.GetAttackModel().weapons[0].projectile.canCollisionBeBlockedByMapLos = false;
        towerModel.GetAttackModel().weapons[0].rate *= 1.5f;
        if (towerModel.appliedUpgrades.Contains(UpgradeID<M_5>()))
        {
            towerModel.GetAttackModel().weapons[0].rate *= 0.3f;
            towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().speed = 200;
            towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("Moabs", "Moabs", 1, 5, false, true));
            towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ceramic", "Ceramic", 1, 7, false, false));
            OldAttackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("Moabs", "Moabs", 1, 10, false, true));
        }
        towerModel.AddBehavior(OldAttackModel);
        //Ability
        var abilityModel = new AbilityModel("AbilityModel_Middle_4", "SmallFire",
            "s", 0, 0,
            GetSpriteReference("040"), 90f, null, false, false, null,
            0, 0, 9999999, false, false);

        var activateAttackModel = new ActivateAttackModel("ActivateAttackModel_SmallFire", 0, true, new Il2CppReferenceArray<AttackModel>(1), true, false, false, false, false);
        abilityModel.AddBehavior(activateAttackModel);
        var attackModel1 = activateAttackModel.attacks[0] = Game.instance.model.GetTower(TowerType.SuperMonkey, 0, 0, 2).GetAttackModel().Duplicate();
        attackModel1.range = 2000;
        attackModel1.fireWithoutTarget = true;
        attackModel1.attackThroughWalls = true;
        attackModel1.weapons[0].emission = new ArcEmissionModel("fireballSmall", 8, 0, 360, null, false, false);
        attackModel1.weapons[0].projectile.ignoreBlockers = true;
        attackModel1.weapons[0].projectile.canCollisionBeBlockedByMapLos = false;
        attackModel1.weapons[0].projectile.ApplyDisplay<SmallFireDisplay>();
        attackModel1.weapons[0].projectile.pierce = 99999999;
        attackModel1.weapons[0].projectile.maxPierce = 99999999;
        attackModel1.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        attackModel1.weapons[0].projectile.GetDamageModel().damage = 250;
        attackModel1.weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan *= 10;
        attackModel1.weapons[0].projectile.GetBehavior<TravelStraitModel>().speed *= 0.4f;
        activateAttackModel.AddChildDependant(attackModel1);
        towerModel.AddBehavior(abilityModel);
    }
}
public class B_4 : ModUpgrade<RobotTower>
{
    public override int Path => Bottom;
    public override int Tier => 4;
    public override int Cost => 9600;

    public override string DisplayName => "Upgraded Bloonshipper!";
    public override string Description => "You can push MOABs aside and sometimes shoot stunnig ligtnings at every enemy";
    public override string Portrait => "Portrait_004";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.ApplyDisplay<Robot004DP>();
        var attackModel = towerModel.GetAttackModel();
        var lightning = Game.instance.model.GetTower(TowerType.Druid, 2).GetAttackModel().Duplicate();
        lightning.RemoveBehavior<RotateToTargetModel>();
        lightning.GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
        lightning.range = 999;
        lightning.weapons[1].rate *= 3;
        
        lightning.weapons[1].projectile.GetDamageModel().damage = 3;
        lightning.weapons[1].projectile.pierce = 500;
        lightning.weapons[1].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;

        lightning.weapons[1].projectile.GetBehavior<DamageModel>().overrideDistributeBlocker = true;
        lightning.weapons[1].projectile.collisionPasses = new int[] { 0, -1 };
        if (towerModel.appliedUpgrades.Contains(UpgradeID<B_5>()))
        {
            lightning.weapons[1].rate = 2;
            lightning.weapons[1].projectile.GetDamageModel().damage = 13;
            lightning.weapons[1].projectile.pierce = 12500;
            lightning.weapons[1].projectile.AddBehavior(new FreezeModel("FreezeModel_", 0f, 1.75f, "LigntningStun", 999, "Stun", true, new GrowBlockModel("GrowBlockModel_"), null, 1f, true, true));
            attackModel.weapons[0].projectile.AddBehavior(new WindModel("WindModel_",  2.7f,3, 1f, true, null, 5, "Ddt", 5));
            attackModel.weapons[0].projectile.AddBehavior(new WindModel("WindModel_",  2.5f,3, 1f, true, null, 3, "Moab", 5));
            attackModel.weapons[0].projectile.AddBehavior(new WindModel("WindModel_",  2.5f,3, 1f, true, null, 2, "Bfb", 5));
            attackModel.weapons[0].projectile.AddBehavior(new WindModel("WindModel_",  2.5f,3, 1f, true, null, 1, "Zomg", 5));
        }
        else
        {
            attackModel.weapons[0].projectile.AddBehavior(new WindModel("WindModel_", 2, 2.5f, 0.75f, true, null, 5, "Ddt", 3));
            attackModel.weapons[0].projectile.AddBehavior(new WindModel("WindModel_", 2, 2.5f, 0.75f, true, null, 3, "Moab", 3));
            attackModel.weapons[0].projectile.AddBehavior(new WindModel("WindModel_", 2, 2.5f, 0.75f, true, null, 2, "Bfb", 3));
            attackModel.weapons[0].projectile.AddBehavior(new WindModel("WindModel_", 2, 2.5f, 0.75f, true, null, 1, "Zomg", 3));
            lightning.weapons[1].projectile.AddBehavior(new FreezeModel("FreezeModel_", 0f, 1.5f, "LigntningStun", 999, "Stun", true, new GrowBlockModel("GrowBlockModel_"), null, 0.75f, true, true));
        }
        lightning.RemoveWeapon(lightning.weapons[0]);
        towerModel.AddBehavior(lightning);
    }
}
public class T_4 : ModUpgrade<RobotTower>
{
    public override int Path => Top;
    public override int Tier => 4;
    public override int Cost => 12500;

    public override string DisplayName => "Cactus Man";
    public override string Description => "You launch spikes in all directions";

    public override string Portrait => "Portrait_400";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.ApplyDisplay<Robot400DP>();
        var attackModel1 = Game.instance.model.GetTower(TowerType.SuperMonkey, 0, 0, 2).GetAttackModel().Duplicate();
        attackModel1.weapons[0].rate = 3;
        attackModel1.fireWithoutTarget = true;
        attackModel1.weapons[0].emission = new ArcEmissionModel("fireballSmall", 16, 0, 360, null, false, false);
        attackModel1.weapons[0].projectile.ApplyDisplay<Small_Spike>();
        attackModel1.weapons[0].projectile.pierce = 2;
        attackModel1.weapons[0].projectile.maxPierce = 10;
        attackModel1.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        attackModel1.weapons[0].projectile.GetDamageModel().damage = 10;
        attackModel1.weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan *= 3;
        attackModel1.weapons[0].projectile.GetBehavior<TravelStraitModel>().speed *= 0.8f;
        if (towerModel.appliedUpgrades.Contains(UpgradeID<M_1>()))
        {
            attackModel1.weapons[0].projectile.ApplyDisplay<Small_Fire_Spike>();
            var Fire = Game.instance.model.GetTower(TowerType.MortarMonkey, 0, 0, 2).GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetBehavior<AddBehaviorToBloonModel>();
            Fire.lifespan = 5;
            attackModel1.weapons[0].projectile.AddBehavior(Fire);
            attackModel1.weapons[0].projectile.collisionPasses = new[] { -1, 0 };
        }
        if (towerModel.appliedUpgrades.Contains(UpgradeID<T_5>()))
        {
            attackModel1.weapons[0].emission = new ArcEmissionModel("fireballSmall", 24, 0, 360, null, false, false);
            attackModel1.weapons[0].rate = 1.5f;
            attackModel1.weapons[0].projectile.GetDamageModel().damage = 51;
            attackModel1.weapons[0].projectile.pierce = 3;
            attackModel1.weapons[0].projectile.maxPierce = 20;
            var bomb = Game.instance.model.GetTower(TowerType.BombShooter, 0).GetAttackModel().weapons[0].projectile.Duplicate();
            var blast = bomb.GetBehavior<CreateProjectileOnContactModel>().projectile.Duplicate();
            var explosion = bomb.GetBehavior<CreateEffectOnContactModel>().Duplicate();

            blast.GetDamageModel().damage = 8;
            blast.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ceramic", "Ceramic", 1, 12, false, false));
            blast.radius = 25;
            blast.pierce = 32;
            blast.GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
            var contactModel = new CreateProjectileOnContactModel("smallExplosion", blast, new ArcEmissionModel("ArcEmissionModel_", 1, 0, 0, null, false, false), true, false, false)
            { name = "RifleBlast_" };
            attackModel1.weapons[0].projectile.AddBehavior(contactModel);
            attackModel1.weapons[0].projectile.AddBehavior(explosion);

        }
        towerModel.AddBehavior(attackModel1);
    }
}
public class M_5 : ModUpgrade<RobotTower>
{
    public override int Path => Middle;
    public override int Tier => 5;
    public override int Cost => 32500;

    public override string DisplayName => "Overheated like Isab";
    public override string Description => "Your Ability now shoots 16 Fireballs in 3 bursts";
    public override string Portrait => "Portrait_050";
    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.GetAttackModel().weapons[0].rate *= 0.75f;
        towerModel.ApplyDisplay<Robot050DP>();


        //Ability
        towerModel.RemoveBehavior<AbilityModel>();
        var abilityModel = new AbilityModel("AbilityModel_Middle_5", "SmallFire",
            "s", 0, 0,
            GetSpriteReference("050"), 85f, null, false, false, null,
            0, 0, 9999999, false, false);

        var activateAttackModel = new ActivateAttackModel("ActivateAttackModel_SmallFire", 2.1f, true, new Il2CppReferenceArray<AttackModel>(1), true, false, false, false, false);
        abilityModel.AddBehavior(activateAttackModel);
        var attackModel1 = activateAttackModel.attacks[0] = Game.instance.model.GetTower(TowerType.SuperMonkey, 0, 0, 2).GetAttackModel().Duplicate();
        attackModel1.range = 2000;
        attackModel1.weapons[0].rate = 1;
        attackModel1.fireWithoutTarget = true;
        attackModel1.attackThroughWalls = true;
        attackModel1.weapons[0].emission = new ArcEmissionModel("fireballSmall", 16, 0, 360, null, false, false);
        attackModel1.weapons[0].projectile.ignoreBlockers = true;
        attackModel1.weapons[0].projectile.canCollisionBeBlockedByMapLos = false;
        attackModel1.weapons[0].projectile.ApplyDisplay<BigFireDisplay>();
        attackModel1.weapons[0].projectile.pierce = 99999999;
        attackModel1.weapons[0].projectile.maxPierce = 99999999;
        attackModel1.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        attackModel1.weapons[0].projectile.GetDamageModel().damage = 751;
        attackModel1.weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan *= 10;
        attackModel1.weapons[0].projectile.GetBehavior<TravelStraitModel>().speed *= 0.5f;
        activateAttackModel.AddChildDependant(attackModel1);
        towerModel.AddBehavior(abilityModel);
    }
}
public class B_5 : ModUpgrade<RobotTower>
{
    public override int Path => Bottom;
    public override int Tier => 5;
    public override int Cost => 42500;

    public override string DisplayName => "Living Hurricane";
    public override string Description => "There's a little bit of Bloon in the air";
    public override string Portrait => "Portrait_005";
    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.ApplyDisplay<Robot005DP>();
    }
}
public class T_5 : ModUpgrade<RobotTower>
{
    public override int Path => Top;
    public override int Tier => 5;
    public override int Cost => 37500;

    public override string DisplayName => "Cactus God";
    public override string Description => "What's better than spikes? Exploding spikes!";
    public override string Portrait => "Portrait_500";
    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.ApplyDisplay<Robot500DP>();
        towerModel.GetAttackModel().weapons[0].rate *= 0.33f;
        towerModel.GetAttackModel().range *= 1.5f;
        towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage += 8;
        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().speed *= 1.25f;
        towerModel.GetAttackModel().weapons[0].projectile.pierce += 15;
    }
}
public class Paragon : ModParagonUpgrade<RobotTower>
{
    public override int Cost => 900000;
    public override string Description => "Hasta la vista ,Baby! Every Level grants you 2% more damage and other bosting effects";
    public override string DisplayName => "Terminator";
    public override string Portrait => "Paragon";
    public override string Icon => "Paragon";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        int ParagonLevel = towerModel.tier;
        

        //AttackOne
        towerModel.ApplyDisplay<Robot_Paragon_DP>();
        towerModel.GetAttackModel().weapons[0].projectile.ApplyDisplay<Fireball003>();
        towerModel.GetAttackModel().weapons[0].emission = new ArcEmissionModel("paragonAttack", 16, 0, 360, null, false, false);
        towerModel.GetAttackModel().fireWithoutTarget = true;
        towerModel.GetAttackModel().weapons[0].rate = 1.25f;
        towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        towerModel.GetAttackModel().weapons[0].projectile.pierce = 50 + ParagonLevel * 1;
        towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage = 1000 + ParagonLevel * 20;
        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(new WindModel("WindModel_", 16, 20, 1f, true, null, 1.15f, "Ddt", 2));
        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan *= 4;
        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().speed *= 6f;
        towerModel.GetAttackModel().weapons[0].projectile.ignoreBlockers = true;
        towerModel.GetAttackModel().weapons[0].projectile.canCollisionBeBlockedByMapLos = false;
        var aimAssist = Game.instance.model.GetTowerFromId("Adora 20").GetAttackModel().weapons[0].projectile.GetBehavior<AdoraTrackTargetModel>().Duplicate();
        aimAssist.maximumSpeed = 110;
        aimAssist.minimumSpeed = 100;
        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(aimAssist);
        //AttackTwo
        var am2 = Game.instance.model.GetTower(TowerType.DartMonkey, 0, 0, 2).GetAttackModel(0).Duplicate();
        am2.fireWithoutTarget = true;
        am2.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        am2.weapons[0].ejectY = 0;
        am2.weapons[0].ejectX = 0;
        am2.weapons[0].ejectZ = -6;
        am2.weapons[0].projectile.GetDamageModel().damage = 400 + ParagonLevel * 4f;
        am2.weapons[0].projectile.AddBehavior(new FreezeModel("FreezeModel_", 0.01f, 3.5f, "LigntningStun", 999, "Stun", true, new GrowBlockModel("GrowBlockModel_"), null, 1f, true, true));
        am2.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Fortified", "Fortified", 2, 0, false, false));
        am2.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ddt", "Ddt", 2, 0, false, true));
        am2.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ceramic", "Ceramic", 3, 0, false, false));
        am2.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("Moab", "Moab", 1.25f, 0, false, true));
        am2.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("Bfb", "Bfb", 1.5f, 0, false, true));
        am2.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("Zomg", "Zomg", 1.75f, 0, false, true));
        am2.weapons[0].projectile.pierce = 5;
        am2.weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan *= 2.5f;
        am2.weapons[0].projectile.GetBehavior<TravelStraitModel>().speed *= 9f;
        am2.weapons[0].projectile.ignoreBlockers = true;
        am2.weapons[0].projectile.canCollisionBeBlockedByMapLos = false;
        am2.weapons[0].projectile.ApplyDisplay<FireballGreen555>();
        am2.weapons[0].rate = 0.2f;
        towerModel.AddBehavior(am2);
        //AttackThree
        var Spike = Game.instance.model.GetTower(TowerType.NinjaMonkey, 0, 0, 2).GetAttackModel(1).Duplicate();
        Spike.range = 9;
        Spike.weapons[0].rate *= 6f;
        Spike.weapons[0].projectile.ApplyDisplay<BigCactus1DP>();
        Spike.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        Spike.weapons[0].projectile.pierce = 100;
        Spike.weapons[0].projectile.GetDamageModel().damage = 500 + ParagonLevel * 10;
        var SpikeExplosionProjectile = Game.instance.model.GetTower(TowerType.BoomerangMonkey, 2, 0, 2).GetAttackModel().weapons[0].projectile.Duplicate();
        SpikeExplosionProjectile.pierce = 30;
        SpikeExplosionProjectile.ApplyDisplay<Big_Fire_Spike>();
        SpikeExplosionProjectile.GetDamageModel().damage = 5000 + ParagonLevel * 100;
        SpikeExplosionProjectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Fortified", "Fortified", 3, 8, false, false));
        SpikeExplosionProjectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ddt", "Ddt", 4, 18, false, true));
        SpikeExplosionProjectile.AddBehavior(new DamageModifierForTagModel("Moab", "Moab", 1, 600, false, true));
        SpikeExplosionProjectile.AddBehavior(new DamageModifierForTagModel("Bfb", "Bfb", 1, 1200, false, true));
        SpikeExplosionProjectile.AddBehavior(new DamageModifierForTagModel("Zomg", "Zomg", 1, 2400, false, true));
        SpikeExplosionProjectile.AddBehavior(new DamageModifierForTagModel("Bad", "Bad", 1, 5000, false, true));
        SpikeExplosionProjectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        Spike.weapons[0].projectile.AddBehavior(new CreateProjectileOnExpireModel("SpectralShards", SpikeExplosionProjectile, new ArcEmissionModel("", 22, 0, 360, null, false, false), false));
        Spike.weapons[0].projectile.AddBehavior(new CreateProjectileOnExhaustFractionModel("SpectralShards", SpikeExplosionProjectile, new ArcEmissionModel("", 18, 0, 360, null, false, false), 1, 1, false, false, true));
        towerModel.AddBehavior(Spike);
        var Aim = Game.instance.model.GetTower(TowerType.SniperMonkey, 2, 4, 0).GetAttackModel(0).Duplicate();
        Aim.weapons[0].rate = 1.2f;
        towerModel.AddBehavior(Aim);
        //AbilityOne
        var abilityModel = new AbilityModel("AbilityModel_Paragon", "TotalBurn",
         "s", 0, 0,
        GetSpriteReference("ParagonUltaFire"), 90f, null, false, false, null,
        0, 0, 9999999, false, false);
        var activateAttackModel = new ActivateAttackModel("ActivateAttackModel_SmallFire", 3.1f, true, new Il2CppReferenceArray<AttackModel>(1), true, false, false, false, false);
        abilityModel.AddBehavior(activateAttackModel);
        var attackModel1 = activateAttackModel.attacks[0] = Game.instance.model.GetTower(TowerType.SuperMonkey, 0, 0, 2).GetAttackModel().Duplicate();
        attackModel1.range = 2000;
        attackModel1.weapons[0].rate = 0.5f;
        attackModel1.fireWithoutTarget = true;
        attackModel1.attackThroughWalls = true;
        attackModel1.weapons[0].emission = new ArcEmissionModel("fireballSmall", 32, 0, 360, null, false, false);
        attackModel1.weapons[0].projectile.ignoreBlockers = true;
        attackModel1.weapons[0].projectile.canCollisionBeBlockedByMapLos = false;
        attackModel1.weapons[0].projectile.ApplyDisplay<BigFireDisplay>();
        attackModel1.weapons[0].projectile.pierce = 99999999;
        attackModel1.weapons[0].projectile.maxPierce = 99999999;
        attackModel1.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        attackModel1.weapons[0].projectile.GetDamageModel().damage = 2500 + ParagonLevel * 50;
        attackModel1.weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan *= 8;
        attackModel1.weapons[0].projectile.GetBehavior<TravelStraitModel>().speed *= 0.8f;
        activateAttackModel.AddChildDependant(attackModel1);
        towerModel.AddBehavior(abilityModel);
        //BuffOne
        var Buff1 = new RangeSupportModel("RangeSupport", true, 0.25f+ 0.005f * ParagonLevel, 0, "Range:Support", null, false, null, null);
        var Buff2 = new PierceSupportModel("PierceSupport", true, 2f + 0.04f * ParagonLevel, "Pierce:Support", null, false, "PierceBuff", "Zombey_Buff");
        var Buff3 = new RateSupportModel("SpeedBuff", 0.80f - 0.004f * ParagonLevel, true, "Rate:Support", false, 1, null, "Speed", "maudado_Buff", false);
        var Buff4 = new DamageSupportModel("DamageAddaptive", true, 10 + 0.2f * ParagonLevel, "Damage:Support", null, false, false, 0);
        Buff1.ApplyBuffIcon<Robo_Paragon_Buff>();
        towerModel.AddBehavior(Buff2);
        towerModel.AddBehavior(Buff3);
        towerModel.AddBehavior(Buff4);
        towerModel.AddBehavior(Buff1);
    }
}
