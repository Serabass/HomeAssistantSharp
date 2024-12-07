namespace Websocket;

using HomeAssistantSharp.Entities.Typed;
using HomeAssistantSharp.Entities.Attributes;

public class MotionSensorEntity : BinarySensorEntity<IBaseAttributes>
{
  public MotionSensorEntity(string id) : base(id) { }
}

public class Home
{
  public MotionSensorEntity KitchenMotionSensor = new MotionSensorEntity("binary_sensor.mqtt_kitchen_occupancy");

  public Home()
  {
    KitchenMotionSensor.On(true, () =>
    {
      Console.WriteLine("Kitchen motion sensor is on");
    });
  }
}
