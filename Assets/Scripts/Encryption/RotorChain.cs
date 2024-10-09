using System.Collections.Generic;
using System.Linq;

namespace Encryption
{
    public class RotorChain
    {
        private readonly List<Rotor> _rotorChain;

        public RotorChain(IEnumerable<RotorConfiguration> rotorChain)
        {
            _rotorChain = InitializeRotorChain(rotorChain);
        }

        private static List<Rotor> InitializeRotorChain(IEnumerable<RotorConfiguration> ringConfigChain)
        {
            RotorConfiguration firstConfig = ringConfigChain.First();
            Rotor first = new(firstConfig.RotorProps, firstConfig.InitialPosition, firstConfig.StepCallback, firstConfig.RingSetting);
            List<Rotor> rotorList = new() { first };

            return ringConfigChain.Skip(1).Aggregate(rotorList, (currentList, config) =>
            {
                Rotor lastAdded = currentList.Last();
                currentList.Add(new Rotor(config.RotorProps, config.InitialPosition, (newPosition) =>
                {
                    lastAdded.Increment();
                    config.StepCallback?.Invoke(newPosition);
                }, config.RingSetting));

                return currentList;
            });
        }

        public char IncrementAndGetMappedCharacter(char input)
        {
            _rotorChain.Last().Increment();
            return _rotorChain.AsEnumerable().Reverse()
                .Aggregate(input, (lastRotorOutput, rotor) => rotor.GetMappedCharacter(lastRotorOutput));
        }

        public char GetInverseCharacter(char input)
        {
            return _rotorChain.Aggregate(input, (lastRotorOutput, rotor) => rotor.GetInverseCharacter(lastRotorOutput));
        }
    }
}
