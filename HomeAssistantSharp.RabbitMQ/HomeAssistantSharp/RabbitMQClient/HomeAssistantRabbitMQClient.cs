namespace HomeAssistantSharp.RabbitMQClient;

using System.Text;
using HomeAssistantSharp.Client;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

#pragma warning disable CS1998

public abstract class HomeAssistantRabbitMQListener<D> : HomeAssistantClientBase
{
  private readonly string _ip;
  private readonly short _port;

  protected abstract string Label { get; }

  private ConnectionFactory _rabbitmq { get; set; }
  public IConnection? connection;
  public IModel? channel;

  private EventingBasicConsumer? consumer;

  public bool Connected = false; у

  protected abstract D ConvertData(string str);

  public event EventHandler<RabbitMQListenerResult<D>>? OnDataReceived;

  public HomeAssistantRabbitMQListener(string ip, short port = 5672)
  {
    _ip = ip;
    _port = port;
    _rabbitmq = new ConnectionFactory()
    {
      HostName = _ip,
      Port = _port,
    };
  }

  public override async Task ConnectAsync()
  {
    if (Connected)
      return;

    connection = _rabbitmq.CreateConnection();
    channel = connection.CreateModel();
    channel.QueueDeclare(
      queue: Label,
      durable: true,
      exclusive: false,
      autoDelete: false
    // arguments: new Dictionary<string, object>
    // {
    //   { "x-message-ttl", 10000 }
    // }
    );

    Consume();

    Connected = true;
  }

  private void Consume()
  {
    consumer = new EventingBasicConsumer(channel);

    consumer.Registered += (sender, e) =>
    {
      Console.WriteLine($"Registered RabbitMQ {Label}. host: {_rabbitmq.HostName}:{_rabbitmq.Port}");
    };

    consumer.Shutdown += (sender, e) =>
    {
      Console.WriteLine($"Shutdown RabbitMQ {Label} {e.ReplyCode} {e.ReplyText}");
    };

    consumer.Unregistered += (sender, e) =>
    {
      Console.WriteLine($"Unregistered RabbitMQ {Label}");
    };

    consumer.Received += (model, ea) =>
    {
      var body = ea.Body.Span;
      var json = Encoding.UTF8.GetString(body);
      D data = ConvertData(json);

      channel.BasicAck(ea.DeliveryTag, false);

      if (data == null)
        return;

      OnDataReceived?.Invoke(this, new RabbitMQListenerResult<D>
      {
        Data = data,
        Source = json
      });
    };

    consumer.Shutdown += (sender, e) =>
    {
      Console.WriteLine($"Shutdown RabbitMQ {Label} {e.ReplyCode} {e.ReplyText}. Exiting...");
      Environment.Exit(0);
    };

    channel.BasicConsume(queue: Label, autoAck: false, consumer: consumer);

  }

  public override void Dispose()
  {
    connection?.Dispose();
    channel?.Dispose();
  }
}
