namespace Browser.FormsApp.Behaviors
{
    using System;

    using Smart.Forms.Interactivity;

    using Xamarin.Forms;

    public sealed class SideMenuBehavior : BehaviorBase<View>
    {
        private PanGestureRecognizer recognizer;

        private bool showSideMenu;

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
                ViewExtensions.CancelAnimations(AssociatedObject);

                var rect = AbsoluteLayout.GetLayoutBounds(AssociatedObject);
                showSideMenu = rect.X > -AssociatedObject.Width;
                lastPosition = rect.X;
            }
            else if (e.StatusType == GestureStatus.Running)
            {
                var rect = AbsoluteLayout.GetLayoutBounds(AssociatedObject);
                rect.X = Math.Max(Math.Min(0, rect.X + e.TotalX), -AssociatedObject.Width);
                AbsoluteLayout.SetLayoutBounds(AssociatedObject, rect);

                if (rect.X > lastPosition)
                {
                    showSideMenu = true;
                }
                else if (rect.X < lastPosition)
                {
                    showSideMenu = false;
                }
                lastPosition = rect.X;
            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                var rect = AbsoluteLayout.GetLayoutBounds(AssociatedObject);
                var pos = showSideMenu ? 0 : -AssociatedObject.Width;
                var length = (uint)(250 * Math.Abs(pos - rect.X) / AssociatedObject.Width);

                AssociatedObject.Animate(
                    "Slide",
                    x =>
                    {
                        rect.X = x;
                        AbsoluteLayout.SetLayoutBounds(AssociatedObject, rect);
                    },
                    rect.X,
                    pos,
                    length: length);
            }
        }
    }
}
