namespace Websocket;

using HomeAssistantSharp.Entities;

public class MotionSensorEntity : BinarySensorEntity<IBaseAttributes>
{
  public MotionSensorEntity(string id) : base(id) { }
}

public class Home
{
  public MotionSensorEntity KitchenMotionSensor = new MotionSensorEntity("binary_sensor.mqtt_kitchen_occupancy");

  public Home()
  {
    KitchenMotionSensor.StateChanged += (sender, args) =>
    {
      Console.WriteLine($"Kitchen motion sensor state changed to {args.New.State}");
    };
  }
}
