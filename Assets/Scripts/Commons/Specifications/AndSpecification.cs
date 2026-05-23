namespace Commons.Specifications
{
    internal class AndSpecification : ISpecification
    {
        private readonly ISpecification _left;
        private readonly ISpecification _right;

        public AndSpecification(ISpecification left, ISpecification right)
        {
            _left = left;
            _right = right;
        }

        public bool IsSatisfied() => _left.IsSatisfied() && _right.IsSatisfied();
    }
}
