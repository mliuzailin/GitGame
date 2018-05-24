//----------------------------------------------
// MVVM 4 uGUI
// © 2015 yedo-factory
//----------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace M4u
{
    /// <summary>
    /// M4uImageBinding. Bind Image
    /// </summary>
	[AddComponentMenu("M4u/ImageBinding")]
    public class M4uImageBinding : M4uBindingSingle
    {
        private Image ui = null;
        private DynamicImageMaterialAssigner assigner = null;
        public bool SetNativeSize = false;

        public override void Start()
        {
            base.Start();

            ui = GetComponent<Image>();
            assigner = GetComponent<DynamicImageMaterialAssigner>();
            // 2018.01.10 Developer追記
            // 下記のOnChange内でui.spriteを変更しているが,
            // 「Values[0] as Sprite」がNULLになる場合、既に設定されているSpriteをNULLにしてしまうため,
            // プレハブ上でImageコンポーネントにSpriteと独自Materialが設定されている場合に異常な表示になる不具合が発生していた.
            // 対処方法として,プレハブに設定されていたSpriteを再度代入するようにしているが,今後同様の不具合が発生する可能性がある.
            // 不具合該当チケット: DG0-4217
            // コミット: 3aa7c89ba9f0b4aafaaae1a1fc75c8c8eae48180 [3aa7c89]
            OnChange();
        }

        public override void OnChange()
        {
            base.OnChange();

            ui.sprite = Values[0] as Sprite;
            //ネイティブサイズに変更
            if (SetNativeSize) ui.SetNativeSize();
            if (assigner != null)
            {

                assigner.Assined = false;
            }

        }

        public override string ToString()
        {
            return "Image.sprite=" + GetBindStr(Path);
        }
    }
}
