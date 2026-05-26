namespace Commons.Specifications
{
    public static class SpecificationExtensions
    {
        public static ISpecification And(this ISpecification left, ISpecification right)
        {
            return new AndSpecification(left, right);
        }

        public static ISpecification Or(this ISpecification left, ISpecification right)
        {
            return new OrSpecification(left, right);
        }

        public static ISpecification Not(this ISpecification specification)
        {
            return new NotSpecification(specification);
        }
    }
}
