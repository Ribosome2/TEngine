using Cysharp.Threading.Tasks;
using UnityEngine;
using TEngine;
using UnityEngine.UI;

namespace GameLogic
{
	[UI("Assets/AssetRaw/UI/UIMainCity.prefab")]
	public class UIMainCity: UIWindowBase
	{
		#region 节点定义

		protected GameObject m_btn_Zombie {  get	{   return RefBind.GetGO("m_btn_Zombie");	}}
        #endregion 节点定义

        protected override void OnBindUIEvent()
        {
            m_btn_Zombie.GetComponent<Button>().onClick.AddListener(OnClickZombie);
        }

        private async void  OnClickZombie()
        {
	        await GameModule.Scene.LoadScene("ZombieGame").ToUniTask();
        }
	}
}
