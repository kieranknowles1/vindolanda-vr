using System.Collections.Generic;
using System.Linq;

public class FollowerTracker : Saveable
{
    public class FollowerSaveData : SaveData
    {
        public List<int> followers = new();

        public FollowerSaveData() { }
        public FollowerSaveData(FollowerTracker tracker)
        {
            followers = tracker.followers.Select(x => x.Id).ToList();
        }
    }

    public List<ActorController> followers = new();

    public override SaveData Save()
    {
        return new FollowerSaveData(this);
    }

    public override void Load(SaveData data)
    {
        base.Load(data);
        var fData = (FollowerSaveData)data;
        followers = fData.followers.Select(f => GuidManager.Instance.Find<ActorController>(f)).ToList();
    }
}
