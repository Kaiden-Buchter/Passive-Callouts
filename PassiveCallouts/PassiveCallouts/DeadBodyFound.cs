using CitizenFX.Core;
using FivePD.API;
using FivePD.API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Callout_Pack_Test_2023
{
    [CalloutProperties("Dead Body","Husky","v1.0")]
    internal class DeadBodyFound : Callout
    {
        public List<Vector3> calloutLocations = new List<Vector3>()
        {
            new Vector3(-1567.34f, 749.92f, 192.58f),
            new Vector3(-1573.06f, 771.45f, 189.19f),
            new Vector3(-1462.25f, 179.30f, 54.77f),
            new Vector3(-27.44f, -1307.06f, 29.56f),
            new Vector3(-570.42f, -1676.99f, 19.62f),
            new Vector3(379.06f, -1830.08f, 28.67f),
            new Vector3(124.96f, -1185.44f, 29.50f),
            new Vector3(-97.25f, -1001.56f, 21.28f),
            new Vector3(266.03f, -2430.39f, 8.04f),
            new Vector3(-1464.79f, -1092.01f, 0.29f)
        };
        public Vector3 calloutLocation;
        
        public Ped victim, suspect, witness;
        
        public DeadBodyFound()
        {
            calloutLocation = calloutLocations.SelectRandom();

            InitInfo(calloutLocation);
            ShortName = "Dead Body Found";
            CalloutDescription = $"A Dead Body has turned up in the {calloutLocation} area.";
            StartDistance = 165f;
            ResponseCode = 1;
        }

        public override async Task OnAccept()
        {
            InitBlip();
            UpdateData();

            await Task.FromResult(0);
        }

        public override async void OnStart(Ped closest)
        {
            base.OnStart(closest);
            
            witness = await SpawnPed(RandomUtils.GetRandomPed(), calloutLocation + 2f);
            witness.AttachBlip();
            witness.AttachedBlip.Color = BlipColor.Red;
            witness.AlwaysKeepTask = true;
            witness.BlockPermanentEvents = true;
            
            victim = await SpawnPed(RandomUtils.GetRandomPed(), calloutLocation);
            victim.Kill();
            victim.AttachBlip();
            victim.AttachedBlip.Color = BlipColor.Blue;

            await Task.FromResult(0);
            
            VictimQuestions();
            
        }
        
        public override void OnCancelBefore()
        {
            base.OnCancelBefore();

            if(witness != null && witness.Exists() && !witness.IsCuffed)
            {
                witness.Task.WanderAround();
            }

            if (victim != null && victim.Exists() && !victim.IsCuffed)
            {
                victim.Task.WanderAround();
            }
        }

        public void VictimQuestions()
        {
            PedQuestion q1 = new PedQuestion();
            q1.Question = "Are you the one who called about the dead body?";
            q1.Answers = new List<string>(){
                "~y~* timidly remains silent*~s~",
                "~y~* stares at the ground, proceeds to shed a tear*~s~",
                "We got into an argument.",
                "Nothing officer."
            };
            PedQuestion[] questions = new PedQuestion[] { q1 };
            AddPedQuestions(witness, questions);
        }
    }
}
