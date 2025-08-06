using System;
using System.Linq;
using UnityEngine;

public class FunGameManager : FunGameScreen
{
    [SerializeField] private WolfController2D wolfController;
    [SerializeField] private GameObject[] leaves;
    public event Action OnFunGameFinished;

    private int leavesCount;
    public void LoadFunGame()
    {
        base.Show();
        leaves = GetLeaves();
        leavesCount = leaves.Length;
        wolfController.OnLeavesCollect += HandleLeavesCollect;
    }

    private GameObject[] GetLeaves()
    {
        Transform[] transformChildrens = GetComponentsInChildren<Transform>(true);
        GameObject[] leavesArr = Array.FindAll(transformChildrens, child => child.CompareTag($"leaves"))
            .Select(child => child.gameObject).ToArray();
        return leavesArr;
    }

    private void HandleLeavesCollect()
    {
        leavesCount--;
        if (leavesCount <= 0)
        {
            wolfController.OnLeavesCollect -= HandleLeavesCollect;
            OnFunGameFinished?.Invoke();
            Hide();
        }
    }
}