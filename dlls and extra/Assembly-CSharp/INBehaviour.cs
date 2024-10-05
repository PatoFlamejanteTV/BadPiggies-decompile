public class INBehaviour
{
	public enum StatusCode
	{
		None,
		Building,
		Running
	}

	protected StatusCode m_status;

	public StatusCode Status => m_status;

	public virtual void Awake()
	{
	}

	public virtual void Start()
	{
	}

	public virtual void FixedUpdate()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void LateUpdate()
	{
	}

	public virtual void OnEnable()
	{
	}

	public virtual void OnDisable()
	{
	}

	public virtual void OnDestroy()
	{
	}
}
