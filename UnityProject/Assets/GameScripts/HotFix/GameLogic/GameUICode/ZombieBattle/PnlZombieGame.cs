using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
	[UI("Assets/AssetRaw/ZombieGameDemo/UI/ZombieBattle/PnlZombieGame.prefab")]
	public class PnlZombieGame: UIWindowBase
	{
		#region 节点定义

		protected GameObject m_playerLevelSld {  get	{   return RefBind.GetGO("m_playerLevelSld");	}}
		protected GameObject m_txtPlayerLevel {  get	{   return RefBind.GetGO("m_txtPlayerLevel");	}}
		protected GameObject m_btnQuit {  get	{   return RefBind.GetGO("m_btnQuit");	}}
		protected GameObject m_btnSetting {  get	{   return RefBind.GetGO("m_btnSetting");	}}
		protected GameObject m_zombies {  get	{   return RefBind.GetGO("m_zombies");	}}
		protected GameObject m_Slider {  get	{   return RefBind.GetGO("m_Slider");	}}
		protected GameObject m_txtHp {  get	{   return RefBind.GetGO("m_txtHp");	}}
        #endregion 节点定义

        protected override void OnBindUIEvent()
        {
            m_btnQuit.GetComponent<Button>().onClick.AddListener(OnClickQuit);
            m_btnSetting.GetComponent<Button>().onClick.AddListener(OnClickSetting);
            
        }

        private void OnClickSetting()
        {
	        UIManager.Instance.OpenUI<PnlZombieTestSetting>();
        }

        private void OnClickQuit()
        {
	        // SceneMgr.Instance.GoToScene(GameSceneTypes.MainCity);
	        GameModule.Scene.LoadScene("SceneTest");
        }

        protected override void OnCreate()
        {
	        base.OnCreate();
	        EventCenter.Subscribe(GlobalEvent.ZombieEatHealthEvent,UpdateHp);
	        // UnityMessage.Instance.OnUpdateMessage += OnUpdate;
	        UpdateHp();
	        
        }

        private void OnUpdate(float arg1, float arg2)
        {
	        
        }


        protected override void OnDestroy()
        {
	        base.OnDestroy();
	        EventCenter.UnSubscribe(GlobalEvent.ZombieEatHealthEvent,UpdateHp);
        }
        
        private void UpdateHp()
        {
	        m_txtHp.GetComponent<Text>().text = ZombieBattleMgr.Instance.Hp.ToString();
        }
	}
}
