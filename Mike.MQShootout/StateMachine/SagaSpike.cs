using System;

namespace Mike.MQShootout.StateMachine
{
    public class SagaSpike
    {
        public void Show_how_a_saga_machine_might_look()
        {
            var saga = new Saga();

            saga.Initially(State.Ready).On<NewOrder>((msg, sm) =>
            {
                DoSomeStuffWith(msg);
                sm.Bus.Publish(new StockCheckRequest());
                sm.TransitionTo(State.ReceivedOrder);
            });

            saga.At(State.ReceivedOrder).On<StockAvailibility>((msg, sm) =>
            {
                if (msg.InStock)
                {
                    DoSomeStuffWith(msg);
                    sm.Bus.Publish(new TakePaymentRequest());
                    sm.TransitionTo(State.CheckedStock);
                }
                else
                {
                    sm.TransitionTo(State.Rejected);
                }
            });

            saga.At(State.CheckedStock).On<TakePaymentResponse>((msg, sm) =>
            {
                if (msg.PaymentFailed)
                {
                    sm.TransitionTo(State.Rejected);
                }
                else
                {
                    DoSomeStuffWith(msg);
                    sm.Bus.Publish(new DispatchOrder());
                    sm.TransitionTo(State.Dispatched);
                }
            });

            saga.EndWith(State.Dispatched).Or().EndWith(State.Rejected);
        }

        public void DoSomeStuffWith<T>(T message) where T : IMessage {}
    }

    public interface IMessage { }
    public class NewOrder : IMessage { }
    public class StockCheckRequest : IMessage { }
    public class StockAvailibility : IMessage
    {
        public bool InStock { get; set; }
    }
    public class TakePaymentRequest : IMessage { }
    public class TakePaymentResponse : IMessage
    {
        public bool PaymentFailed { get; set; }
    }
    public class DispatchOrder : IMessage { }

    public enum State
    {
        Ready,
        ReceivedOrder,
        CheckedStock,
        TakenPayment,
        Dispatched,
        Rejected
    }

    public class Saga
    {
        public Saga Initially(State state) { return null; }
        public Saga At(State state) { return null; }
        public void On<T>(Action<T, Saga> messageAction) where T : IMessage { }
        public void TransitionTo(State checkedStock) { }
        public MessageBus Bus { get; set; }
        public Saga EndWith(State state) { return null; }
        public Saga Or() { return null; }
    }

    public class MessageBus
    {
        public void Publish<T>(T message) where T : IMessage { }
    }
}