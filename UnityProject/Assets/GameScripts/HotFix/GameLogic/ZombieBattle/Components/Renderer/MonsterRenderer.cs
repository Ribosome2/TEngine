using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Framework;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class MonsterRenderer : GameComponent
    {
        private GameObject viewGo;
        private Transform viewTrans;
        private SpriteRenderer mSpriteRender;
        public bool isInHitState;
        public override void OnInit()
        {
            base.OnInit();
          
            InitView();
        }

        private async Task InitView()
        {
            viewGo=await GameModule.Resource.LoadGameObjectAsync("Assets/PatchGameRes/Prefabs/Zombie1.prefab");
            viewTrans = viewGo.transform;
            mSpriteRender = viewGo.GetComponentInChildren<SpriteRenderer>();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (viewGo)
            {
                GameObject.Destroy(viewGo);
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (viewTrans)
            {
                var movementComp = GetComponent<MonsterMoveComponent>();
                viewTrans.position = movementComp.Pos;
                var monsterUnit = mOwner as MonsterUnit;
                if (monsterUnit.IsHitDirty)
                {
                    monsterUnit.IsHitDirty = false;
                    if (isInHitState == false)
                    {
                        mSpriteRender.color = new Color(0.9f, 0.3f, 0.3f);
                        isInHitState = true;
                    }
                }

                if (isInHitState)
                {
                    if (Time.realtimeSinceStartup - monsterUnit.lastHitTime > 0.5f)
                    {
                        isInHitState = false;
                        mSpriteRender.color = new Color(1, 1f, 1f);
                    }
                }

            }
        }
    }
}