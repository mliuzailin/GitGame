//----------------------------------------------
// MVVM 4 uGUI
// © 2015 yedo-factory
//----------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace M4u
{
    /// <summary>
    /// M4uProperty. Data Binding to View
    /// </summary>
    /// <typeparam name="T">type</typeparam>
	public class M4uProperty<T> : M4uPropertyBase
	{
		private T value;

		public T Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;

                // ViewModel->View
                for (int i = Bindings.Count - 1; i >= 0; i--)
                {
                    M4uBinding binding = Bindings[i];
                    if (binding == null)
                    {
                        Bindings.RemoveAt(i);
                    }
                    else
                    {
                        binding.OnChange();
                    }
                }
			}
		}

		/// <summary>
		/// 値に変化があった時だけ代入が実行されるバージョン（処理負荷軽減目的）
		/// 値が変わっていないのに毎フレーム代入している所が多かったので追加(値が変わってなくても代入しただけでNGUI関連の処理が動作して重くなる)
		/// </summary>
		public T Value2
		{
			get
			{
				return value;
			}
			set
			{
				if (isSame(value))
				{
					return;
				}

				Value = value;
			}
		}
		private bool isSame(T value)
		{
			if (this.value == null)
			{
				if (value == null)
				{
					return true;
				}
			}
			else if (this.value.Equals(value))
			{
				return true;
			}

			return false;
		}

        public M4uProperty(T value) { Value = value; }
        public M4uProperty() { }
	}
}