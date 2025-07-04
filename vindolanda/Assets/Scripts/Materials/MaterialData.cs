using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Material Data")]
public class MaterialData : ScriptableObject
{
    static Dictionary<PhysicsMaterial, MaterialData> extraDatas = new();
    static MaterialData defaultMaterialData;

    public static MaterialData GetExtraData(PhysicsMaterial material)
    {
        if (material != null && extraDatas.TryGetValue(material, out var data)) return data;
        return defaultMaterialData;
    }

    [Tooltip("The material this data describes")]
    public PhysicsMaterial material;
    [Tooltip("Is this data for the default material?")]
    public bool isDefault;

    public FootstepData footsteps;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterData()
    {
        var datas = Resources.LoadAll<MaterialData>("MaterialData");
        extraDatas = datas.ToDictionary(d => d.material, d => d);
        defaultMaterialData = datas.First(d => d.isDefault);

        if (defaultMaterialData == null) Debug.LogWarning($"No {nameof(defaultMaterialData)} set");
    }
}
