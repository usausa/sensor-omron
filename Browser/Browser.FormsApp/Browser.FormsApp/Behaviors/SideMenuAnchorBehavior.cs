namespace Browser.FormsApp.Behaviors
{
    using System;
    using Smart.Forms.Interactivity;

    using Xamarin.Forms;

    public sealed class SideMenuAnchorBehavior : BehaviorBase<View>
    {
        public static readonly BindableProperty TargetObjectProperty = BindableProperty.Create(
            nameof(TargetObject),
            typeof(VisualElement),
            typeof(SetFocusAction));

        public VisualElement TargetObject
        {
            get => (VisualElement)GetValue(TargetObjectProperty);
            set => SetValue(TargetObjectProperty, value);
        }

        private PanGestureRecognizer recognizer;

        private bool showTargetObject;

        private double lastPosition;

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);

            recognizer = new PanGestureRecognizer();
            recognizer.PanUpdated += OnPanUpdated;
            bindable.GestureRecognizers.Add(recognizer);
        }

        protected override void OnDetachingFrom(View bindable)
        {
            recognizer.PanUpdated -= OnPanUpdated;
            bindable.GestureRecognizers.Remove(recognizer);

            base.OnDetachingFrom(bindable);
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Started)
            {
                ViewExtensions.CancelAnimations(TargetObject);

                var rect = AbsoluteLayout.GetLayoutBounds(TargetObject);
                showTargetObject = rect.X > -TargetObject.Width;
                lastPosition = rect.X;
            }
            else if (e.StatusType == GestureStatus.Running)
            {
                var rect = AbsoluteLayout.GetLayoutBounds(TargetObject);
                rect.X = Math.Max(Math.Min(0, e.TotalX - TargetObject.Width), -TargetObject.Width);
                AbsoluteLayout.SetLayoutBounds(TargetObject, rect);

                if (rect.X > lastPosition)
                {
                    showTargetObject = true;
                }
                else if (rect.X < lastPosition)
                {
                    showTargetObject = false;
                }
                lastPosition = rect.X;
            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                var rect = AbsoluteLayout.GetLayoutBounds(TargetObject);
                var pos = showTargetObject ? 0 : -TargetObject.Width;
                var length = (uint)(250 * Math.Abs(pos - rect.X) / TargetObject.Width);

                TargetObject.Animate(
                    "Slide",
                    x =>
                    {
                        rect.X = x;
                        AbsoluteLayout.SetLayoutBounds(TargetObject, rect);
                    },
                    rect.X,
                    pos,
                    length: length);
            }
        }
    }
}
