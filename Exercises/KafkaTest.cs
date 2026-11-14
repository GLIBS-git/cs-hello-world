using Confluent.Kafka;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Exercises
{
    internal class KafkaTest
    {
        public static void Starter()
        {
            try
            {
                //KafkaTest.PrintAdminClient();
                //KafkaTest.ResetOffset("test-consumer", "test-5", 0);
                //KafkaTest.ReadMessagesAutoCommit("test-5", "test-consumer", 100, false, AutoOffsetReset.Earliest);
                //KafkaTest.ReadMessagesAutoCommit("test-5", "test-consumer", 100, true, AutoOffsetReset.Earliest);
                //KafkaTest.ReadMessagesManualCommit("test-5", "test-consumer", 100, false, AutoOffsetReset.Earliest);
                //KafkaTest.ReadMessagesManualCommit("test-5", "test-consumer", 100, true, AutoOffsetReset.Earliest);
                //KafkaTest.SendMessage();
                //KafkaTest.SendMessageAsync().Wait();
                //KafkaTest.SendMessageAsync2();
                KafkaTest.SendMessageBatch1("test-8", 10000, 1);
                //KafkaTest.SendMessageBatch1("test-5", 10, 21);
                //KafkaTest.SendMessageBatch2("test-7", 1000, 1000, true);
                //KafkaTest.SendMessageBatch3("test-7", 1000, 1000);
                //KafkaTest.SendMessageBatch3("test", 10, 100);
                //KafkaTest.SendMessageBatch4("test-3", 20, 1).Wait();
                //KafkaTest.SendMessageBatch5("test-7", 1000, 1000, true);
                //KafkaTest.SendMessageBatchSameKey("test-3", 100, "Key 1");
                //KafkaTest.Starter2();
                //KafkaTest.Starter3();
            }
            catch (Exception _e)
            {
                Console.WriteLine("Exception!");
                Console.WriteLine(_e.StackTrace);
            }
        }
        public static void Starter2()
        {
            KafkaTest.SendMessageAsync();
            Thread.Sleep(10 * 1000);
            Console.WriteLine("Finish!");
        }
        public static void Starter3()
        {
            KafkaTest.SendMessageBatchSameKey("test-7", 1000, "Key 00001", 1000, true);
            KafkaTest.SendMessageBatchSameKey("test-7", 1000, "Key 00002", 1000, true);
            KafkaTest.SendMessageBatchSameKey("test-7", 1000, "Key 00003", 1000, true);
            Thread.Sleep(90 * 1000);
            Console.WriteLine("Finish!");
        }
        public static void ReadMessagesManualCommit(string _topicName, string _consumerGroup, int _messageQty, bool _doCommit = true, AutoOffsetReset _autoOffsetReset = AutoOffsetReset.Latest)
        {
            Stopwatch sw = Stopwatch.StartNew();
            ConsumerConfig Config = new ConsumerConfig{ BootstrapServers = "192.168.0.185:9092", GroupId = _consumerGroup, AutoOffsetReset = _autoOffsetReset, EnableAutoCommit = false, 
                EnableAutoOffsetStore = true, };
            Boolean anyRead = false;
            using (IConsumer<string, string> consumer = new ConsumerBuilder<string, string>(Config).Build())
            {
                try
                {
                    consumer.Subscribe(_topicName);
                    for (int i = 0; i < _messageQty; i++)
                    {
                        ConsumeResult<string, string> result = consumer.Consume(5000); // Timeout milliseconds // Subscribing takes to much time. Who may guess that?! Timeout must be large!
                        if (result == null)
                        {
                            if (!anyRead)
                                Console.WriteLine("Timeout. No messages.");
                            break;
                        }
                        else
                        {
                            KafkaTest.OutputResult(result);
                            anyRead = true;
                            //if (_doCommit) // Committing offset for each message 
                            //    consumer.Commit(result);
                        }
                    }
                    if (_doCommit)
                        consumer.Commit(); // Committing all read messages
                }
                catch (Exception _e)
                {
                    Console.WriteLine(_e.Message);
                    Console.WriteLine(_e.StackTrace);
                }
                finally
                {
                    consumer?.Close();
                }
            }
            Console.WriteLine($"Elapsed: {sw.Elapsed}");
            sw.Stop();
        }
        public static void ReadMessagesManualCommit2(string _topicName, string _consumerGroup, int _messageQty, bool _doCommit = true, AutoOffsetReset _autoOffsetReset = AutoOffsetReset.Latest)
        {
            Stopwatch sw = Stopwatch.StartNew();
            ConsumerConfig Config = new ConsumerConfig{ BootstrapServers = "192.168.0.185:9092", GroupId = _consumerGroup, AutoOffsetReset = _autoOffsetReset, EnableAutoCommit = false, 
                EnableAutoOffsetStore = false };
            Boolean anyRead = false;
            // Store the last read message for each TopicPartition.
            Dictionary<TopicPartition, ConsumeResult<string, string>> lastResults =
                new Dictionary<TopicPartition, ConsumeResult<string, string>>();
            using (IConsumer<string, string> consumer =
                new ConsumerBuilder<string, string>(Config).Build())
            {
                try
                {
                    consumer.Subscribe(_topicName);

                    for (int i = 0; i < _messageQty; i++)
                    {
                        ConsumeResult<string, string> result =
                            consumer.Consume(5000);
                        if (result == null)
                        {
                            if (!anyRead)
                                Console.WriteLine("Timeout. No messages.");
                            break;
                        }
                        else
                        {
                            KafkaTest.OutputResult(result);
                            anyRead = true;
                            // Remember only the last message for this particular topic + partition.
                            lastResults[result.TopicPartition] = result;
                        }
                    }
                    if (_doCommit && lastResults.Count > 0)
                    {
                        // Explicitly store the offset for each topic/partition.
                        foreach (ConsumeResult<string, string> result in lastResults.Values)
                            consumer.StoreOffset(result);
                        // Commit all explicitly stored offsets once.
                        consumer.Commit();
                    }
                }
                catch (Exception _e)
                {
                    Console.WriteLine(_e.Message);
                    Console.WriteLine(_e.StackTrace);
                }
                finally
                {
                    consumer?.Close();
                }
            }

            Console.WriteLine($"Elapsed: {sw.Elapsed}");
            sw.Stop();
        }
        public static void ReadMessagesAutoCommit(string _topicName, string _consumerGroup, int _messageQty, bool _manualCommit = true, AutoOffsetReset _autoOffsetReset = AutoOffsetReset.Latest)
        {
            Stopwatch sw = Stopwatch.StartNew();
            ConsumerConfig Config = new ConsumerConfig
            {BootstrapServers = "192.168.0.185:9092", GroupId = _consumerGroup, AutoOffsetReset = _autoOffsetReset, EnableAutoCommit = true, EnableAutoOffsetStore = !_manualCommit, 
                AutoCommitIntervalMs = 5000, SessionTimeoutMs = 30000};
            Boolean anyRead = false;
            ConsumeResult<string, string>? lastResult = null;
            using (IConsumer<string, string> consumer = new ConsumerBuilder<string, string>(Config).Build())
            {
                try
                {
                    consumer.Subscribe(_topicName);
                    for (int i = 0; i < _messageQty; i++)
                    {
                        ConsumeResult<string, string>? result = consumer.Consume(5000); // Timeout milliseconds // Subscribing takes to much time. Who may guess that?! Timeout must be large!
                        if (result == null)
                        {
                            if (!anyRead)
                                Console.WriteLine("Timeout. No messages.");
                            break;
                        }
                        else
                        {
                            KafkaTest.OutputResult(result);
                            lastResult = result;
                            anyRead = true;
                        }
                    }
                    if (_manualCommit && anyRead && lastResult != null)
                        consumer.StoreOffset(lastResult);
                }
                catch (Exception _e)
                {
                    Console.WriteLine(_e.Message);
                    Console.WriteLine(_e.StackTrace);
                }
                finally
                {
                    consumer?.Close();
                }
            }
            Console.WriteLine($"Elapsed: {sw.Elapsed}");
            sw.Stop();
        }
        public static void OutputResult(ConsumeResult<string, string> _result)
        {
            Console.WriteLine($"Offset: {_result.Offset}");
            Console.WriteLine($"Partition: {_result.Partition}");
            Console.WriteLine($"Topic: {_result.Topic}");
            Console.WriteLine($"Message key: {_result.Message.Key}");
            Console.WriteLine("Message value:");
            Console.WriteLine($"{_result.Message.Value}");
            Console.WriteLine($"Message timestamp: {_result.Message.Timestamp.UtcDateTime}");
            foreach (IHeader header in _result.Message.Headers)
            {
                Console.WriteLine($"Message header key: {header.Key}");
                Console.WriteLine($"Message header value: {Encoding.UTF8.GetString(header.GetValueBytes())}");
            }
            Console.WriteLine();
        }
        public static void SendMessage()
        {
            ProducerConfig Config = new ProducerConfig{ BootstrapServers = "192.168.0.185:9092", CompressionType = CompressionType.Zstd, CompressionLevel = 6, LingerMs = 100 };
            //ProducerConfig Config = new ProducerConfig();
            //Config.BootstrapServers = "192.168.0.185:9092";
            using (IProducer<string, string> producer = new ProducerBuilder<string, string>(Config).Build())
            {
                try
                {
                    string topic = "test";
                    string key = "Key 3";
                    JsonSerializerOptions opt = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.Never };
                    TestKafkaJson jsonStruct = new TestKafkaJson();
                    jsonStruct.Key = key;
                    jsonStruct.Data = "Test value 3.0";
                    string body = JsonSerializer.Serialize(jsonStruct, opt);
                    Console.WriteLine("Body:");
                    Console.WriteLine(body);
                    Console.WriteLine();
                    Message<string, string> message_1 = new Message<string, string>() { Key = key , Value = body, Headers = new Headers() };
                    message_1.Headers.Add("Key", Encoding.UTF8.GetBytes(key));
                    producer.Produce(topic, message_1);
                    producer.Flush();
                }
                catch (Exception _e)
                {
                    Console.WriteLine("Exception!");
                    Console.WriteLine(_e.StackTrace);
                }
            }
        }
        public static async Task SendMessageAsync() // Experiment 1
        {
            ProducerConfig Config = new ProducerConfig{ BootstrapServers = "192.168.0.185:9092" };
            using (IProducer<string, string> producer = new ProducerBuilder<string, string>(Config).Build())
            {
                try
                {
                    string topic = "test";
                    string key = "Key 0003";
                    JsonSerializerOptions opt = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.Never };
                    TestKafkaJson jsonStruct = new TestKafkaJson();
                    jsonStruct.Key = key;
                    jsonStruct.Data = "Test value 0003.0003";
                    string body = JsonSerializer.Serialize(jsonStruct, opt);
                    Console.WriteLine("Body:");
                    Console.WriteLine(body);
                    Console.WriteLine();
                    Message<string, string> message_1 = new Message<string, string>() { Key = key , Value = body, Headers = new Headers() };
                    message_1.Headers.Add("Key", Encoding.UTF8.GetBytes(key));
                    message_1.Headers.Add("Topic", Encoding.UTF8.GetBytes(topic));
                    DeliveryResult<string, string> deliveryResult = await producer.ProduceAsync(topic, message_1).ConfigureAwait(true);
                    Console.WriteLine($"Offset: {deliveryResult.Offset}");
                    Console.WriteLine($"Partition: {deliveryResult.Partition}");
                    producer.Flush();
                }
                catch (Exception _e)
                {
                    Console.WriteLine("Exception!");
                    Console.WriteLine(_e.StackTrace);
                }
            }
        }
        public static /*async Task*/ void SendMessageAsync2() // Experiment 2
        {
            ProducerConfig Config = new ProducerConfig{ BootstrapServers = "192.168.0.185:9092" };
            using (IProducer<string, string> producer = new ProducerBuilder<string, string>(Config).Build())
            {
                try
                {
                    string topic = "test-3";
                    string key = "Key 0002";
                    JsonSerializerOptions opt = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.Never };
                    TestKafkaJson jsonStruct = new TestKafkaJson();
                    jsonStruct.Key = key;
                    jsonStruct.Data = "Test value 0002.0002";
                    string body = JsonSerializer.Serialize(jsonStruct, opt);
                    Console.WriteLine("Body:");
                    Console.WriteLine(body);
                    Console.WriteLine();
                    Message<string, string> message_1 = new Message<string, string>() { Key = key , Value = body, Headers = new Headers() };
                    message_1.Headers.Add("Key", Encoding.UTF8.GetBytes(key));
                    message_1.Headers.Add("Topic", Encoding.UTF8.GetBytes(topic));
                    Task<DeliveryResult<string, string>> task_1 = producer.ProduceAsync(topic, message_1);
                    //await task_1; // Does not work!
                    task_1.Wait();
                    DeliveryResult<string, string> deliveryResult = task_1.Result;
                    Console.WriteLine($"Offset: {deliveryResult.Offset}");
                    Console.WriteLine($"Partition: {deliveryResult.Partition}");
                    producer.Flush();
                }
                catch (Exception _e)
                {
                    Console.WriteLine(_e.StackTrace);
                }
            }
        }
        public static void SendMessageAsync3() // Experiment 3
        {
            ProducerConfig Config = new ProducerConfig{ BootstrapServers = "192.168.0.185:9092" };
            using (IProducer<string, string> producer = new ProducerBuilder<string, string>(Config).Build()) // == producer.Dispose();
            {
                try
                {
                    string topic = "test";
                    string key = "Key 3";
                    JsonSerializerOptions opt = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.Never };
                    TestKafkaJson jsonStruct = new TestKafkaJson();
                    jsonStruct.Key = key;
                    jsonStruct.Data = "Test value 3.0";
                    string body = JsonSerializer.Serialize(jsonStruct, opt);
                    Console.WriteLine(body);
                    Message<string, string> message_1 = new Message<string, string>() { Key = key , Value = body, Headers = new Headers() };
                    message_1.Headers.Add("Key", Encoding.UTF8.GetBytes(key));
                    //Task[] task_1 = new Task[1];
                    //Task.WaitAll();
                    Task task_1 = producer.ProduceAsync(topic, message_1);
                    task_1.Wait();
                    //Thread thread_1 = new Thread(KafkaTest.SendMassage);
                    //thread_1.Join();
                    producer.Flush();
                    //producer.Dispose();
                }
                catch (Exception _e)
                {
                    Console.WriteLine(_e.StackTrace);
                }
            }
        }
        public static void SendMessageBatch1(string _topicName,int _bathSize, int _version)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Message<string, string>[] message_1 = KafkaTest.TestMessageArray1(_bathSize, _version, _topicName);
            Console.WriteLine($"Creating massages: {sw.Elapsed}");
            sw.Stop();
            sw = Stopwatch.StartNew();
            ProducerConfig Config = new ProducerConfig{ BootstrapServers = "192.168.0.185:9092" };
            using (IProducer<string, string> producer = new ProducerBuilder<string, string>(Config).Build())
            {
                try
                {
                    for (int i = 0; i < message_1.Length; i++)
                        producer.Produce(_topicName, message_1[i]);
                    producer.Flush();
                }
                catch (Exception _e)
                {
                    Console.WriteLine(_e.StackTrace);
                }
            }
            Console.WriteLine($"Sending messages: {sw.Elapsed}");
            sw.Stop();
        }
        public static void SendMessageBatch2(string _topicName, int _bathSize, int _maxLines, bool _minMaxOnly = false)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Message<string, string>[] message = KafkaTest.TestMessageArray2(_bathSize, _maxLines, _topicName, _minMaxOnly);
            Console.WriteLine($"Creating massages: {sw.Elapsed}");
            sw.Stop();
            sw = Stopwatch.StartNew();
            ProducerConfig Config = new ProducerConfig { BootstrapServers = "192.168.0.185:9092", MessageMaxBytes = 10485760, LingerMs = 100 };
            Task<DeliveryResult<string, string>>[] tasks = new Task<DeliveryResult<string, string>>[_bathSize];
            using (IProducer<string, string> producer = new ProducerBuilder<string, string>(Config).Build())
            {
                try
                {
                    for (int i = 0; i < _bathSize; i++)
                    {
                        tasks[i] = producer.ProduceAsync(_topicName, message[i]);
                    }
                    for (int i = 0; i < _bathSize; i++)
                    {
                        DeliveryResult<string, string> deliveryResult = tasks[i].Result;
                        Console.WriteLine($"Result: Message {i}. Offset {deliveryResult.Offset}");
                    }
                    producer.Flush();
                }
                catch (Exception _e)
                {
                    Console.WriteLine(_e.Message);
                    Console.WriteLine(_e.StackTrace);
                }
            }
            Console.WriteLine($"Sending to Kafka: {sw.Elapsed}");
            sw.Stop();
        }
        public static void SendMessageBatch3(string _topicName, int _bathSize, int _maxLines, bool _minMaxOnly = false)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Message<string, string>[] message = KafkaTest.TestMessageArray2(_bathSize, _maxLines, _topicName, _minMaxOnly);
            Console.WriteLine($"Creating massages: {sw.Elapsed}");
            sw.Stop();
            sw = Stopwatch.StartNew();
            ProducerConfig Config = new ProducerConfig{ BootstrapServers = "192.168.0.185:9092" };
            using (IProducer<string, string> producer = new ProducerBuilder<string, string>(Config).Build())
            {
                try
                {
                    for (int i = 0; i < _bathSize; i++)
                    {
                        producer.Produce(_topicName, message[i]);
                    }
                    producer.Flush();
                }
                catch (Exception _e)
                {
                    Console.WriteLine(_e.Message);
                    Console.WriteLine(_e.StackTrace);
                }
            }
            Console.WriteLine($"Sending to Kafka: {sw.Elapsed}");
            sw.Stop();
        }
        async public static Task SendMessageBatch4(string _topicName, int _bathSize, int _maxLines)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Message<string, string>[] message = KafkaTest.TestMessageArray2(_bathSize, _maxLines, _topicName);
            Console.WriteLine($"Creating massages: {sw.Elapsed}");
            sw.Stop();
            sw = Stopwatch.StartNew();
            ProducerConfig Config = new ProducerConfig { BootstrapServers = "192.168.0.185:9092" };
            DeliveryResult<string, string>[] deliveryResult = new DeliveryResult<string, string>[_bathSize];
            using (IProducer<string, string> producer = new ProducerBuilder<string, string>(Config).Build())
            {
                try
                {
                    for (int i = 0; i < _bathSize; i++)
                        deliveryResult[i] = await producer.ProduceAsync(_topicName, message[i]).ConfigureAwait(false);
                    for (int i = 0; i < _bathSize; i++)
                        Console.WriteLine($"Result: Message {i}. Offset {deliveryResult[i].Offset}");
                    producer.Flush();
                }
                catch (Exception _e)
                {
                    Console.WriteLine(_e.Message);
                    Console.WriteLine(_e.StackTrace);
                }
            }
            Console.WriteLine($"Sending to Kafka: {sw.Elapsed}");
            sw.Stop();
        }
        public static void SendMessageBatch5(string _topicName, int _bathSize, int _maxLines, bool _minMaxOnly = false) // Destructive test (break the messages order)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Message<string, string>[] message = KafkaTest.TestMessageArray2(_bathSize, _maxLines, _topicName, _minMaxOnly);
            Console.WriteLine($"Creating massages: {sw.Elapsed}");
            sw.Stop();
            sw = Stopwatch.StartNew();
            ProducerConfig Config = new ProducerConfig { BootstrapServers = "192.168.0.185:9092", MessageMaxBytes = 10485760, LingerMs = 100 };
            Task<DeliveryResult<string, string>>[] tasks = new Task<DeliveryResult<string, string>>[_bathSize];
            IProducer<string, string>[] producer = new IProducer<string, string>[_bathSize];
            try
            {
                for (int i = 0; i < _bathSize; i++)
                    producer[i] = new ProducerBuilder<string, string>(Config).Build();
                for (int i = 0; i < _bathSize; i++)
                    tasks[i] = producer[i].ProduceAsync(_topicName, message[i]);
                for (int i = 0; i < _bathSize; i++)
                {
                    DeliveryResult<string, string> deliveryResult = tasks[i].Result;
                    Console.WriteLine($"Result: Message {i}. Offset {deliveryResult.Offset}");
                }
                for (int i = 0; i < _bathSize; i++)
                {
                    producer[i].Flush();
                    producer[i].Dispose();
                }
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);
                Console.WriteLine(_e.StackTrace);
            }
            Console.WriteLine($"Sending to Kafka: {sw.Elapsed}");
            sw.Stop();
        }
        public async static Task SendMessageBatchSameKey(string _topicName,int _bathSize, String _key, int _maxLines, bool _minMaxOnly = false)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Message<string, string>[] message = KafkaTest.TestMessageArray3(_bathSize, _maxLines, _key, _minMaxOnly, _topicName);
            Console.WriteLine($"Creating massages thread {_key}: {sw.Elapsed}");
            sw.Stop();
            sw = Stopwatch.StartNew();
            ProducerConfig Config = new ProducerConfig{ BootstrapServers = "192.168.0.185:9092" };
            JsonSerializerOptions opt = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.Never };
            using (IProducer<string, string> producer = new ProducerBuilder<string, string>(Config).Build())
            {
                try
                {
                    for (int i = 0; i < _bathSize; i++)
                    {
                        DeliveryResult<string, string> deliveryResult = await producer.ProduceAsync(_topicName, message[i]);
                        Console.WriteLine($"Result {_key}: Message {i}. Offset {deliveryResult.Offset}");
                    }
                    producer.Flush();
                }
                catch (Exception _e)
                {
                    Console.WriteLine(_e.StackTrace);
                }
            }
            Console.WriteLine($"Sent messages: {sw.Elapsed}");
            sw.Stop();
        }
        public static Message<string, string>[] TestMessageArray1(int _bathSize, int _version = 0, string _topicName = "") // Simple message with one line in the body
        {
            JsonSerializerOptions opt = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.Never };
            Message<string, string>[] message = new Message<string, string>[_bathSize];
            for (int i = 0; i < _bathSize; i++)
            {
                string key = $"Key {i.ToString("00000")}";
                TestKafkaJson jsonStruct = new TestKafkaJson();
                jsonStruct.Key = key;
                jsonStruct.Data = $"Test value: {key}.{_version.ToString("0000")}.";
                string body = JsonSerializer.Serialize(jsonStruct, opt);
                message[i] = new Message<string, string>() { Key = key, Value = body, Headers = new Headers() };
                message[i].Headers.Add("Topic", Encoding.UTF8.GetBytes(_topicName));
                message[i].Headers.Add("Key", Encoding.UTF8.GetBytes(key));
                message[i].Headers.Add("Version", Encoding.UTF8.GetBytes(_version.ToString()));
            }
            return message;
        }
        public static Message<string, string>[] TestMessageArray2(int _bathSize, int _maxLines, string _topicName = "", bool _minMaxOnly = false) // Message with multiple lines in the body
        {
            Message<string, string>[] message = new Message<string, string>[_bathSize];
            for (int i = 0; i < _bathSize; i++)
            {
                //string key = $"Key {i.ToString("00000")}";
                string key = $"Key {i:D5}";
                JsonSerializerOptions opt = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.Never };
                TestKafkaJsonHeader messageJson = new TestKafkaJsonHeader();
                messageJson.Key = key;
                int lines = 0;
                if (_minMaxOnly)
                {
                    lines = new Random().Next(1, 3);
                    if (lines == 2)
                        lines = _maxLines;
                }
                else
                    lines = new Random().Next(1, _maxLines);
                List<TestKafkaJsonLines> messageData = new List<TestKafkaJsonLines>();
                for (int j = 1; j <= lines; j++)
                {
                    TestKafkaJsonLines messageLine = new TestKafkaJsonLines();
                    messageLine.LineNum = $"{j}";
                    messageLine.LineData = $"Line number '{j}' of '{lines}' for message with key '{key}'.";
                    messageData.Add(messageLine);
                }
                messageJson.Data = messageData;
                string body = JsonSerializer.Serialize(messageJson, opt);
                message[i] = new Message<string, string>() { Key = key, Value = body, Headers = new Headers() };
                message[i].Headers.Add("Topic", Encoding.UTF8.GetBytes(_topicName));
                message[i].Headers.Add("Key", Encoding.UTF8.GetBytes(key));
                message[i].Headers.Add("Lines", Encoding.UTF8.GetBytes($"{lines}"));
            }
            return message;
        }
        public static Message<string, string>[] TestMessageArray3(int _bathSize, int _maxLines, string _key, bool _minMaxOnly = false, string _topicName = "") // For sending messages with same key
        {
            Message<string, string>[] message = new Message<string, string>[_bathSize];
            for (int i = 0; i < _bathSize; i++)
            {
                JsonSerializerOptions opt = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.Never };
                TestKafkaJsonHeader messageJson = new TestKafkaJsonHeader();
                messageJson.Key = _key;
                messageJson.Version = i.ToString("00000");
                int lines = 0;
                if (_minMaxOnly)
                {
                    lines = new Random().Next(1, 3);
                    if (lines == 2)
                        lines = _maxLines;
                }
                else
                {
                    lines = new Random().Next(1, _maxLines);
                }
                List<TestKafkaJsonLines> messageData = new List<TestKafkaJsonLines>();
                for (int j = 1; j <= lines; j++)
                {
                    TestKafkaJsonLines messageLine = new TestKafkaJsonLines();
                    messageLine.LineNum = $"{j}";
                    messageLine.LineData = $"Line number '{j}' of '{lines}' for message with key '{_key}.{i.ToString("00000")}'.";
                    messageData.Add(messageLine);
                }
                messageJson.Data = messageData;
                string body = JsonSerializer.Serialize(messageJson, opt);
                message[i] = new Message<string, string>() { Key = _key, Value = body, Headers = new Headers() };
                message[i].Headers.Add("Topic", Encoding.UTF8.GetBytes(_topicName));
                message[i].Headers.Add("Key", Encoding.UTF8.GetBytes(_key));
                message[i].Headers.Add("Version", Encoding.UTF8.GetBytes(i.ToString("00000")));
                message[i].Headers.Add("Lines", Encoding.UTF8.GetBytes($"{lines}"));
            }
            return message;
        }
        public static void PrintAdminClient()
        {
            AdminClientConfig config = new AdminClientConfig { BootstrapServers = "192.168.0.185:9092" };
            using (IAdminClient adminClient = new AdminClientBuilder(config).Build())
            {
                Metadata metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                Console.WriteLine($"Topics count: {metadata.Topics.Count}");
                foreach (TopicMetadata topicMetadata in metadata.Topics)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Topic name: {topicMetadata.Topic}. Partitions: {topicMetadata.Partitions.Count}.");
                    foreach (PartitionMetadata partitionMetadata in topicMetadata.Partitions)
                    {
                        Console.WriteLine($"        Partition ID: {partitionMetadata.PartitionId}. Leader: {partitionMetadata.Leader}. " +
                            $"Replicas: {string.Join(", ", partitionMetadata.Replicas)}. InSyncReplicas: {string.Join(", ", partitionMetadata.InSyncReplicas)}.");
                    }
                }
                //TopicMetadata topicMetadata = metadata.Topics.FirstOrDefault(t => t.Topic == _topicName);
                Console.WriteLine();
                foreach (BrokerMetadata brockerMetadata in metadata.Brokers)
                {
                    Console.WriteLine($"Brocker ID: {brockerMetadata.BrokerId}. Host: {brockerMetadata.Host}. Port: {brockerMetadata.Port}.");
                }
            }
        }
        public static void TopicPartitionTest()
        {
            TopicPartition topicPartition_1 = new TopicPartition("", 0);
        }
        public static void ResetOffset(string _consumerGroup, string _topicName, int _partition) // Not working!
        {
            Stopwatch sw = Stopwatch.StartNew();
            ConsumerConfig Config = new ConsumerConfig { BootstrapServers = "192.168.0.185:9092", GroupId = _consumerGroup };
            using (IConsumer<string, string> consumer = new ConsumerBuilder<string, string>(Config).Build())
            {
                consumer.Subscribe(_topicName);
                Thread.Sleep(5 * 1000);
                Partition partition_2 = new Partition(_partition);
                TopicPartition topicPartition_2 = new TopicPartition(_topicName, partition_2);
                consumer.Assign(topicPartition_2);
                List<TopicPartition> assignment_1 = consumer.Assignment;
                foreach (TopicPartition topicPartition_1 in assignment_1)
                {
                    //consumer.Assign(topicPartition_1);
                    TopicPartitionOffset topicPartitionOffset_1 = new TopicPartitionOffset(topicPartition_1, Offset.Beginning);
                    consumer.Seek(topicPartitionOffset_1);
                }
                consumer.Close();
            }
            Console.WriteLine($"Elapsed: {sw.Elapsed}");
            sw.Stop();
        }
        public static void ResetOffset2(string _consumerGroup, string _topicName, int _partition) // Not working!
        {
            Stopwatch sw = Stopwatch.StartNew();
            ConsumerConfig Config = new ConsumerConfig { BootstrapServers = "192.168.0.185:9092", GroupId = _consumerGroup };
            using (IConsumer<string, string> consumer = new ConsumerBuilder<string, string>(Config).Build())
            {
                consumer.Subscribe(_topicName);
                Thread.Sleep(5 * 1000);
                Partition partition_1 = new Partition(_partition);
                TopicPartition topicPartition_1 = new TopicPartition(_topicName, partition_1);
                consumer.Assign(topicPartition_1);
                TopicPartitionOffset topicPartitionOffset_1 = new TopicPartitionOffset(topicPartition_1, Offset.Beginning);
                consumer.Seek(topicPartitionOffset_1);
                consumer.Close();
            }
            Console.WriteLine($"Elapsed: {sw.Elapsed}");
            sw.Stop();
        }
    }
    public struct TestKafkaJson
    {
        public string Key { get; set; }
        public string Data { get; set; }
    }
    public struct TestKafkaJsonHeader
    {
        public string Key { get; set; }
        public string Version { get; set; }
        public IEnumerable<TestKafkaJsonLines> Data { get; set; }
    }
    public struct TestKafkaJsonLines
    {
        public string LineNum { get; set; }
        public string LineData { get; set; }
    }
}
