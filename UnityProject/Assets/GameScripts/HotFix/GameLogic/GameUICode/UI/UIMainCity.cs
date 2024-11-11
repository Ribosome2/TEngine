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
		protected GameObject m_btn_Test {  get	{   return RefBind.GetGO("m_btn_Test");	}}
        #endregion 节点定义

        protected override void OnBindUIEvent()
        {
            m_btn_Zombie.GetComponent<Button>().onClick.AddListener(OnClickZombie);
            m_btn_Test.GetComponent<Button>().onClick.AddListener(OnClickTest);
        }

        private void OnClickTest()
        {
	        // cfg.demo.Reward reward = tables.TbReward.Get(1001);
	        var configItem = ItemConfigMgr.Instance.GetItemConfig(10002);
	        Debug.Log($"reward:{configItem.Desc}");
	        var kyleTest = ConfigSystem.Instance.Tables.TbKyleItem.DataList;
	        Debug.Log($"reward:{configItem.Desc}");
	        foreach (var kyleTestItem in kyleTest)
	        {
				Debug.Log($"kyleTestItem :{kyleTestItem.Desc}");
	        }
        }

        private async void  OnClickZombie()
        {
	        await GameModule.Scene.LoadScene("ZombieGame").ToUniTask();
        }
	}
}
