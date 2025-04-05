/*
Developed by TakNof
Copyright (c) 2023 TakNof

Thanks a lot for donwnloading this package, if you found it usefull please 
consider giving me a star in github or buying me a coffee!
https://buymeacoffee.com/taknof
*/

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Linq;

public class EasierAnimatiorTransitionCopier : EditorWindow{
    private AnimatorController animatorController;
    private string[] stateNames;
    private Dictionary<string, AnimatorState> stateDictionary = new();
    private Dictionary<string, AnimatorStateMachine> subStateMachineDictionary = new();
    private readonly List<AnimatorTransitionBase> selectedTransitions = new();

    private AnimatorState sourceState;
    private AnimatorState targetState;

    private AnimatorStateMachine sourceSubMachine;
    private AnimatorStateMachine targetSubMachine;

    private string sourceSpecialState;
    private string targetSpecialState;

    private string statusMessage;
    private MessageType statusMessageType;


    [MenuItem("TakNof Tools/Animatior Transition Copier")]
    public static void ShowWindow()
    {
        GetWindow<EasierAnimatiorTransitionCopier>("Copy Animator Transition");
    }

    private void OnGUI()
    {
        GUILayout.Label("Copy Animator Transition", EditorStyles.boldLabel);

        animatorController = (AnimatorController)EditorGUILayout.ObjectField("Animator Controller", animatorController, typeof(AnimatorController), false);

        if (animatorController != null)
        {
            LoadStatesAndSubStateMachines();

            if (stateNames.Length > 0)
            {
                if (GUILayout.Button("Copy Selected Transitions"))
                {
                    UpdateSelectedTransitions();
                }

                if (GUILayout.Button("Paste Transitions"))
                {
                    GetSourceAndTargetElements();
                    PasteTransitions();
                }
            }
            else
            {
                statusMessage = "No states found in Animator Controller";
                statusMessageType = MessageType.Warning;
            }
        }

        if (!string.IsNullOrEmpty(statusMessage)){
            EditorGUILayout.HelpBox(statusMessage, statusMessageType);
        }
    }
    
    private void UpdateSelectedTransitions()
    {
        selectedTransitions.Clear();

        foreach (var obj in Selection.objects)
        {
            if (obj is AnimatorStateTransition stateTransition)
            {
                selectedTransitions.Add(stateTransition);
            }
            else if (obj is AnimatorTransition subMachineTransition)
            {
                selectedTransitions.Add(subMachineTransition);
            }
        }

        statusMessage = $"Selected {selectedTransitions.Count} transitions.";
        statusMessageType = MessageType.Info;
    }

    private void PasteTransitions()
    {
        if (selectedTransitions == null || selectedTransitions.Count == 0)
        {
            statusMessage = "No transitions selected to paste.";
            statusMessageType = MessageType.Error;
            return;
        }

        if (sourceState == null && sourceSubMachine == null && sourceSpecialState == null)
        {
            statusMessage = "No source state or submachine selected.";
            statusMessageType = MessageType.Error;
            return;
        }

        if (targetState == null && targetSubMachine == null && targetSpecialState == null)
        {
            statusMessage = "No target state or submachine selected.";
            statusMessageType = MessageType.Error;
            return;
        }

        AnimatorStateMachine commonStateMachine = GetCommonStateMachine(sourceState, sourceSubMachine, sourceSpecialState, targetState, targetSubMachine, targetSpecialState);
        if (commonStateMachine == null)
        {
            statusMessage = "Source and target must be in the same state machine or submachine.";
            statusMessageType = MessageType.Error;
            return;
        }

        foreach (var selectedTransition in selectedTransitions)
        {
            if (sourceState != null)
            {
                if (targetState != null)
                {
                    AnimatorStateTransition newTransition = sourceState.AddTransition(targetState);
                    CopyTransitionProperties(selectedTransition, newTransition);
                }
                else if (targetSubMachine != null)
                {
                    AnimatorStateTransition newTransition = sourceState.AddTransition(targetSubMachine);
                    CopyTransitionProperties(selectedTransition, newTransition);
                }
                else if (targetSpecialState == "Exit")
                {
                    AnimatorStateTransition newTransition = sourceState.AddExitTransition();
                    CopyTransitionProperties(selectedTransition, newTransition);
                }
            }
            else if (sourceSubMachine != null)
            {
                if (targetState != null)
                {
                    AnimatorTransition newTransition = commonStateMachine.AddStateMachineTransition(sourceSubMachine, targetState);
                    CopyTransitionProperties(selectedTransition, newTransition);
                }
                else if (targetSubMachine != null)
                {
                    AnimatorTransition newTransition = commonStateMachine.AddStateMachineTransition(sourceSubMachine, targetSubMachine);
                    CopyTransitionProperties(selectedTransition, newTransition);
                }
                else if (targetSpecialState == "Exit")
                {
                    AnimatorTransition newTransition = commonStateMachine.AddStateMachineExitTransition(sourceSubMachine);
                    CopyTransitionProperties(selectedTransition, newTransition);
                }
            }
            else if (sourceSpecialState == "Entry")
            {
                if (targetState != null)
                {
                    AnimatorStateTransition newTransition = sourceState.AddTransition(targetState);
                    CopyTransitionProperties(selectedTransition, newTransition);
                }
                else if (targetSubMachine != null)
                {
                    AnimatorStateTransition newTransition = sourceState.AddTransition(targetSubMachine);
                    CopyTransitionProperties(selectedTransition, newTransition);
                }
            }else if (sourceSpecialState == "Any State")
            {
                if (targetState != null)
                {
                    AnimatorStateTransition newTransition = sourceState.AddTransition(targetState);
                    CopyTransitionProperties(selectedTransition, newTransition);
                }
                else if (targetSubMachine != null)
                {
                    AnimatorStateTransition newTransition = sourceState.AddTransition(targetSubMachine);
                    CopyTransitionProperties(selectedTransition, newTransition);
                }
            }
        }

        statusMessage = "Transitions pasted successfully.";
        statusMessageType = MessageType.Info;
    }


    private void GetSourceAndTargetElements()
    {
        sourceState = null;
        targetState = null;
        sourceSubMachine = null;
        targetSubMachine = null;
        sourceSpecialState = null;
        targetSpecialState = null;

        var selectedObjects = Selection.objects;
        if (selectedObjects.Length < 2)
        {
            statusMessage = "You must select both source and target elements in the Animator.";
            statusMessageType = MessageType.Error;
            return;
        }

        string firstPath = GetFullPath(selectedObjects[0]);
        string secondPath = GetFullPath(selectedObjects[1]);

        if (firstPath != null && stateDictionary.ContainsKey(firstPath))
            sourceState = stateDictionary[firstPath];
        else if (firstPath != null && subStateMachineDictionary.ContainsKey(firstPath))
            sourceSubMachine = subStateMachineDictionary[firstPath];
        else if (IsSpecialState(firstPath))
            sourceSpecialState = firstPath;

        if (secondPath != null && stateDictionary.ContainsKey(secondPath))
            targetState = stateDictionary[secondPath];
        else if (secondPath != null && subStateMachineDictionary.ContainsKey(secondPath))
            targetSubMachine = subStateMachineDictionary[secondPath];
        else if (IsSpecialState(secondPath))
            targetSpecialState = secondPath;

        statusMessage = $"Source: {sourceState?.name ?? sourceSubMachine?.name ?? sourceSpecialState}, Target: {targetState?.name ?? targetSubMachine?.name ?? targetSpecialState}";
        statusMessageType = MessageType.Info;

    }

    private bool IsSpecialState(string stateName)
    {
        return stateName == "Entry" || stateName == "Exit" || stateName == "Up" || stateName == "Any State";
    }

    private string GetFullPath(Object selectedObject)
    {
        var stateMatch = stateDictionary.FirstOrDefault(x => x.Value == selectedObject);
        if (!string.IsNullOrEmpty(stateMatch.Key))
            return stateMatch.Key;

        var subMachineMatch = subStateMachineDictionary.FirstOrDefault(x => x.Value == selectedObject);
        if (!string.IsNullOrEmpty(subMachineMatch.Key))
            return subMachineMatch.Key;

        return selectedObject.name;
    }

    private AnimatorStateMachine GetCommonStateMachine(
    AnimatorState sourceState, 
    AnimatorStateMachine sourceSubMachine, 
    string sourceSpecialState, 
    AnimatorState targetState, 
    AnimatorStateMachine targetSubMachine, 
    string targetSpecialState)
    {
        AnimatorStateMachine sourceParent = GetParentStateMachine(sourceState, sourceSubMachine);
        AnimatorStateMachine targetParent = GetParentStateMachine(targetState, targetSubMachine);

        if (!string.IsNullOrEmpty(sourceSpecialState))
            return targetParent;

        if (!string.IsNullOrEmpty(targetSpecialState))
            return sourceParent;

        return sourceParent == targetParent ? sourceParent : null;
    }

    private AnimatorStateMachine GetParentStateMachine(AnimatorState state, AnimatorStateMachine subMachine)
    {
        if (state != null)
        {
            return FindParentStateMachine(state);
        }
        if (subMachine != null)
        {
            return FindParentStateMachine(subMachine);
        }
        return null;
    }

    private AnimatorStateMachine FindParentStateMachine(UnityEngine.Object element)
    {
        foreach (var layer in animatorController.layers)
        {
            var parent = FindParentRecursive(layer.stateMachine, element);
            if (parent != null)
                return parent;
        }
        return null;
    }

    private AnimatorStateMachine FindParentRecursive(AnimatorStateMachine currentMachine, UnityEngine.Object element)
    {
        foreach (var childState in currentMachine.states)
        {
            if (childState.state == element)
                return currentMachine;
        }

        foreach (var childSubMachine in currentMachine.stateMachines)
        {
            if (childSubMachine.stateMachine == element)
                return currentMachine;

            var foundParent = FindParentRecursive(childSubMachine.stateMachine, element);
            if (foundParent != null)
                return foundParent;
        }

        return null;
    }

    private void CopyTransitionProperties(AnimatorTransitionBase transitionCopied, AnimatorTransitionBase newTransition){
        if (transitionCopied == null || newTransition == null)
        {
            statusMessage = "One of the transitions is null.";
            statusMessageType = MessageType.Warning;
            return;
        }

        newTransition.mute = transitionCopied.mute;
        newTransition.solo = transitionCopied.solo;
            
        if (transitionCopied is AnimatorStateTransition stateTransitionCopied && newTransition is AnimatorStateTransition stateTransitionNew)
        {
            stateTransitionNew.hasExitTime = stateTransitionCopied.hasExitTime;
            stateTransitionNew.hasFixedDuration = stateTransitionCopied.hasFixedDuration;
        }

        if (transitionCopied.conditions.Length > 0)
        {
            newTransition.conditions = transitionCopied.conditions;
        }
    }

    private void LoadStatesAndSubStateMachines()
    {
        stateDictionary = new Dictionary<string, AnimatorState>();
        subStateMachineDictionary = new Dictionary<string, AnimatorStateMachine>();

        foreach (var layer in animatorController.layers)
        {
            GetStatesRecursive(layer.stateMachine, "", null);
        }

        List<string> stateList = new(stateDictionary.Keys);
        stateList.AddRange(subStateMachineDictionary.Keys.Select(name => "[SM] " + name));

        stateNames = stateList.ToArray();
    }

    private void GetStatesRecursive(AnimatorStateMachine stateMachine, string path, AnimatorStateMachine parentStateMachine)
    {
        string machinePath = string.IsNullOrEmpty(path) ? stateMachine.name : path + "/" + stateMachine.name;

        foreach (ChildAnimatorState childState in stateMachine.states)
        {
            string statePath = machinePath + "/" + childState.state.name;
            stateDictionary[statePath] = childState.state;
        }

        foreach (ChildAnimatorStateMachine childStateMachine in stateMachine.stateMachines)
        {
            string subMachinePath = machinePath + "/" + childStateMachine.stateMachine.name;
            subStateMachineDictionary[subMachinePath] = childStateMachine.stateMachine;

            GetStatesRecursive(childStateMachine.stateMachine, subMachinePath, stateMachine);
        }

        string entryPath = machinePath + " [Entry]";
        string exitPath = machinePath + " [Exit]";
        string upPath = machinePath + " [Up]";
        string anyStatePath = machinePath + " [Any State]";

        if (!stateDictionary.ContainsKey(entryPath))
            stateDictionary[entryPath] = null;

        if (!stateDictionary.ContainsKey(exitPath))
            stateDictionary[exitPath] = null;

        if (!stateDictionary.ContainsKey(anyStatePath))
            stateDictionary[exitPath] = null;

        if (parentStateMachine != null && !stateDictionary.ContainsKey(upPath))
            stateDictionary[upPath] = null;
    }
}