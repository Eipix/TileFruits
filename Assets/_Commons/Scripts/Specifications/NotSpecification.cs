namespace Commons.Specifications
{
    internal class NotSpecification : ISpecification
    {
        private readonly ISpecification _specification;

        public NotSpecification(ISpecification specification)
        {
            _specification = specification;
        }

        public bool IsSatisfied() => !_specification.IsSatisfied();
    }
}
