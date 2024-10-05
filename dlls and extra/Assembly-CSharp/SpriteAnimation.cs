using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Sprite))]
public class SpriteAnimation : MonoBehaviour
{
	public delegate void OnPlay(string name);

	[Serializable]
	public class Animation
	{
		public string name;

		public bool loop;

		public List<FrameTiming> frames;

		public Animation(string name)
		{
			this.name = name;
			frames = new List<FrameTiming>();
		}

		public Animation()
		{
			name = string.Empty;
			frames = new List<FrameTiming>();
		}
	}

	[Serializable]
	public class FrameTiming
	{
		public string id;

		public float time = 0.2f;

		[NonSerialized]
		public float endTime;

		[NonSerialized]
		public Vector2[] uv;

		[NonSerialized]
		public string timeText;

		[NonSerialized]
		public Mesh mesh;

		public FrameTiming(string id, float time, Rect uvRect)
		{
			this.id = id;
			this.time = time;
			timeText = this.time.ToString();
			uv = new Vector2[4];
			uv[0].x = uvRect.x;
			uv[0].y = uvRect.y;
			uv[1].x = uvRect.x;
			uv[1].y = uvRect.y + uvRect.height;
			uv[2].x = uvRect.x + uvRect.width;
			uv[2].y = uvRect.y + uvRect.height;
			uv[3].x = uvRect.x + uvRect.width;
			uv[3].y = uvRect.y;
		}
	}

	public OnPlay onPlay;

	[SerializeField]
	private List<Animation> m_animations = new List<Animation>();

	[SerializeField]
	private List<SpriteAnimation> m_childAnimations;

	[SerializeField]
	private bool m_AutoPlay;

	[SerializeField]
	private string m_AutoPlayName;

	[SerializeField]
	private bool m_useTestKeys;

	private Sprite m_sprite;

	private float m_timer;

	private int m_frame;

	private Animation m_currentAnimation;

	private Animation m_nextAnimation;

	private MeshFilter m_meshFilter;

	private bool m_initialized;

	public List<Animation> Animations
	{
		get
		{
			return m_animations;
		}
		set
		{
			m_animations = value;
		}
	}

	private void OnEnable()
	{
		if (m_AutoPlay && !string.IsNullOrEmpty(m_AutoPlayName))
		{
			Play(m_AutoPlayName);
		}
	}

	public List<Animation> GetAnimations()
	{
		return m_animations;
	}

	public Animation GetAnimation(string name)
	{
		for (int i = 0; i < m_animations.Count; i++)
		{
			if (m_animations[i].name == name)
			{
				return m_animations[i];
			}
		}
		return null;
	}

	public void CopyFrom(SpriteAnimation source)
	{
		m_animations = new List<Animation>(source.m_animations);
		m_childAnimations = new List<SpriteAnimation>(source.m_childAnimations);
		InitializeAnimations(Singleton<RuntimeSpriteDatabase>.Instance);
	}

	public void Play(string name)
	{
		if (!m_initialized)
		{
			InitializeAnimations(Singleton<RuntimeSpriteDatabase>.Instance);
		}
		m_nextAnimation = null;
		for (int i = 0; i < m_animations.Count; i++)
		{
			if (m_animations[i].name == name)
			{
				m_nextAnimation = m_animations[i];
				break;
			}
		}
		if (m_nextAnimation == null && m_animations.Count > 0)
		{
			m_nextAnimation = m_animations[0];
		}
		if (m_currentAnimation == null && m_nextAnimation != null)
		{
			PlayImmediately(m_nextAnimation);
		}
	}

	private void PlayImmediately(Animation animation)
	{
		if (!m_initialized)
		{
			InitializeAnimations(Singleton<RuntimeSpriteDatabase>.Instance);
		}
		m_currentAnimation = animation;
		m_nextAnimation = null;
		m_timer = 0f;
		m_frame = 0;
		m_meshFilter.sharedMesh = m_currentAnimation.frames[m_frame].mesh;
		for (int i = 0; i < m_childAnimations.Count; i++)
		{
			m_childAnimations[i].PlayImmediately(animation.name);
		}
		if (onPlay != null)
		{
			onPlay(animation.name);
		}
	}

	private void PlayImmediately(string name)
	{
		m_currentAnimation = null;
		Play(name);
	}

	public void InitializeAnimations(RuntimeSpriteDatabase db)
	{
		m_timer = 0f;
		m_frame = 0;
		m_sprite = GetComponent<Sprite>();
		m_meshFilter = GetComponent<MeshFilter>();
		string id = m_sprite.Id;
		foreach (Animation animation in m_animations)
		{
			float num = 0f;
			foreach (FrameTiming frame in animation.frames)
			{
				num = (frame.endTime = num + frame.time);
				SpriteData data = db.Find(frame.id);
				m_sprite.SelectSprite(data, forceResetMesh: true);
				frame.mesh = m_sprite.GetComponent<MeshFilter>().sharedMesh;
			}
		}
		m_sprite.SelectSprite(db.Find(id), forceResetMesh: true);
		m_initialized = true;
	}

	private void Awake()
	{
		if (!m_initialized)
		{
			m_sprite = GetComponent<Sprite>();
			m_meshFilter = GetComponent<MeshFilter>();
			m_timer = 0f;
			m_frame = 0;
			InitializeAnimations(Singleton<RuntimeSpriteDatabase>.Instance);
		}
	}

	private void Update()
	{
		if (m_currentAnimation == null)
		{
			return;
		}
		m_timer += Time.deltaTime;
		if (!(m_timer >= m_currentAnimation.frames[m_frame].endTime))
		{
			return;
		}
		if (m_frame >= m_currentAnimation.frames.Count - 1)
		{
			if (m_nextAnimation != null)
			{
				PlayImmediately(m_nextAnimation);
			}
			else if (m_currentAnimation.loop)
			{
				m_frame = 0;
				m_timer = 0f;
				m_meshFilter.sharedMesh = m_currentAnimation.frames[m_frame].mesh;
			}
		}
		else
		{
			m_frame++;
			m_meshFilter.sharedMesh = m_currentAnimation.frames[m_frame].mesh;
		}
	}
}
