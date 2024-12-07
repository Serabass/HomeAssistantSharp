namespace HomeAssistantSharp.Entities.Typed;

using HomeAssistantSharp.Entities.Attributes;
using System.Globalization;

public class FloatEntity<A>(string entityId) : Entity<float, A>(entityId) where A : IBaseAttributes
{
  public override float ConvertStateValue(HAState state) =>
    float.Parse(state.State ?? "0", CultureInfo.InvariantCulture);

  public override async Task Init()
  {
    await base.Init();
  }
}
