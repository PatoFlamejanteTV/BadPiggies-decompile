using UnityEngine;

public class DeviceDependency : MonoBehaviour
{
	[SerializeField]
	private DeviceInfo.DeviceFamily[] enabledOnDevices;

	private void Awake()
	{
		DeviceInfo.DeviceFamily activeDeviceFamily = DeviceInfo.ActiveDeviceFamily;
		bool flag = false;
		DeviceInfo.DeviceFamily[] array = enabledOnDevices;
		foreach (DeviceInfo.DeviceFamily deviceFamily in array)
		{
			if (activeDeviceFamily == deviceFamily)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			base.gameObject.SetActive(value: false);
			Object.Destroy(base.gameObject);
		}
	}
}
