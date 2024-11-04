using System.Threading.Tasks;
using Framework;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class LaserRenderer:GameComponent
    {
        private GameObject viewGo;
        private bool isLoadingView;
        private int renderIndex = -1;
        private LineRenderer mLineRenderer;
        public override void OnUpdate()
        {
            base.OnUpdate();
            var laserComp = mOwner.GetComponent<DirectionalLaserComponent>();
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

        private async Task InitView(DirectionalLaserComponent laserComp)
        {
            viewGo = await GameModule.Resource.LoadGameObjectAsync("Assets/AssetRaw/ZombieGameDemo/Prefabs/LaserNoControl.prefab");
            mLineRenderer = viewGo.GetComponent<LineRenderer>();
            UpdateLaser(laserComp);
        }

        void UpdateLaser(DirectionalLaserComponent laserComp)
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