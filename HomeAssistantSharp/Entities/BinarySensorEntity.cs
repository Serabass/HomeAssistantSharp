namespace HomeAssistantSharp.Entities;

/**
  Boolean entity

  TODO Rename to BinarySensorEntity
*/
public class BinarySensorEntity<A>(string entityId) : Entity<bool, A>(entityId) where A : IBaseAttributes
{
  public override bool ConvertStateValue(HAState state) => state.state == "on";

  public override async Task Init()
  {
    await base.Init();
  }
}
