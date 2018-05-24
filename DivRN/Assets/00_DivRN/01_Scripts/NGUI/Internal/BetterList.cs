//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

//#define DEF_NGUI_GC_RENEW		//!< ガベージコレクション改善
//#define DEF_NGUI_GC_RENEW_2		//!< ガベージコレクション改善

//----------------------------------------------------------------
//	DEF_NGUI_GC_RENEW
//  ※ガベージコレクション対応の基本方針は以下
//
//	・配列の再構築は可能な限り行わない
//	・配列は拡大はするが縮小はしない
//	・入力情報を入力する際に配列領域に余りが生じた場合、終端情報をコピーして頂点情報を潰すことで対応
//		※Vector3のコピー等が頻発するので、頂点座標のみを例外対応で潰す。法線とかカラーとかにゴミが入るけど気にしない
//	・クラス参照を内包するリストの場合、コピー時や余剰埋めの際に不具合が出るのでオリジナルの配列クラス（BetterListOrigin）を使用する
//	
//----------------------------------------------------------------



using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This improved version of the System.Collections.Generic.List that doesn't release the buffer on Clear(), resulting in better performance and less garbage collection.
/// </summary>

public class BetterList<T>
{
#if UNITY_FLASH

	List<T> mList = new List<T>();
	
	/// <summary>
	/// Direct access to the buffer. Note that you should not use its 'Length' parameter, but instead use BetterList.size.
	/// </summary>
	
	public T this[int i]
	{
		get { return mList[i]; }
		set { mList[i] = value; }
	}
	
	/// <summary>
	/// Compatibility with the non-flash syntax.
	/// </summary>
	
	public List<T> buffer { get { return mList; } }

	/// <summary>
	/// Direct access to the buffer's size. Note that it's only public for speed and efficiency. You shouldn't modify it.
	/// </summary>

	public int size { get { return mList.Count; } }

	/// <summary>
	/// For 'foreach' functionality.
	/// </summary>

	public IEnumerator<T> GetEnumerator () { return mList.GetEnumerator(); }

	/// <summary>
	/// Clear the array by resetting its size to zero. Note that the memory is not actually released.
	/// </summary>

	public void Clear () { mList.Clear(); }

	/// <summary>
	/// Clear the array and release the used memory.
	/// </summary>

	public void Release () { mList.Clear(); }

	/// <summary>
	/// Add the specified item to the end of the list.
	/// </summary>

	public void Add (T item) { mList.Add(item); }

	/// <summary>
	/// Insert an item at the specified index, pushing the entries back.
	/// </summary>

	public void Insert (int index, T item) { mList.Insert(index, item); }

	/// <summary>
	/// Returns 'true' if the specified item is within the list.
	/// </summary>

	public bool Contains (T item) { return mList.Contains(item); }

	/// <summary>
	/// Remove the specified item from the list. Note that RemoveAt() is faster and is advisable if you already know the index.
	/// </summary>

	public bool Remove (T item) { return mList.Remove(item); }

	/// <summary>
	/// Remove an item at the specified index.
	/// </summary>

	public void RemoveAt (int index) { mList.RemoveAt(index); }

	/// <summary>
	/// Mimic List's ToArray() functionality, except that in this case the list is resized to match the current size.
	/// </summary>

	public T[] ToArray () { return mList.ToArray(); }

	/// <summary>
	/// List.Sort equivalent.
	/// </summary>

	public void Sort (System.Comparison<T> comparer) { mList.Sort(comparer); }

#else

    /// <summary>
    /// Direct access to the buffer. Note that you should not use its 'Length' parameter, but instead use BetterList.size.
    /// </summary>

    public T[] buffer;

    /// <summary>
    /// Direct access to the buffer's size. Note that it's only public for speed and efficiency. You shouldn't modify it.
    /// </summary>

    public int size = 0;

#if DEF_NGUI_GC_RENEW
	// 情報の終端をコピーで埋める処理の有無
	public	bool	bufferFill = false;
	public	T[]		bufferTrim;
#endif


    /// <summary>
    /// For 'foreach' functionality.
    /// </summary>

    public IEnumerator<T> GetEnumerator()
    {
        if (buffer != null)
        {
            for (int i = 0; i < size; ++i)
            {
                yield return buffer[i];
            }
        }
    }

    /// <summary>
    /// Convenience function. I recommend using .buffer instead.
    /// </summary>

    public T this[int i]
    {
        get { return buffer[i]; }
        set { buffer[i] = value; }
    }

#if DEF_NGUI_GC_RENEW
	//----------------------------------------------------------------------------
	/*!
		@brief	固定サイズのバッファを確保する
		@note	
	*/
	//----------------------------------------------------------------------------
	public void AllocateFixBuffer ( int nSize )
	{
		buffer = new T[ nSize ];
		size = 0;
	}
#endif

    /// <summary>
    /// Helper function that expands the size of the array, maintaining the content.
    /// </summary>

    void AllocateMore()
    {
        T[] newList = (buffer != null) ? new T[Mathf.Max(buffer.Length << 1, 32)] : new T[32];
        if (buffer != null && size > 0)
        {
            buffer.CopyTo(newList, 0);
            //			Debug.LogError( "aaaaaa" + buffer.Length +" -> " + newList.Length );
        }

        buffer = newList;
    }

    public void Resize(int nNewSize)
    {
        bool bHasBuffer = (null != buffer);
        if (bHasBuffer)
        {
            if (nNewSize == buffer.Length)
            {
                return;
            }
        }


        T[] acNewList = new T[nNewSize];

        if (bHasBuffer && size > 0)
        {
            size = Mathf.Min(size, nNewSize);
            System.Array.Copy(buffer, acNewList, size);
        }

        buffer = acNewList;
    }



#if DEF_NGUI_GC_RENEW_2
	// 指定のサイズがバッファに乗るかチェック。
	// 乗らない場合にはバッファを拡張する
	public bool AllocateAndChk ( int nAddChk )
	{
		if( buffer == null )
		{
			int nAllocateSize = Mathf.Max( nAddChk , 32 );
			buffer = new T[ nAllocateSize ];
			return true;
		}
		if( buffer == null
		||	buffer.Length < size + nAddChk
		)
		{
			int nAllocateSize = Mathf.Max( (buffer.Length + nAddChk) , ( buffer.Length << 1 ) );
			T[] newList = new T[ nAllocateSize ];
				
			if( size > 0 )
			{
				buffer.CopyTo(newList, 0);
			}
			buffer = newList;
			return true;
		}
		return false;
	}
#endif

#if DEF_NGUI_GC_RENEW
	//----------------------------------------------------------------------------
	/*!
		@brief	もともとはバッファを詰めて配列サイズを正規化して返す関数。
		@note	もともとはリサイズのためのNewDeleteが走ってた。
				ガベージコレクションの改善として領域確保を行わずに領域を使いまわす用に対応
				
				最後の情報を終端までコピーすることで頂点情報を潰してる。
	*/
	//----------------------------------------------------------------------------
	void TrimRecycle ()
	{
		if( size <= 0 )
			return;
		if( buffer == null )
			return;
		if( bufferFill == false )
			return;
		
		T temp = buffer[size-1];
		for( int i = size; i < buffer.Length; i++)
		{
			buffer[i] = temp;
		}
	}
#else
    /// <summary>
    /// Trim the unnecessary memory, resizing the buffer to be of 'Length' size.
    /// Call this function only if you are sure that the buffer won't need to resize anytime soon.
    /// </summary>

    void Trim()
    {
        if (size > 0)
        {
            if (size < buffer.Length)
            {
                T[] newList = new T[size];
                for (int i = 0; i < size; ++i) newList[i] = buffer[i];
                buffer = newList;
            }
        }
        else buffer = null;
    }

#endif

    /// <summary>
    /// Clear the array by resetting its size to zero. Note that the memory is not actually released.
    /// </summary>

    public void Clear() { size = 0; }

    /// <summary>
    /// Clear the array and release the used memory.
    /// </summary>

    public void Release() { size = 0; buffer = null; }

    /// <summary>
    /// Add the specified item to the end of the list.
    /// </summary>

    public void Add(T item)
    {
        if (buffer == null || size == buffer.Length) AllocateMore();
        buffer[size++] = item;
    }
    public void AddRef(ref T item)
    {
        if (buffer == null || size == buffer.Length) AllocateMore();
        buffer[size++] = item;
    }
    public void Add(ref BetterList<T> lItem)
    {
        if (null == buffer)
        {
            Resize(lItem.size);
        }
        else
        {
            int nSpace = buffer.Length - size;
            if (nSpace < lItem.size)
            {
                Resize(buffer.Length + lItem.size - nSpace);
            }
        }

        System.Array.Copy(lItem.buffer, 0, buffer, size, lItem.size);
        size += lItem.size;
    }

    /// <summary>
    /// Insert an item at the specified index, pushing the entries back.
    /// </summary>

    public void Insert(int index, T item)
    {
        if (buffer == null || size == buffer.Length) AllocateMore();

        if (index < size)
        {
            for (int i = size; i > index; --i) buffer[i] = buffer[i - 1];
            buffer[index] = item;
            ++size;
        }
        else Add(item);
    }

    /// <summary>
    /// Returns 'true' if the specified item is within the list.
    /// </summary>

    public bool Contains(T item)
    {
        if (buffer == null) return false;
        for (int i = 0; i < size; ++i) if (buffer[i].Equals(item)) return true;
        return false;
    }

    /// <summary>
    /// Remove the specified item from the list. Note that RemoveAt() is faster and is advisable if you already know the index.
    /// </summary>

    public bool Remove(T item)
    {
        if (buffer != null)
        {
            EqualityComparer<T> comp = EqualityComparer<T>.Default;

            for (int i = 0; i < size; ++i)
            {
                if (comp.Equals(buffer[i], item))
                {
                    --size;
                    buffer[i] = default(T);
                    for (int b = i; b < size; ++b) buffer[b] = buffer[b + 1];
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Remove an item at the specified index.
    /// </summary>

    public void RemoveAt(int index)
    {
        if (buffer != null && index < size)
        {
            --size;
            buffer[index] = default(T);
            for (int b = index; b < size; ++b) buffer[b] = buffer[b + 1];
        }
    }

#if DEF_NGUI_GC_RENEW
	//----------------------------------------------------------------------------
	/*!
		@brief	バッファを詰めて配列サイズを正規化して返す関数。
		@note	もともとはリサイズのためのNewDeleteが走ってた。
				ガベージコレクションの改善として領域確保を行わずに領域を使いまわす用に対応
	*/
	//----------------------------------------------------------------------------
	public T[] ToArray ()
	{
		if( size > 0 )
		{
			TrimRecycle();
			return buffer;
		}
		else
		{
			return null;
		}
	}

	//----------------------------------------------------------------------------
	/*!
		@brief	バッファを詰めて配列サイズを正規化して返す関数。
		@note	もともとはリサイズのためのNewDeleteが走ってた。
				ガベージコレクションの改善として領域確保を行わずに領域を使いまわす用に対応
	*/
	//----------------------------------------------------------------------------
	public T[] ToArrayTrim ()
	{
		if( size == 0 )
			return null;
		if( buffer == null )
			return null;
		if( buffer.Length == size )
		{
			return buffer;
		}
		
#if true
		bufferTrim = new T[size];
#else
		if( bufferTrim == null )
		{
//			Debug.LogError( "aaaaaaaaaaaaaaaaaa" );
			bufferTrim = new T[size];
		}
		else
//		if( bufferTrim.Length == size )
		if( bufferTrim.Length >= size 
		&&	bufferTrim.Length <= size + 100
		)
		{

		}
		else
		{
//			Debug.LogError( "bbbbbbbbbbbbb " + bufferTrim.Length + " -> " + size );
			bufferTrim = new T[size];
		}
#endif

		for (int i = 0; i < size; ++i)
		{
			bufferTrim[i] = buffer[i];
		}
		for (int i = size; i < bufferTrim.Length; ++i)
		{
			bufferTrim[i] = buffer[0];
		}
		return bufferTrim;
	}
#else
    /// <summary>
    /// Mimic List's ToArray() functionality, except that in this case the list is resized to match the current size.
    /// </summary>

    public T[] ToArray() { Trim(); return buffer; }

    public void ToArrayNoTrim(ref T[] atRet)
    {
        if (size > 0)
        {
            if (null == atRet || atRet.Length != size)
            {
                atRet = new T[size];
            }

            for (int i = 0; i < size; ++i)
            {
                atRet[i] = buffer[i];
            }
        }
        else buffer = null;
    }
#endif

    public int GetLength()
    {
        if (buffer == null)
        {
            return 0;
        }
        else
        {
            return buffer.Length;
        }
    }

    /// <summary>
    /// List.Sort equivalent.
    /// </summary>

    private class Comparer : System.Collections.IComparer
    {
        System.Comparison<T> mCompare;


        public Comparer(System.Comparison<T> comparer)
        {
            mCompare = comparer;
        }

        public int Compare(object x, object y)
        {
            return mCompare((T)x, (T)y);
        }
    }

    /// <summary>
    /// List.Sort equivalent.
    /// </summary>

    public void Sort(System.Comparison<T> comparer)
    {
        if (buffer == null)
        {
            return;
        }

        System.Array.Sort(buffer, 0, size, new Comparer(comparer));
    }

    /*
        public void Sort (System.Comparison<T> comparer)
        {
            bool changed = true;

            while (changed)
            {
                changed = false;

                for (int i = 1; i < size; ++i)
                {
                    if (comparer.Invoke(buffer[i - 1], buffer[i]) > 0)
                    {
                        T temp = buffer[i];
                        buffer[i] = buffer[i - 1];
                        buffer[i - 1] = temp;
                        changed = true;
                    }
                }
            }
        }
    */
#endif
}