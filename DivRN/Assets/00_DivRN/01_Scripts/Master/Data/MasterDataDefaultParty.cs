using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：パーティ関連：デフォルトパーティ
*/
//----------------------------------------------------------------------------
[Serializable]
public class MasterDataDefaultParty : Master
{
    //public uint fix_id{get;set;}                     //!< 情報固有固定ID

    public string party_name
    {
        get;
        set;
    } //!< パーティ名称

    public uint party_chara0_id
    {
        get;
        set;
    } //!< キャラ0：キャラID	※[ MasterDataParamChara.fix_id ]と一致

    public uint party_chara0_level
    {
        get;
        set;
    } //!< キャラ0：キャラレベル

    public uint party_chara1_id
    {
        get;
        set;
    } //!< キャラ1：キャラID	※[ MasterDataParamChara.fix_id ]と一致

    public uint party_chara1_level
    {
        get;
        set;
    } //!< キャラ1：キャラレベル

    public uint party_chara2_id
    {
        get;
        set;
    } //!< キャラ2：キャラID	※[ MasterDataParamChara.fix_id ]と一致

    public uint party_chara2_level
    {
        get;
        set;
    } //!< キャラ2：キャラレベル

    public uint party_chara3_id
    {
        get;
        set;
    } //!< キャラ3：キャラID	※[ MasterDataParamChara.fix_id ]と一致

    public uint party_chara3_level
    {
        get;
        set;
    } //!< キャラ3：キャラレベル

    public uint party_chara4_id
    {
        get;
        set;
    } //!< キャラ4：キャラID	※[ MasterDataParamChara.fix_id ]と一致

    public uint party_chara4_level
    {
        get;
        set;
    } //!< キャラ4：キャラレベル

    public Dictionary<uint, uint> PartyCharaDict
    {
        get
        {
            return new Dictionary<uint, uint>()
            {
                {
                    party_chara0_id, party_chara0_level
                },
                {
                    party_chara1_id, party_chara1_level
                },
                {
                    party_chara2_id, party_chara2_level
                },
                {
                    party_chara3_id, party_chara3_level
                },
                {
                    party_chara4_id, party_chara4_level
                },
            };
        }
    }
    public uint material_chara0_id
    {
        get;
        set;
    } //!< キャラ0：キャラID	※[ MasterDataParamChara.fix_id ]と一致

    public uint material_chara0_level
    {
        get;
        set;
    } //!< キャラ0：キャラレベル

    public uint material_chara1_id
    {
        get;
        set;
    } //!< キャラ1：キャラID	※[ MasterDataParamChara.fix_id ]と一致

    public uint material_chara1_level
    {
        get;
        set;
    } //!< キャラ1：キャラレベル

    public uint material_chara2_id
    {
        get;
        set;
    } //!< キャラ2：キャラID	※[ MasterDataParamChara.fix_id ]と一致

    public uint material_chara2_level
    {
        get;
        set;
    } //!< キャラ2：キャラレベル

    public uint material_chara3_id
    {
        get;
        set;
    } //!< キャラ3：キャラID	※[ MasterDataParamChara.fix_id ]と一致

    public uint material_chara3_level
    {
        get;
        set;
    } //!< キャラ3：キャラレベル

    public uint material_chara4_id
    {
        get;
        set;
    } //!< キャラ4：キャラID	※[ MasterDataParamChara.fix_id ]と一致

    public uint material_chara4_level
    {
        get;
        set;
    } //!< キャラ4：キャラレベル

    public Dictionary<uint, uint> MaterialCharaDict
    {
        get
        {
            return new Dictionary<uint, uint>()
            {
                {
                    material_chara0_id, material_chara0_level
                },
                {
                    material_chara1_id, material_chara1_level
                },
                {
                    material_chara2_id, material_chara2_level
                },
                {
                    material_chara3_id, material_chara3_level
                },
                {
                    material_chara4_id, material_chara4_level
                },
            };
        }
    }
};
