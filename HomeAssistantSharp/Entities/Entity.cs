namespace HomeAssistantSharp.Entities;

public abstract class Entity
{
  public static Dictionary<string, Entity> AllEntities { get; set; } = [];
  public string Id { get; } = id;
  public HAState PrevState { get; set; }
  public HAState CurrentState { get; set; }
  public abstract Task Init();
  public event EventHandler<HAEventData>? StateChanged;
  public abstract void InvokeStateChanged();
  public Entity(string id) : base(id)
  {
    // Subscribe();
    // Register();

    StateChanged += (sender, stateMessage) =>
    {
      PrevState = stateMessage.oldState;
      CurrentState = stateMessage.newState;
    };
  }
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

public abstract class Entity<A> : Entity, IDisposable where A : IBaseAttributes
{
  public abstract A Attributes { get; }

  public override void InvokeStateChanged()
  {
    if (CurrentState.IsUnavailable) return;

    StateChanged?.Invoke(this, new()
    {
      entityId = Id,
      oldState = PrevState,
      newState = CurrentState
    });
  }

  public virtual void Dispose()
  {
    GC.SuppressFinalize(this);
    // Unsubscribe();
    // Unregister();
  }
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

public abstract partial class Entity<T, A> : Entity<A> where A : IBaseAttributes
{
  public override A Attributes => CurrentState.Attributes<A>();

  public T StateValue => ConvertStateValue(CurrentState);

  public abstract T ConvertStateValue(HAState state);

  public bool IsUnavailable => CurrentState.IsUnavailable;

  public Entity(string id) : base(id)
  {
    StateChanged += (s, m) =>
    {
      PrevState = m.oldState;
      CurrentState = m.newState;
    };
  }
}
