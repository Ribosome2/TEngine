using System.Threading.Tasks;
using Framework;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class LockLaserRenderer:GameComponent
    {
        private GameObject viewGo;
        private bool isLoadingView;
        private int renderIndex = -1;
        private LineRenderer mLineRenderer;
        private Transform hitFlashTran;
        public override void OnUpdate()
        {
            base.OnUpdate();
            var laserComp = mOwner.GetComponent<LockLaserComponent>();
            if (laserComp.isShooting)
            {
                if (viewGo == null)
                {
                    if (!isLoadingView)
                    {
                        isLoadingView = true;
                        InitView(laserComp);
                    }
                }
                else
                {
                    UpdateLaser(laserComp);
                }
            }
            else
            {
                if (viewGo)
                {
                    UpdateLaser(laserComp);
                }
            }
        }

        async Task InitView(LockLaserComponent laserComp)
        {
            viewGo= await GameModule.Resource.LoadGameObjectAsync("Assets/AssetRaw/ZombieGameDemo/Prefabs/LockLaser.prefab");
            mLineRenderer = viewGo.GetComponent<LineRenderer>();
            hitFlashTran = viewGo.transform.Find("HitFlash");
            UpdateLaser(laserComp);
        }

        void UpdateLaser(LockLaserComponent laserComp)
        {
            if (mLineRenderer.gameObject.activeSelf != laserComp.isShooting)
            {
                mLineRenderer.gameObject.SetActive( laserComp.isShooting);
            }

            if (laserComp.isShooting)
            {
                if (renderIndex != laserComp.ShootIndex)
                {
                    renderIndex = laserComp.ShootIndex;
                    mLineRenderer.SetPosition(0,laserComp.laserStartPoint);
                    mLineRenderer.SetPosition(1,laserComp.LaserEndPos);
                    viewGo.transform.position = laserComp.laserStartPoint;
                    viewGo.transform.forward = laserComp.ShootDirection;
                    hitFlashTran.position = laserComp.LaserEndPos;
                }
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
        }
    }
}