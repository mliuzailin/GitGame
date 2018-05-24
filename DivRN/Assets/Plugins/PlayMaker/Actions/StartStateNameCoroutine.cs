// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
#endif

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Start a Coroutine in a Behaviour on a Game Object. See Unity StartCoroutine docs.")]
    public class StartStateNameCoroutine : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The game object that owns the Behaviour.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Behaviour)]
        [Tooltip("The Behaviour that contains the method to start as a coroutine.")]
        public FsmString behaviour;

        [Tooltip("Stop the coroutine when the state is exited.")]
        public bool stopOnExit;

        public override void Reset()
        {
            gameObject = null;
            behaviour = null;
            stopOnExit = false;
        }

        MonoBehaviour component;

#if UNITY_EDITOR

        private Type cachedType;
        private List<string> methodNames;

#endif

        public override void OnEnter()
        {
            DoStartStateNameCoroutine();

            Finish();
        }

        private string FunctionName
        {
            get
            {
                string n = State.Name;
                if (n.Contains(" "))
                {
                    n = n.Split(' ')[0];
                }
                return "On" + n;
            }
        }

        void DoStartStateNameCoroutine()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null)
            {
                return;
            }

            component = go.GetComponent(ReflectionUtils.GetGlobalType(behaviour.Value)) as MonoBehaviour;

            if (component == null)
            {
                LogWarning("StartStateNameCoroutine: " + go.name + " missing behaviour: " + behaviour.Value);
                return;
            }

            component.StartCoroutine(FunctionName);
        }

        public override void OnExit()
        {
            if (component == null)
            {
                return;
            }

            if (stopOnExit)
            {
                component.StopCoroutine(FunctionName);
            }
        }

#if UNITY_EDITOR


        public override string ErrorCheck()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null || string.IsNullOrEmpty(behaviour.Value))
            {
                return string.Empty;
            }

            var type = ReflectionUtils.GetGlobalType(behaviour.Value);
            if (type == null)
            {
                return "Missing Behaviour: " + behaviour.Value;
            }

            if (cachedType != type)
            {
                cachedType = type;
                methodNames = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Select(m => m.Name).ToList();
            }

            if (!string.IsNullOrEmpty(FunctionName))
            {
                if (!methodNames.Contains(FunctionName))
                {
                    return "Missing Method: " + FunctionName;
                }
            }
            return string.Empty;
        }

#endif

    }
}
