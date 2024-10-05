using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResourceBar : WPFMonoBehaviour
{
	public enum Item
	{
		SnoutCoin,
		Scrap,
		PlayerProgress,
		StarCounter
	}

	private static ResourceBar instance;

	public ResourceBarItem[] items;

	private Transform leftItems;

	private Transform rightItems;

	private Dictionary<Item, bool> locked;

	private Dictionary<Item, Tuple<bool, bool>> pending;

	public static ResourceBar Instance
	{
		get
		{
			if (instance == null)
			{
				Object.Instantiate(WPFMonoBehaviour.gameData.m_resourceBar);
			}
			return instance;
		}
	}

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		leftItems = base.transform.Find("Left");
		rightItems = base.transform.Find("Right");
		locked = new Dictionary<Item, bool>
		{
			{
				Item.PlayerProgress,
				false
			},
			{
				Item.Scrap,
				false
			},
			{
				Item.SnoutCoin,
				false
			},
			{
				Item.StarCounter,
				false
			}
		};
		pending = new Dictionary<Item, Tuple<bool, bool>>
		{
			{
				Item.PlayerProgress,
				new Tuple<bool, bool>(IsItemActive(Item.PlayerProgress), IsItemEnabled(Item.PlayerProgress))
			},
			{
				Item.Scrap,
				new Tuple<bool, bool>(IsItemActive(Item.Scrap), IsItemEnabled(Item.Scrap))
			},
			{
				Item.SnoutCoin,
				new Tuple<bool, bool>(IsItemActive(Item.SnoutCoin), IsItemEnabled(Item.SnoutCoin))
			},
			{
				Item.StarCounter,
				new Tuple<bool, bool>(IsItemActive(Item.StarCounter), IsItemEnabled(Item.StarCounter))
			}
		};
		ShowItem(Item.SnoutCoin, showItem: false);
		ShowItem(Item.Scrap, showItem: false);
		ShowItem(Item.PlayerProgress, showItem: false);
		ShowItem(Item.StarCounter, showItem: false);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void UpdateLayout()
	{
		if ((bool)WPFMonoBehaviour.hudCamera)
		{
			base.transform.position = WPFMonoBehaviour.hudCamera.transform.position + Vector3.up * WPFMonoBehaviour.hudCamera.orthographicSize + Vector3.forward * 1f;
		}
		float num = 0f;
		if (leftItems != null)
		{
			for (int i = 0; i < leftItems.childCount; i++)
			{
				Transform child = leftItems.GetChild(i);
				if (child.gameObject.activeInHierarchy)
				{
					ResourceBarItem component = child.GetComponent<ResourceBarItem>();
					if (component != null)
					{
						num += component.PaddingLeft;
						component.SetHorizontalPosition(num);
						num += component.PaddingRight;
					}
				}
			}
		}
		num = 0f;
		if (!(rightItems != null))
		{
			return;
		}
		for (int num2 = rightItems.childCount - 1; num2 >= 0; num2--)
		{
			Transform child2 = rightItems.GetChild(num2);
			if (child2.gameObject.activeInHierarchy)
			{
				ResourceBarItem component2 = child2.GetComponent<ResourceBarItem>();
				if (component2 != null)
				{
					num -= component2.PaddingRight;
					component2.SetHorizontalPosition(num);
					num -= component2.PaddingLeft;
				}
			}
		}
	}

	public bool IsItemActive(Item itemToShow)
	{
		if (locked[itemToShow])
		{
			return pending[itemToShow].Item1;
		}
		if (items != null && (int)itemToShow < items.Length && items[(int)itemToShow] != null)
		{
			return items[(int)itemToShow].IsShowing;
		}
		return false;
	}

	public bool IsItemEnabled(Item itemToShow)
	{
		if (locked[itemToShow])
		{
			return pending[itemToShow].Item2;
		}
		if (items != null && (int)itemToShow < items.Length && items[(int)itemToShow] != null)
		{
			return items[(int)itemToShow].IsEnabled;
		}
		return false;
	}

	public void ShowItem(Item itemToShow, bool showItem, bool enableItem = true)
	{
		if (locked[itemToShow])
		{
			pending[itemToShow].Item1 = showItem;
			pending[itemToShow].Item2 = enableItem;
		}
		else if (items != null && (int)itemToShow < items.Length && items[(int)itemToShow] != null)
		{
			items[(int)itemToShow].SetItem(showItem, enableItem);
		}
	}

	public void LockItem(Item item, bool showItem, bool enableItem, bool revertable)
	{
		if (!locked[item])
		{
			pending[item].Item1 = ((!revertable) ? showItem : IsItemActive(item));
			pending[item].Item2 = ((!revertable) ? enableItem : IsItemEnabled(item));
			ShowItem(item, showItem, enableItem);
			locked[item] = true;
		}
	}

	public void ReleaseItem(Item item)
	{
		if (locked[item])
		{
			locked[item] = false;
			ShowItem(item, pending[item].Item1, pending[item].Item2);
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		UpdateLayout();
	}
}
