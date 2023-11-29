using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIPrefab : MonoBehaviour
{
    MissionGroup missionGroup;

    private Scrollbar LoadingBar;
    private float progress;

    private void Awake()
    {
        LoadingBar = GetComponentInChildren<Scrollbar>();
        missionGroup = new MissionGroup();

        progress = 0;
        LoadingBar.size = 0;
    }

    private void Start()
    {
    }

    private void Update()
    {
        LoadingBar.size = progress;
    }


    public void CreateMission(MissionBase mission)
    {
        mission.OnFinish += () => SetLoadingProgress(missionGroup.CompleteCount, missionGroup.TotalMissionCount);
        mission.OnProcess += () => SetLoadingProgress(missionGroup.CompleteCount, missionGroup.TotalMissionCount);
        mission.OnProgress += () => SetLoadingProgress(missionGroup.CompleteCount, missionGroup.TotalMissionCount);
        mission.OnComplete += () => SetLoadingProgress(missionGroup.CompleteCount, missionGroup.TotalMissionCount);

        this.missionGroup.AddMission(mission);
    }

    public Task LoadingStart()
    {
        if (this.missionGroup.isComplete)
            return Task.CompletedTask;

        progress = 0;

        return missionGroup.MissionGroupWork();
    }


    private Task SetLoadingProgress(float Current, float total)
    {
        if (Current / total <= 1)
            progress = (Current / total);

        else
            progress = 1;

        return Task.Delay(TimeSpan.FromSeconds(0.5));
    }

    public void OnLoadingUIFinish()
    {
        if (this != null)
            Destroy(gameObject);
    }
}
