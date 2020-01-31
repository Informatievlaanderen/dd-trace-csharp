namespace Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac
{
    using FeatureToggle;

    public class ApiDataDogToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public ApiDataDogToggle(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
