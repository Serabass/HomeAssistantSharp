namespace HomeAssistantSharp.Entities;

public abstract partial class Entity<T, A> : Entity<A>
{
  public Entity<T, A> On(T val, Action action)
  {
    StateChanged += (s, m) =>
    {
      var newStateValue = ConvertStateValue(m.New);

      if (newStateValue.Equals(val))
      {
        action();
      }
    };

    return this;
  }
}
