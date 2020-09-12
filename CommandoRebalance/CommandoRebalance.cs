using System;
using System.IO;
using System.Text;
using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;


namespace CommandoRebalance
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Vercility.RoR", "CommandoRebalance", "1.0")]
    public class Changes : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.BulletAttack.Fire += (orig,self) =>
            {
                
                self.minSpread = 0.0f;
                self.maxSpread = 0.0f;
                orig(self);
                self.owner.GetComponent<CharacterBody>().SetSpreadBloom(0.0f, false);     
            };

            // Copied & Fixed from Wordam, don't even ask me how that works
            IL.EntityStates.Commando.DodgeState.OnEnter += (il1) =>
            {
                var c2 = new ILCursor(il1);
                c2.Emit(OpCodes.Ldarg_0);

                c2.EmitDelegate<Action<EntityStates.Commando.DodgeState>>((d) =>
                {
                    if (d.outer)
                    {
                        if (d.outer.commonComponents.characterBody)
                        {
                            d.outer.commonComponents.characterBody.AddTimedBuff(BuffIndex.Immune, 0.1f);
                        }
                    }
                });
            };

            IL.EntityStates.Commando.SlideState.OnEnter += (il1) =>
            {
                var c2 = new ILCursor(il1);
                c2.Emit(OpCodes.Ldarg_0);

                c2.EmitDelegate<Action<EntityStates.Commando.SlideState>>((d) =>
                {
                    if (d.outer)
                    {
                        if (d.outer.commonComponents.characterBody)
                        {
                            d.outer.commonComponents.characterBody.AddTimedBuff(BuffIndex.Immune, 0.1f);
                        }
                    }
                });
            };
        }

    }
}
