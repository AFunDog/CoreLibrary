using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml.Media.Animation;

namespace CoreServices.WinUI.Animation
{
    public record ConverterArgs<T>(T From, T to, double Progress);

    public abstract class PropertyAnimation
    {
        public event Action<object, PropertyAnimation>? Finished;

        protected readonly object _target;
        protected readonly string _propertyName;
        protected Task? _animtaionTask;
        protected readonly CancellationTokenSource _cts = new();
        protected readonly EasingFunctionBase? _easingFunction;
        protected TimeSpan _duration;
        protected readonly TimeSpan _delay;
        protected Stopwatch _stopwatch = new();

        public object Target => _target;
        public string PropertyName => _propertyName;

        protected PropertyAnimation(
            object target,
            string propertyName,
            EasingType easingType,
            EasingMode easingMode,
            TimeSpan duration,
            TimeSpan delay
        )
        {
            _target = target;
            _propertyName = propertyName;
            _easingFunction = easingType.ToEasingFunction(easingMode);
            _duration = duration;
            _delay = delay;
        }

        public abstract void StartAnimation();
        public abstract void StopAnimation();
    }

    public sealed class PropertyAnimation<PropertyType> : PropertyAnimation
        where PropertyType : notnull
    {
        private PropertyType _from;
        private PropertyType _to;
        private readonly Func<ConverterArgs<PropertyType>, PropertyType> _converter;

        public PropertyAnimation(
            object target,
            string propertyName,
            PropertyType from,
            PropertyType to,
            Func<ConverterArgs<PropertyType>, PropertyType> converter,
            EasingType easingType = EasingType.Cubic,
            EasingMode easingMode = EasingMode.EaseOut,
            TimeSpan duration = default,
            TimeSpan delay = default
        )
            : base(target, propertyName, easingType, easingMode, duration, delay)
        {
            _from = from;
            _to = to;
            _converter = converter;
        }

        public override void StartAnimation()
        {
            if (_animtaionTask is null || _animtaionTask.IsCompleted)
            {
                _animtaionTask = AnimationAsync();
                Debug.WriteLine("动画开始");
            }
        }

        private async Task AnimationAsync()
        {
            _duration = _duration == default ? TimeSpan.FromSeconds(1) : _duration;
            var property =
                _target
                    .GetType()
                    .GetProperty(
                        _propertyName,
                        System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.NonPublic
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.Static
                    ) ?? throw new ArgumentNullException("属性不存在");

            _stopwatch ??= new();

            await Task.Yield();
            await Task.Delay(_delay, _cts.Token);
            _stopwatch.Restart();

            while (_stopwatch.ElapsedMilliseconds < _duration.TotalMilliseconds)
            {
                _cts.Token.ThrowIfCancellationRequested();
                property.SetValue(
                    _target,
                    _converter(
                        new(
                            _from,
                            _to,
                            _easingFunction?.Ease(_stopwatch.ElapsedMilliseconds / _duration.TotalMilliseconds)
                                ?? _stopwatch.ElapsedMilliseconds / _duration.TotalMilliseconds
                        )
                    )
                );
                await Task.Delay(6, _cts.Token);
            }
            property.SetValue(_target, _converter(new(_from, _to, 1)));
            _stopwatch.Stop();
            Debug.WriteLine("动画结束");
        }

        public override void StopAnimation()
        {
            if (_animtaionTask is not null && !_animtaionTask.IsCompleted)
            {
                _cts.Cancel();
            }
        }

        public void UpdateToValue(PropertyType to, bool isLengthenDuration = true)
        {
            if (_animtaionTask is null || _animtaionTask.IsCompleted)
                return;
            _from = _converter(
                new(
                    _from,
                    _to,
                    _easingFunction?.Ease(_stopwatch.ElapsedMilliseconds / _duration.TotalMilliseconds)
                        ?? _stopwatch.ElapsedMilliseconds / _duration.TotalMilliseconds
                )
            );
            _to = to;
            _stopwatch.Restart();
        }
    }
}
