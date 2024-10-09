namespace Encryption
{
    public record RotorConfiguration(RotorProps RotorProps, char InitialPosition, Rotor.StepMechanismCallback StepCallback = null, char RingSetting = 'A')
    {
        public RotorProps RotorProps { get; } = RotorProps;
        public char InitialPosition { get; } = InitialPosition;
        public Rotor.StepMechanismCallback StepCallback { get; } = StepCallback;
        public char RingSetting { get; } = RingSetting;
    }
}
