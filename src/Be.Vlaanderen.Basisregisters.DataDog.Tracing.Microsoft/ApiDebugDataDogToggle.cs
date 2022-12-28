namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.Microsoft
{
    using FeatureToggle;

    public class ApiDebugDataDogToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ApiDebugDataDogToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
