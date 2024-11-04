using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
	[UI("Assets/PatchGameRes/UI/ZombieBattle/PnlZombieTestSetting.prefab")]
	public class PnlZombieTestSetting: UIWindowBase
	{
		#region 节点定义

		protected GameObject m_btnSetInterval {  get	{   return RefBind.GetGO("m_btnSetInterval");	}}
		protected GameObject m_btnClearMonster {  get	{   return RefBind.GetGO("m_btnClearMonster");	}}
		protected GameObject m_btnClose {  get	{   return RefBind.GetGO("m_btnClose");	}}
		protected GameObject m_inputInterval {  get	{   return RefBind.GetGO("m_inputInterval");	}}
        #endregion 节点定义

        protected override void OnBindUIEvent()
        {
            m_btnClose.GetComponent<Button>().onClick.AddListener(CloseThisWindow);
            m_btnSetInterval.GetComponent<Button>().onClick.AddListener(OnClickSetInterval);
            m_btnClearMonster.GetComponent<Button>().onClick.AddListener(OnClickClearMonster);
        }

        private void OnClickClearMonster()
        {
	        var monsterUnits = ZombieBattleMgr.Instance.GetAllMonsterUnits();
	        foreach (var monsterUnit in monsterUnits)
	        {
		        monsterUnit.SetAsToDelete();
	        }
        }

        private void OnClickSetInterval()
        {
	        ZombieBattleMgr.Instance.SetSpawnInterval(float.Parse(m_inputInterval.GetComponent<InputField>().text));
        }
	}
}
