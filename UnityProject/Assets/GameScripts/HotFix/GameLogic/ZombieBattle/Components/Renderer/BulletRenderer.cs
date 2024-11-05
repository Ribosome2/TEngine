using System.Threading.Tasks;
using Framework;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class BulletRenderer : GameComponent
    {
        private GameObject viewGo;
        private Transform viewTrans;

        public override void OnInit()
        {
            base.OnInit();
            InitView();
        }

        private async Task InitView()
        {
            viewGo = await GameModule.Resource.LoadGameObjectAsync("Assets/PatchGameRes/UI/ZombieBattle/bullet.prefab");
            viewTrans = viewGo.transform;
            var directionalBullet = GetComponent<DirectionalBulletComponent>();
            viewTrans.forward = directionalBullet.MoveDir;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (viewTrans)
            {
                var directionalBullet = GetComponent<DirectionalBulletComponent>();
                viewTrans.position = directionalBullet.Pos;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (viewGo)
            {
                GameObject.Destroy(viewGo);
            }

            viewGo = null;
            viewTrans = null;
        }
    }
}