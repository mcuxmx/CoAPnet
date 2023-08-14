﻿using CoAPnet;
using CoAPnet.Client;
using CoAPnet.Extensions.DTLS;
using CoAPnet.Logging;
using CoAPnet.Protocol.Options;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoAP.TestClient
{
    static class Program
    {
        static async Task Main22()
        {
            var coapFactory = new CoapFactory();
            coapFactory.DefaultLogger.RegisterSink(new CoapNetLoggerConsoleSink());

            using var coapClient = coapFactory.CreateClient();

            Console.WriteLine("< CONNECTING...");

            var connectOptions = new CoapClientConnectOptionsBuilder()
                .WithHost("GW-B8D7AF2B3EA3.fritz.box")
                //.WithHost("127.0.0.1")
                .WithDtlsTransportLayer(o =>
                    o.WithPreSharedKey("Client_identity", "7x3A1gqWvu9cBGD7"))
                .Build();

            using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                await coapClient.ConnectAsync(connectOptions, cancellationTokenSource.Token);
            }

            var request = new CoapRequestBuilder()
                .WithMethod(CoapRequestMethod.Get)
                .WithPath("15001")
                .Build();

            using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                var response = await coapClient.RequestAsync(request, cancellationTokenSource.Token);
                PrintResponse(response);
            }
        }

        static async Task MainPsk()
        {
            // Generate new PSK Token.
            
            var coapFactory = new CoapFactory();
            coapFactory.DefaultLogger.RegisterSink(new CoapNetLoggerConsoleSink());

            using (var coapClient = coapFactory.CreateClient())
            {
                Console.WriteLine("< CONNECTING...");

                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost("GW-B8D7AF2B3EA3.fritz.box")
                    .WithDtlsTransportLayer(o =>
                        o.WithPreSharedKey("Client_identity", File.ReadAllText(@"D:\SourceCode\Wirehome.Private\Tradfri\Key.txt")))
                    .Build();

                using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    await coapClient.ConnectAsync(connectOptions, cancellationTokenSource.Token).ConfigureAwait(false);
                }

                var request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Post)
                    .WithPath("15011/9063")
                    .WithPayload("{\"9090\":\"WH\"}")
                    .Build();

                var response = await coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);
                PrintResponse(response);
            }
        }

        static async Task Main8()
        {
            var coapFactory = new CoapFactory();
            coapFactory.DefaultLogger.RegisterSink(new CoapNetLoggerConsoleSink());

            using (var coapClient = coapFactory.CreateClient())
            {
                Console.WriteLine("< CONNECTING...");

                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost("GW-B8D7AF2B3EA3.fritz.box")
                    .WithDtlsTransportLayer(o =>
                        o.WithPreSharedKey("WH", "UP3ThsT7ineCsKoc"))
                    .Build();

                using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    await coapClient.ConnectAsync(connectOptions, cancellationTokenSource.Token).ConfigureAwait(false);
                }

                var request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("15001")
                    .Build();

                var response = await coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);
                PrintResponse(response);

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("15001/65550")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);
                PrintResponse(response);

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Put)
                    .WithPath("15001/65550")
                    .WithPayload("{\"3311\": [{\"5850\": 1}]}")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None).ConfigureAwait(false);
                PrintResponse(response);

                var observeOptions = new CoapObserveOptionsBuilder()
                    .WithPath("15001/65550")
                    .WithResponseHandler(new ResponseHandler())
                    .Build();

                var observeResponse = await coapClient.ObserveAsync(observeOptions, CancellationToken.None).ConfigureAwait(false);
                PrintResponse(observeResponse.Response);

                Console.WriteLine("Observed messages for lamp!");

                Console.WriteLine("Press any key to unobserve.");
                Console.ReadLine();

                await coapClient.StopObservationAsync(observeResponse, CancellationToken.None).ConfigureAwait(false);
            }
        }

        class ResponseHandler : ICoapResponseHandler
        {
            public Task HandleResponseAsync(HandleResponseContext context)
            {
                Console.WriteLine("> RECEIVED OBSERVED RESOURCE");
                Console.WriteLine("    + Sequence number = " + context.SequenceNumber);
                PrintResponse(context.Response);
                return Task.CompletedTask;
            }
        }

        public static void PrintResponse(CoapResponse response)
        {
            if (response != null)
            {
                Console.WriteLine("> RESPONSE");
                Console.WriteLine("   + Status         = " + response.StatusCode);
                Console.WriteLine("   + Status code    = " + (int)response.StatusCode);
                Console.WriteLine("   + Content format = " + response.Options.ContentFormat);
                Console.WriteLine("   + Max age        = " + response.Options.MaxAge);
                Console.WriteLine("   + E tag          = " + ByteArrayToString(response.Options.ETag));
                Console.WriteLine("   + Payload        = " + Encoding.UTF8.GetString(response.Payload));
                Console.WriteLine();
            }
        }

        static string ByteArrayToString(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return string.Empty;
            }

            var hex = new StringBuilder(buffer.Length * 2);
            hex.Append("0x");

            foreach (var @byte in buffer)
            {
                hex.AppendFormat("{0:x2}", @byte);
            }

            return hex.ToString();
        }

        static async Task Main9()
        {
            var coapFactory = new CoapFactory();
            coapFactory.DefaultLogger.RegisterSink(new CoapNetLoggerConsoleSink());

            using (var coapClient = coapFactory.CreateClient())
            {
                Console.WriteLine("< CONNECTING...");

                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost("coap.me")
                    .Build();

                await coapClient.ConnectAsync(connectOptions, CancellationToken.None).ConfigureAwait(false);


                // separate

                var request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("/.well-known/core")
                    .Build();

                var response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);
                return;
                // separate

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("separate")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                // hello

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("hello")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                // broken

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("broken")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                // secret

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("secret")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                // large-create

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("large-create")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                // location1/location2/location3

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("location1/location2/location3")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                // large

                request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Get)
                    .WithPath("large")
                    .Build();

                response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }


        

        static async Task Main()
        {
            const string TEST_LARGE = @"
/-------------------------------------------------------------\
|                 RESOURCE BLOCK NO. 1 OF 8                   |
|               [each line contains 64 bytes]                 |
\-------------------------------------------------------------/
/-------------------------------------------------------------\
|                 RESOURCE BLOCK NO. 2 OF 8                   |
|               [each line contains 64 bytes]                 |
\-------------------------------------------------------------/
/-------------------------------------------------------------\
|                 RESOURCE BLOCK NO. 3 OF 8                   |
|               [each line contains 64 bytes]                 |
\-------------------------------------------------------------/
/-------------------------------------------------------------\
|                 RESOURCE BLOCK NO. 4 OF 8                   |
|               [each line contains 64 bytes]                 |
\-------------------------------------------------------------/
/-------------------------------------------------------------\
|                 RESOURCE BLOCK NO. 5 OF 8                   |
|               [each line contains 64 bytes]                 |
\-------------------------------------------------------------/
/-------------------------------------------------------------\
|                 RESOURCE BLOCK NO. 6 OF 8                   |
|               [each line contains 64 bytes]                 |
\-------------------------------------------------------------/
/-------------------------------------------------------------\
|                 RESOURCE BLOCK NO. 7 OF 8                   |
|               [each line contains 64 bytes]                 |
\-------------------------------------------------------------/
/-------------------------------------------------------------\
|                 RESOURCE BLOCK NO. 8 OF 8                   |
|               [each line contains 64 bytes]                 |
\-------------------------------------------------------------/
";

            var coapFactory = new CoapFactory();
            coapFactory.DefaultLogger.RegisterSink(new CoapNetLoggerConsoleSink());

            using (var coapClient = coapFactory.CreateClient())
            {
                Console.WriteLine("< CONNECTING...");

                var connectOptions = new CoapClientConnectOptionsBuilder()
                    .WithHost("2001:140::da7a:3bff:fe3d:ca9b")
                    .Build();

                await coapClient.ConnectAsync(connectOptions, CancellationToken.None).ConfigureAwait(false);

                //var observeOptions = new CoapObserveOptionsBuilder()
                //    .WithPath("ntp")
                //    .WithResponseHandler(new ResponseHandler())
                //    .Build();

                //var observeResponse = await coapClient.ObserveAsync(observeOptions, CancellationToken.None).ConfigureAwait(false);
               
               // PrintResponse(observeResponse.Response);

                var request = new CoapRequestBuilder()
                    .WithMethod(CoapRequestMethod.Post)
                    .WithPath("fwUpgrade")
                    .Build();

                var _optionFactory = new CoapMessageOptionFactory();
                request.Method = CoapRequestMethod.Post;
                request.BlockSize = CoAPnet.Protocol.CoapBlockSizeType.BLOCK_SIZE_64;
                request.Token = new CoapMessageToken(BitConverter.GetBytes(0xabcdef));
                request.Options.Others.Add(_optionFactory.CreateAccept((uint)CoapMessageContentFormat.TextPlain));
                request.Options.Others.Add(_optionFactory.CreateContentFormat(CoapMessageContentFormat.TextPlain));
                request.Payload = System.Text.ASCIIEncoding.UTF8.GetBytes(TEST_LARGE);
                request.Type = CoAPnet.Protocol.CoapMessageType.Confirmable;
                request.Interval = 10000;
                //request.Payload = System.Text.ASCIIEncoding.UTF8.GetBytes("12345678");
                request.RetransmissionCount = 2;
                var response = await coapClient.RequestAsync(request, CancellationToken.None);
                PrintResponse(response);




                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}