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




[assembly: MelonInfo(typeof(robot.robot), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace robot;

public class robot : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ModHelper.Msg<robot>("You got terminated!");
    }
    static int rnd(int min, int max)
    {
        System.Random random = new System.Random();
        return random.Next(min, max);
    }
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
    public override void OnUpdate()
    {
        if (InGameData.CurrentGame != null && InGame.Bridge != null)
        {
            List<Tower> towers = InGame.instance.GetTowers();
            int TC = towers.Count;

            for (int i = 0; i < TC; i++)
            {
                if (towers[i] != null && towers[i].towerModel.name.Contains("Robot"))
                {
                    int TabBooster = 1;
                    if (Input.GetKey(KeyCode.Tab) && towers[i].towerModel.tiers[0] >= 2)
                    {
                        TabBooster = 2;
                        //ModHelper.Msg<robot>("Tab");
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
        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().speed = 120f;
    }

}
public class M_1 : ModUpgrade<RobotTower>
{
    public override int Path => Middle;
    public override int Tier => 1;
    public override int Cost => 750;

    public override string DisplayName => "Actually hot!";
    public override string Description => "You can set Bloons on fire now";



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
    public override string Description => "You are kinda hot.....like Isab";



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
    public override string Description => "You change your fire into Air acc Bloonshipper!";

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
    public override int Cost => 3500;

    public override string DisplayName => "Anti Offtrack!";
    public override string Description => "You always move towards the bloons and place some cacteen on the track it";



    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.ApplyDisplay<Robot300DP>();
        towerModel.GetAttackModel().weapons[0].projectile.ApplyDisplay<Fireball300>();
        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().speed = 160f;
        var Spike = Game.instance.model.GetTower(TowerType.NinjaMonkey, 0, 0, 2).GetAttackModel(1).Duplicate();
        Spike.range = 99;
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
        SpikeExplosionProjectile.AddBehavior(new DamageModifierForTagModel("Zomg", "Zomg", 1, 16, false, true));
        SpikeExplosionProjectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        Spike.weapons[0].projectile.AddBehavior(new CreateProjectileOnExpireModel("SpectralShards", SpikeExplosionProjectile, new ArcEmissionModel("", 9, 0, 360, null, false, false), false));
        Spike.weapons[0].projectile.AddBehavior(new CreateProjectileOnExhaustFractionModel("SpectralShards", SpikeExplosionProjectile, new ArcEmissionModel("", 9, 0, 360, null, false, false), 1, 1, false, false, true));
        //towerModel.AddBehavior(Spike);
        var Aim = Game.instance.model.GetTower(TowerType.SniperMonkey, 1, 2, 0).GetAttackModel(0).Duplicate();
        Aim.weapons[0].rate = 0.8f;
        towerModel.AddBehavior(Aim);
    }
}

public class M_4 : ModUpgrade<RobotTower>
{
    public override int Path => Middle;
    public override int Tier => 4;
    public override int Cost => 12500;

    public override string DisplayName => "Fire Aura";
    public override string Description => "You are kinda hot.....like Isab";



    public override void ApplyUpgrade(TowerModel towerModel)
    {
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
    public override int Cost => 12000;

    public override string DisplayName => "Integrated Bloonshipper!";
    public override string Description => "You can also push Moab Bloons Aside and also sometimes shoot stunnig Ligtnings";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
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
    public override int Cost => 12000;

    public override string DisplayName => "Spike";
    public override string Description => "You shoot spikes ";



    public override void ApplyUpgrade(TowerModel towerModel)
    {
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
            attackModel1.weapons[0].rate = 1.5f;
            attackModel1.weapons[0].projectile.GetDamageModel().damage = 50;
            attackModel1.weapons[0].projectile.pierce = 3;
            attackModel1.weapons[0].projectile.maxPierce = 20;
            var bomb = Game.instance.model.GetTower(TowerType.BombShooter, 0).GetAttackModel().weapons[0].projectile.Duplicate();
            var blast = bomb.GetBehavior<CreateProjectileOnContactModel>().projectile.Duplicate();
            var explosion = bomb.GetBehavior<CreateEffectOnContactModel>().Duplicate();

            blast.GetDamageModel().damage = 8;
            blast.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ceramic", "Ceramic", 1, 12, false, false));
            blast.radius = 15;
            blast.pierce = 30;
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
    public override int Cost => 12500;

    public override string DisplayName => "Fire Aura";
    public override string Description => "You are kinda hot.....like Isab";



    public override void ApplyUpgrade(TowerModel towerModel)
    {




        //Ability
        towerModel.RemoveBehavior<AbilityModel>();
        var abilityModel = new AbilityModel("AbilityModel_Middle_4", "SmallFire",
            "s", 0, 0,
            GetSpriteReference("040"), 90f, null, false, false, null,
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
        attackModel1.weapons[0].projectile.GetDamageModel().damage = 750;
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
    public override int Cost => 12000;

    public override string DisplayName => "Integrated Bloonshipper!";
    public override string Description => "You can also push Moab Bloons Aside";

    public override void ApplyUpgrade(TowerModel towerModel)
    {

    }
}
public class T_5 : ModUpgrade<RobotTower>
{
    public override int Path => Top;
    public override int Tier => 5;
    public override int Cost => 25000;

    public override string DisplayName => "Anti Offtrack!";
    public override string Description => "You always move towards the bloons and place some cacteen on the track it";



    public override void ApplyUpgrade(TowerModel towerModel)
    {

    }
}
