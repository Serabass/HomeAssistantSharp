namespace HomeAssistantSharp.Entities;

using HomeAssistantSharp.Entities.Attributes;

public abstract class Entity
{
  public static Dictionary<string, Entity> AllEntities { get; set; } = [];
  public string Id { get; }
  public HAState PrevState { get; set; }
  public HAState CurrentState { get; set; }
  public abstract Task Init();
  public event EventHandler<HAEventData>? StateChanged;

  public void InvokeStateChanged()
  {
    if (CurrentState.IsUnavailable) return;

    StateChanged?.Invoke(this, new()
    {
      EntityId = Id,
      Old = PrevState,
      New = CurrentState
    });
  }

  public Entity(string id)
  {
    Id = id;
    // Subscribe();
    // Register();

    StateChanged += (sender, stateMessage) =>
    {
      PrevState = stateMessage.Old;
      CurrentState = stateMessage.New;
    };
  }
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

public abstract class Entity<A> : Entity, IDisposable where A : IBaseAttributes
{
  public abstract A Attributes { get; }

  public Entity(string id) : base(id)
  {
    Register();
  }

  public override async Task Init()
  {
  }

  public Entity<A> Register()
  {
    AllEntities.Add(Id, this);
    return this;
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
  public override A Attributes => CurrentState.AttributesAs<A>();

  public T StateValue => ConvertStateValue(CurrentState);

  public abstract T ConvertStateValue(HAState state);

  public bool IsUnavailable => CurrentState.IsUnavailable;

  public Entity(string id) : base(id)
  {
    StateChanged += (s, m) =>
    {
      PrevState = m.Old;
      CurrentState = m.New;
    };
  }
}
