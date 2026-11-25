using System;

namespace LOGIYGames.Abilities
{
    public class AbilityStateMachine
    {
        private AbilityState _current;
        private AbilityContext _ctx;
        public bool IsFinished { get; private set; }
        public AbilityState Current => _current;

        public AbilityStateMachine(AbilityContext ctx) => _ctx = ctx;

        public void ChangeState(AbilityState next)
        {
            _current?.Exit(_ctx);
            _current = next;
            _current?.Enter(_ctx);
        }

        public void Update(float deltaTime)
        {
            if (IsFinished || _current == null) return;

            _current.Update(_ctx);

            // Отмена разрешена только из TargetingState и только если стратегия позволяет
            // (вызывается через TryCancel — см. ниже)
        }

        /// <summary>
        /// Запрос на отмену. Разрешён только в TargetingState, если стратегия позволяет.
        /// Вызывать только из Update.
        /// </summary>
        public void TryCancel()
        {
            if (IsFinished || _current == null) return;
            if (_current is TargetingState && _current.CanCancel(_ctx))
            {
                Finish();
            }
        }

        public void Finish()
        {
            _current?.Exit(_ctx);
            _current = null;
            IsFinished = true;
        }
    }
}
