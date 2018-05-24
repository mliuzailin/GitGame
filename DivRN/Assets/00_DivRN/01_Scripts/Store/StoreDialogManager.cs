/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	StoreDialogManager.cs
	@brief	ストアダイアログ
*/
/*==========================================================================*/
/*==========================================================================*/

public class StoreDialogManager : SingletonComponent<StoreDialogManager>
{
    public enum STORE_DIALOG_STATUS
    {
        WAIT,
        BUY_STONE,
        STAMINA_RECOVERY,
        FRIEND_EXT,
        UNIT_EXT,
    }

    private STORE_DIALOG_STATUS m_Status = STORE_DIALOG_STATUS.WAIT;

    /// チップ購入
    private StoreBuyTip m_StoreBuyTip = new StoreBuyTip();
    /// スタミナ回復ルーチン
    private StoreUseTipStamina m_UseTipStamina = new StoreUseTipStamina();
    /// フレンド枠拡張ルーチン
    private StoreUseTipFriendExt m_UseTipFriendExt = new StoreUseTipFriendExt();
    /// 所持ユニット枠拡張ルーチン
    private StoreUseTipUnitExt m_UseTipUnitExt = new StoreUseTipUnitExt();

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        switch (m_Status)
        {
            case STORE_DIALOG_STATUS.WAIT:
            case STORE_DIALOG_STATUS.BUY_STONE:
                break;
            case STORE_DIALOG_STATUS.STAMINA_RECOVERY:
                if (m_UseTipStamina.UpdateProcess() != true)
                {
                    m_Status = STORE_DIALOG_STATUS.WAIT;
                    //チップ購入へ遷移する？
                    if (m_UseTipStamina.IsNextBuyTip())
                    {
                        OpenBuyStone();
                    } 
                }
                break;
            case STORE_DIALOG_STATUS.FRIEND_EXT:
                if (m_UseTipFriendExt.UpdateProcess() != true)
                {
                    m_Status = STORE_DIALOG_STATUS.WAIT;
                    //チップ購入へ遷移する？
                    if (m_UseTipFriendExt.IsNextBuyTip())
                    {
                        OpenBuyStone();
                    }
                }
                break;
            case STORE_DIALOG_STATUS.UNIT_EXT:
                if (m_UseTipUnitExt.UpdateProcess() != true)
                {
                    m_Status = STORE_DIALOG_STATUS.WAIT;
                    //チップ購入へ遷移する？
                    if (m_UseTipUnitExt.IsNextBuyTip())
                    {
                        OpenBuyStone();
                    }
                }
                break;
        }
    }

    public void OpenBuyStone()
    {
        if (m_Status != STORE_DIALOG_STATUS.WAIT){
             return;
        }

        m_StoreBuyTip.StartProcess(null, (bool isComplete) =>
        {
            if(m_Status == STORE_DIALOG_STATUS.BUY_STONE)
            {
                m_Status = STORE_DIALOG_STATUS.WAIT;
            }
        });
        m_Status = STORE_DIALOG_STATUS.BUY_STONE;
    }

    public void OpenDialogUnitExtend()
    {
        if (m_Status != STORE_DIALOG_STATUS.WAIT)
        {
             return;
        }

        m_UseTipUnitExt.StartProcess(null);
        // ページステップも変える
        m_Status = STORE_DIALOG_STATUS.UNIT_EXT;
    }

    public void OpenDialogFriendExtend()
    {
        if (m_Status != STORE_DIALOG_STATUS.WAIT)
        {
             return;
        }

        m_UseTipFriendExt.SetFriendList(null);
        if (MainMenuManager.HasInstance)
        {
            if( MainMenuManager.Instance.WorkSwitchPageNow == MAINMENU_SEQ.SEQ_FRIEND_LIST)
            {
                MainMenuFriendsList friendList = MainMenuManager.Instance.MainMenuSeqPageNow as MainMenuFriendsList;
                m_UseTipFriendExt.SetFriendList(friendList);
            }
        }

        m_UseTipFriendExt.StartProcess(null);
        // ページステップも変える
        m_Status = STORE_DIALOG_STATUS.FRIEND_EXT;
    }

    public void OpenDialogStaminaRecovery()
    {
        if (m_Status != STORE_DIALOG_STATUS.WAIT)
        {
             return;
        }

        m_UseTipStamina.StartProcess(null);
        // ページステップも変える
        m_Status = STORE_DIALOG_STATUS.STAMINA_RECOVERY;
    }

	// ユーザー追い出しした時、m_Statusが初期化され無い為
	public void ResetStatus()
	{
		switch (m_Status)
		{
			case STORE_DIALOG_STATUS.WAIT:
			case STORE_DIALOG_STATUS.BUY_STONE:
				break;
			case STORE_DIALOG_STATUS.STAMINA_RECOVERY:
				m_UseTipStamina.ResetBuyStep();
				break;
			case STORE_DIALOG_STATUS.FRIEND_EXT:
				m_UseTipFriendExt.ResetBuyStep();
				break;
			case STORE_DIALOG_STATUS.UNIT_EXT:
				m_UseTipUnitExt.ResetBuyStep();
				break;
		}
		m_Status = STORE_DIALOG_STATUS.WAIT;
	}
}
