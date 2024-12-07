namespace HomeAssistantSharp.Entities.Typed;

using HomeAssistantSharp.Entities.Attributes;

/**
  Boolean entity

  TODO Rename to BinarySensorEntity
*/
public class BinarySensorEntity<A>(string entityId) : Entity<bool, A>(entityId) where A : IBaseAttributes
{
  public override bool ConvertStateValue(HAState state) => state.State == "on";
}
