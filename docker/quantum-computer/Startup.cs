using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Quantum.Simulation.Simulators;
using Newtonsoft.Json;

namespace Qooba.QuantumTeleportation
{
    public class Startup
    {
        private const string Teleportation = "/quantum/teleportation";

        private const string Text = "text";

        public void ConfigureServices(IServiceCollection services) { }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async(context) =>
            {
                if (context.Request.Path != Teleportation)
                {
                    return;
                }

                var text = context.Request.Query.FirstOrDefault(x => x.Key == Text).Value;
                using(var sim = new QuantumSimulator())
                {
                    var b = System.Text.Encoding.UTF8.GetBytes(text);
                    var inputs = b.SelectMany(x => ConvertByteToBoolArray(x));
                    var output = new List<bool>();
                    foreach (var input in inputs)
                    {
                        var received = TeleportClassicalMessage.Run(sim, input).Result;
                        output.Add(received);
                    }

                    var outputBytes = new List<byte>();
                    for (int i = 0; i < b.Length; i++)
                    {
                        var r = ConvertBoolArrayToByte(output.Skip(8 * i).Take(8).ToArray());
                        outputBytes.Add(r);
                    }

                    var outputText = System.Text.Encoding.UTF8.GetString(outputBytes.ToArray());
                    var response = JsonConvert.SerializeObject(new { TeleportedText = outputText, QubitsCount = inputs.Count() });
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(response);
                }
            });
        }

        private static bool[] ConvertByteToBoolArray(byte value)
        {
            var result = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                result[i] = (value & (1 << i)) == 0 ? false : true;
            }

            Array.Reverse(result);
            return result;
        }

        private static byte ConvertBoolArrayToByte(bool[] source)
        {
            byte result = 0;
            int index = 8 - source.Length;

            foreach (var value in source)
            {
                if (value)
                {
                    result |= (byte) (1 << (7 - index));
                }

                index++;
            }

            return result;
        }
    }
}