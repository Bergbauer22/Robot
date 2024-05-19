using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace robot
{
    public class Small_Spike : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;
        public override float Scale => 0.3f;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        { Set2DTexture(node, "Small_Spike"); }
    }
    public class Small_Fire_Spike : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;
        public override float Scale => 0.3f;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        { Set2DTexture(node, "Small_Fire_Spike"); }
    }
    public class Cactus1DP : ModCustomDisplay
    {
        public override string AssetBundleName => "robot";
        public override string PrefabName => "Cactus_2";
        public override float Scale => 1;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var meshRenderer in node.GetMeshRenderers())
            {
                //meshRenderer.ApplyOutlineShader();

                //meshRenderer.SetOutlineColor( new Color(1f, 1f, 1f));
            }
        }
    }
    public class SmallFireDisplay : ModDisplay
    {
        public override PrefabReference BaseDisplayReference => new PrefabReference("01dfdf7fe33be28409a9c2e1db9bbec0");
        public override float Scale => 1.9f;
    }
    public class BigFireDisplay : ModDisplay
    {
        public override PrefabReference BaseDisplayReference => new PrefabReference("01dfdf7fe33be28409a9c2e1db9bbec0");
        public override float Scale => 2.5f;
    }
}
