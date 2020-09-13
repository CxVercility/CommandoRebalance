using System;
using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;


namespace CommandoRebalance
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Vercility.RoR", "CommandoRebalance", "1.1.0")]
    public class Changes : BaseUnityPlugin
    {
        public static ConfigEntry<float> MaxDistance { get; set; }
        public static ConfigEntry<float> Radius { get; set; }
        public static ConfigEntry<bool> Recoil { get; set; }
        public static ConfigEntry<bool> Falloff { get; set; }

        public static ConfigEntry<float> SpreadBloom { get; set; }
        public static ConfigEntry<float> Damage { get; set; }
        public static ConfigEntry<float> IFrame { get; set; }




        public void Awake()
        {
            MaxDistance = Config.Bind<float>("Stats", "MaxDistance", 200f, "Max Distance Bullets can travel. Default is 200");
            Radius = Config.Bind<float>("Stats", "Radius", 1f, "Bullet Radius Multiplier. Default is 1");
            Damage = Config.Bind<float>("Stats", "Damage", 1f, "Damage Multiplier. Default is 1");
            Recoil = Config.Bind<bool>("Misc", "Disable Recoil", true, "Set to true if you want to disable recoil/muzzle spread");
            Falloff = Config.Bind<bool>("Misc", "Disable Damage Fall off", true, "Set to true if you want to disable damage fall off over distance");
            IFrame = Config.Bind<float>("Misc", "IFrame duration", 0f, "Duration of Invincibility after using utility skill in seconds. Default is 0");
            On.RoR2.BulletAttack.Fire += (orig,self) =>
            {
                self.maxDistance = MaxDistance.Value;
                self.damage = self.damage * Damage.Value;
                self.radius = self.radius * Radius.Value;
                if (Falloff.Value)
                {
                    self.falloffModel = BulletAttack.FalloffModel.None;
                }
                if (Recoil.Value)
                {
                    self.minSpread = 0.0f;
                    self.maxSpread = 0.0f;
                    self.owner.GetComponent<CharacterBody>().SetSpreadBloom(0.0f, false);
                }
                orig(self);
                  
            };

            // Copied & Fixed from Wordam, don't even ask me how that works
            IL.EntityStates.Commando.DodgeState.OnEnter += (il1) =>
            {
                if(IFrame.Value == 0f)
                {
                    return;
                }
                var c2 = new ILCursor(il1);
                c2.Emit(OpCodes.Ldarg_0);

                c2.EmitDelegate<Action<EntityStates.Commando.DodgeState>>((d) =>
                {
                    if (d.outer)
                    {
                        if (d.outer.commonComponents.characterBody)
                        {
                            d.outer.commonComponents.characterBody.AddTimedBuff(BuffIndex.Immune, IFrame.Value);
                        }
                    }
                });
            };

            IL.EntityStates.Commando.SlideState.OnEnter += (il1) =>
            {
                if (IFrame.Value == 0f)
                {
                    return;
                }
                var c2 = new ILCursor(il1);
                c2.Emit(OpCodes.Ldarg_0);

                c2.EmitDelegate<Action<EntityStates.Commando.SlideState>>((d) =>
                {
                    if (d.outer)
                    {
                        if (d.outer.commonComponents.characterBody)
                        {
                            d.outer.commonComponents.characterBody.AddTimedBuff(BuffIndex.Immune, IFrame.Value);
                        }
                    }
                });
            };
        }

    }
}
