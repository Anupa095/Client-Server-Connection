using Grpc.Core;
using CalculatorServer;
using System.Threading.Tasks;

namespace CalculatorServer.Services
{
    public class CalculatorService : Calculator.CalculatorBase
    {
        // Square
        public override Task<CalculationResponse> Square(CalculationRequest request, ServerCallContext context)
        {
            int number = request.Number;
            int result = number * number;

            return Task.FromResult(new CalculationResponse
            {
                Result = result,
                IsSuccess = true,
                ErrorMessage = "",
                VectorClock = { request.VectorClock }
            });
        }

        // Cube
        public override Task<CalculationResponse> Cube(CalculationRequest request, ServerCallContext context)
        {
            int number = request.Number;
            int result = number * number * number;

            return Task.FromResult(new CalculationResponse
            {
                Result = result,
                IsSuccess = true,
                ErrorMessage = "",
                VectorClock = { request.VectorClock }
            });
        }

        // Slow Multiply
        public override async Task<CalculationResponse> SlowMultiply(MultiplyRequest request, ServerCallContext context)
        {
            await Task.Delay(2000); // simulate delay
            int result = request.Number1 * request.Number2;

            return new CalculationResponse
            {
                Result = result,
                IsSuccess = true,
                ErrorMessage = "",
                VectorClock = { request.VectorClock }
            };
        }
    }
}
