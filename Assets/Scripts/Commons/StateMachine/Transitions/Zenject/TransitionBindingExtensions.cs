using System;
using Zenject;

namespace Commons.StateMachine.Transitions.Zenject
{
    public static class TransitionBindingExtensions
    {
        public static void BindTransitions(
            this DiContainer container,
            string id,
            Func<TransitionDsl, Transition[]> builder)
        {
            container.Bind<Transition[]>()
                .WithId(id)
                .FromMethod(ctx =>
                {
                    var dsl = new TransitionDsl(ctx.Container);
                    return builder(dsl);
                })
                .AsCached();
        }

        public static void BindTransitions<TState>(
            this DiContainer container,
            Func<TransitionDsl, Transition[]> builder) where TState : State
        {
            container.Bind<Transition[]>()
                .FromMethod(ctx =>
                {
                    var dsl = new TransitionDsl(ctx.Container);
                    return builder(dsl);
                })
                .AsCached()
                .WhenInjectedInto<TState>();
        }
    }
}
