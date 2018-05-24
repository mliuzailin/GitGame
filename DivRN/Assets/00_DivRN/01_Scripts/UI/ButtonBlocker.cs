using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

public class ButtonBlocker
{
    private static ButtonBlocker m_instance = null;
    public static ButtonBlocker Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new ButtonBlocker();

            return m_instance;
        }
    }
    private ButtonBlocker() { }

    private Dictionary<string, bool> m_tagedBlock = new Dictionary<string, bool>();

    public void TearDown()
    {
        m_instance = null;
    }

    public void Block(string tag = "no_tag")
    {
        bool cannotBlock = false;
        lock (((ICollection)m_tagedBlock).SyncRoot)
            cannotBlock = m_tagedBlock.ContainsKey(tag)
                        && m_tagedBlock[tag];
        if (cannotBlock)
            return;

        Thread watcher = null;
        if (!IsActive())
            watcher = new Thread(
                () =>
                {
                    try
                    {
                        int elaspedTimeMilSec = 0;
                        int timeOutMilSec = 60 * 1000;
                        int deltaMilSec = 16;
                        while (IsActive())
                        {
                            Debug.Assert(elaspedTimeMilSec < timeOutMilSec, "ButtonBlocker remains blocking too long.");
                            Thread.Sleep(deltaMilSec);
                            elaspedTimeMilSec += deltaMilSec;

                            if (elaspedTimeMilSec > timeOutMilSec)
                                ForceUnblock();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("ERROR:" + e.Message);
                    }
                });

        lock (((ICollection)m_tagedBlock).SyncRoot)
            m_tagedBlock[tag] = true;

        if (watcher != null)
            watcher.Start();
    }

    public void Unblock(string tag = "no_tag")
    {
        lock (((ICollection)m_tagedBlock).SyncRoot)
            Debug.Assert(m_tagedBlock.ContainsKey(tag), "The tag tried to unblock was not exists.");

        lock (((ICollection)m_tagedBlock).SyncRoot)
            m_tagedBlock[tag] = false;
    }


    public bool IsActive()
    {
        bool ret = false;

        lock (((ICollection)m_tagedBlock).SyncRoot)
        {
            foreach (var pair in m_tagedBlock)
            {
                if (pair.Value)
                {
                    ret = true;
                    break;
                }
            }
        }

        if (!ret)
            lock (((ICollection)m_tagedBlock).SyncRoot)
                m_tagedBlock.Clear();

        return ret;
    }

    public bool IsActive(string tag)
    {
        bool ret = false;

        lock (((ICollection)m_tagedBlock).SyncRoot)
            ret = m_tagedBlock.ContainsKey(tag)
                && m_tagedBlock[tag];

        return ret;
    }


    private void ForceUnblock()
    {
        lock (((ICollection)m_tagedBlock).SyncRoot)
            m_tagedBlock.Clear();
    }
}
