using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace UR.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RandomizerController : ControllerBase
    {
        private readonly ILogger<RandomizerController> _logger;

        public RandomizerController(ILogger<RandomizerController> logger)
        {
           _logger = logger;
        }

        private int[] RollDice(int sides, int count)
        {
            var results = new int[count];
            var sidesRange = (byte.MaxValue / sides) * sides;
            using (var provider = new RNGCryptoServiceProvider())
            {
                var buffer = new byte[1];
                for (var i = 0; i < count; i++)
                {
                    provider.GetBytes(buffer);

                    // Provide equal range value opportunity for the dice sides.
                    if (sidesRange < buffer[0])
                    {
                        // Outside range so we must re-roll this result
                        i--;
                    }
                    else
                    {
                        // Under the range so equal share means equal odds.
                        results[i] = (((int)buffer[0]) % sides) + 1;
                    }
                }
            }

            return results;
        }

        [HttpGet("[action]/{id}/{type}/{count}")]
        public int[] Get(string id, int sides, int count)
        {
            // TODO: Add eventing
            var dices = RollDice(sides, count);
            return dices;
        }
    }
}
