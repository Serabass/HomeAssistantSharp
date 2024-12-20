namespace Websocket;

using HomeAssistantSharp.Entities.Typed;
using HomeAssistantSharp.Entities.Attributes;

public class MotionSensorEntity(string id) : BinarySensorEntity<IBaseAttributes>(id)
{
}

public class Home
{
  public MotionSensorEntity KitchenMotionSensor = new("binary_sensor.mqtt_kitchen_occupancy");

  public Home()
  {
    KitchenMotionSensor.On(true, () =>
    {
      Console.WriteLine("Kitchen motion sensor is on");
    });
  }

  public async Task Init()
  {
    await KitchenMotionSensor.Init();
  }
}
